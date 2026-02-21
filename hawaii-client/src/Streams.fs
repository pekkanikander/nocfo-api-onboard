namespace NocfoClient

open System
open System.Net.Http
open FSharp.Control
open NocfoApi.Types
open NocfoClient
open NocfoClient.Http
open NocfoClient.AsyncSeqHelpers

type StreamAlignment<'T1, 'T2> =
    | Aligned of 'T1 * 'T2
    | MissingLeft  of 'T2  // There is no left item for this right item
    | MissingRight of 'T1  // There is no right item for this left item

module Streams =

    let alignByKey<'L, 'R, 'Key when 'Key : comparison>
        (keyL: 'L -> 'Key)
        (keyR: 'R -> 'Key)
        (left : AsyncSeq<'L>)
        (right: AsyncSeq<'R>)
        : AsyncSeq<StreamAlignment<'L,'R>> =

    // local alias for brevity
        let moveNext (e: IAsyncEnumerator<'T>) = e.MoveNext()

        asyncSeq {
            use eL = left.GetEnumerator()
            use eR = right.GetEnumerator()

            let mutable lBuf : 'L option = None
            let mutable rBuf : 'R option = None
            let mutable finished = false

            while not finished do
                // fill buffers if empty
                if lBuf.IsNone then
                    let! lOpt = moveNext eL
                    lBuf <- lOpt
                if rBuf.IsNone then
                    let! rOpt = moveNext eR
                    rBuf <- rOpt

                match lBuf, rBuf with
                | None, None ->
                    finished <- true

                | Some l, None ->
                    yield MissingRight l
                    lBuf <- None

                | None, Some r ->
                    yield MissingLeft r
                    rBuf <- None

                | Some l, Some r ->
                    let kl, kr = keyL l, keyR r
                    match compare kl kr with
                    | 0 ->
                        yield Aligned (l, r)
                        lBuf <- None
                        rBuf <- None
                    | c when c < 0 ->
                        yield MissingRight l
                        lBuf <- None      // advance left only
                    | _ ->
                        yield MissingLeft r
                        rBuf <- None      // advance right only
        }

    let inline streamPaginated< ^Page, 'Item
        when ^Page : (member results : 'Item list)
         and ^Page : (member next    : string option) >
        (http: HttpContext)
        (relativeForPage: int -> string)
        : AsyncSeq<Result<'Item, HttpError>> =

        let fetchPage (page: int) : Async<Result< ^Page , HttpError>> = async {
            let! result = Http.getJson< ^Page > http (relativeForPage page)
            return result
        }
        paginateByPageSRTP fetchPage

    let streamChanges<'Payload, 'Response>
        (change: 'Payload -> Async<Result<'Response, HttpError>>)
        (source: AsyncSeq<'Payload>)
        : AsyncSeq<Result<'Response, HttpError>> =
        source
        |> AsyncSeq.mapAsync change


    let streamPatches<'Payload, 'Response>
        (http: HttpContext)
        (getPath: 'Payload -> string)
        (source: AsyncSeq<'Payload>)
        : AsyncSeq<Result<'Response, HttpError>> =
        source |> streamChanges (fun payload ->
            Http.patchJson<'Payload, 'Response> http (getPath payload) payload)

    let streamCreates<'Payload, 'Response>
        (http: HttpContext)
        (getPath: 'Payload -> string)
        (source: AsyncSeq<'Payload>)
        : AsyncSeq<Result<'Response, HttpError>> =
        source |> streamChanges (fun payload ->
            Http.postJson<'Payload, 'Response> http (getPath payload) payload)

    // TODO: Currently unused. Consider removing.
    let streamDeletes<'Payload>
        (http: HttpContext)
        (getPath: 'Payload -> string)
        (source: AsyncSeq<'Payload>)
        : AsyncSeq<Result<unit, HttpError>> =
        source |> streamChanges (fun payload ->
            Http.deleteJson<unit> http (getPath payload))
