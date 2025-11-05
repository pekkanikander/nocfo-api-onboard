namespace NocfoClient

open FSharp.Control

module AsyncSeqHelpers =
    let nullToEmptyList (items: 'T list) =
        if isNull (box items) then [] else items

    /// Paginates by invoking fetchPage starting from page=1, yielding items lazily until hasNext is false.
    let paginateByPage fetchPage getResults hasNext =
        let rec loop pageNumber : AsyncSeq<'Item> =
            asyncSeq {
                let! page = fetchPage pageNumber
                let items = page |> getResults |> nullToEmptyList
                yield! AsyncSeq.ofSeq items
                if hasNext page then
                    yield! loop (pageNumber + 1)
            }
        loop 1

    /// Convenience: same as paginateByPage but uses a getNext function that returns Some url when there is another page.
    let paginateByPageWithNextOption fetchPage getResults getNext =
        paginateByPage fetchPage getResults (fun page -> getNext page |> Option.isSome)
