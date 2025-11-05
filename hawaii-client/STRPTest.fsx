#r "nuget: FSharp.Control.AsyncSeq"
open FSharp.Control

type Page<'T> = { results: 'T list; next: string option }

let inline paginateByPageSRTP (fetch: int -> Async< ^P >) : AsyncSeq<'T>
  when ^P : (member results : 'T list)
   and ^P : (member next    : string option) =
  let inline resultsOf (p:^P) = (^P : (member results : 'T list) (p))
  let inline nextOf    (p:^P) = (^P : (member next    : string option) (p))
  let rec loop i = asyncSeq {
    let! p = fetch i
    for x in resultsOf p do yield x
    match nextOf p with Some _ -> yield! loop (i+1) | None -> ()
  }
  loop 1

let pages =
  let data = [ [1;2;3]; [4;5]; [] ]
  let fetch i = async { return { results = data.[i-1]; next = if i < data.Length then Some "n" else None } }
  paginateByPageSRTP fetch |> AsyncSeq.toListAsync |> Async.RunSynchronously
// expect [1;2;3;4;5]
