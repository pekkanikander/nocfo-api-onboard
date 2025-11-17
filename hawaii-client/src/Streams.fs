namespace NocfoClient

open System
open System.Net.Http
open FSharp.Control
open NocfoApi.Types
open NocfoClient
open NocfoClient.Http
open NocfoClient.AsyncSeqHelpers

module Streams =
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

    let private streamChanges<'Payload, 'Response>
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
