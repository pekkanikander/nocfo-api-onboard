namespace NocfoClient

open FSharp.Control

module AsyncSeqHelpers =
    let nullToEmptyList (items: 'T list) =
        if isNull (box items) then [] else items

    /// Paginates by invoking fetchPage starting from page=1, yielding items lazily while the page's `next` is Some.
    /// Works for any page type that exposes `results : 'Item list` and `next : string option` members.
    let inline paginateByPageSRTP< ^Page, 'Item
                                when ^Page : (member results : 'Item list)
                                 and ^Page : (member next    : string option) >
        (fetchPage: int -> Async< ^Page >)
        : AsyncSeq<'Item> =

        let inline resultsOf (p:^Page) : 'Item list = (^Page : (member results : 'Item list) (p))
        let inline nextOf    (p:^Page) : string option = (^Page : (member next    : string option) (p))

        let rec loop pageNumber : AsyncSeq<'Item> = asyncSeq {
            let! page = fetchPage pageNumber
            for item in resultsOf page do
                yield item
            match nextOf page with
            | Some _ -> yield! loop (pageNumber + 1)
            | None   -> ()
        }
        loop 1
