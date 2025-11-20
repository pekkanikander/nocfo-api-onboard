namespace NocfoClient

open FSharp.Control
open FSharp.Core
open NocfoClient.Http

module Result =
    let existsOk p result =
        match result with
          | Ok value -> (p value)
          | Error _-> true

module AsyncResult =

    let inline liftAsync (hof: 'T -> 'U) (ar: Async<'T>) : Async<'U> =
        async {
            let! result = ar
            return hof result
        }
    let map f         = liftAsync (Result.map f)
    let bind f        = liftAsync (Result.bind f)
    let mapError f    = liftAsync (Result.mapError f)
    let existsOk p    = liftAsync (Result.existsOk p)

module AsyncSeq =
    let inline liftAsync (hof: 'T seq -> 'T option) (ar: AsyncSeq<'T>) : Async<'T option> =
        async {
            let! result = AsyncSeq.toListAsync ar
            return hof result
        }
    let inline tryHead (s: AsyncSeq<'T>) : Async<'T option> = liftAsync (Seq.tryHead) s

    /// Filter AsyncSeq<Result<_,_>> values, keeping only the Ok values that satisfy the predicate.
    /// Errors pass through untouched so that the consumer can decide how to handle them.
    let filter (predicate: 'T -> bool) (source: AsyncSeq<Result<'T, 'Error>>) : AsyncSeq<Result<'T, 'Error>> =
        FSharp.Control.AsyncSeq.filter (Result.existsOk predicate) source


module AsyncSeqHelpers =
    let nullToEmptyList (items: 'T list) =
        if isNull (box items) then [] else items


    /// Paginates by invoking fetchPage starting from page=1, yielding items lazily while the page's `next` is Some.
    /// Works for any page type that exposes `results : 'Item list` and `next : string option` members.
    let inline paginateByPageSRTP< ^Page, 'Item
                                when ^Page : (member results : 'Item list)
                                 and ^Page : (member next    : string option) >
        (fetchPage: int -> Async<Result< ^Page , HttpError>>)
        : AsyncSeq<Result<'Item, HttpError>> =

        let inline resultsOf (p:^Page) : 'Item list   = (^Page : (member results : 'Item list) (p))
        let inline nextOf    (p:^Page) : string option = (^Page : (member next    : string option) (p))

        let rec loop pageNumber =
            asyncSeq {
                let! result = fetchPage pageNumber
                match result with
                | Ok page ->
                    for item in nullToEmptyList (resultsOf page) do
                        yield Ok item
                    match nextOf page with
                    | Some _ -> yield! loop (pageNumber + 1)
                    | None -> ()
                | Error e ->
                    yield Error e
            }

        loop 1
