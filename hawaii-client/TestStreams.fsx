#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "bin/Debug/net9.0/hawaii-client.dll"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"

open System
open FSharp.Control
open NocfoClient
open NocfoClient.Streams
open NocfoClient.Http
open NocfoApi.Types


let baseUrl = Uri("https://api-tst.nocfo.io")
let token =
    match Environment.GetEnvironmentVariable "NOCFO_TOKEN" with
    | null | "" -> failwith "NOCFO_TOKEN not set"
    | t -> t

let http = Http.createHttpClient baseUrl

printfn "Streaming first 7 businesses...\n"

let first7 =
    streamBusinesses http baseUrl token
    |> AsyncSeq.take 7
    |> AsyncSeq.toListSynchronously

printfn "Fetched %d businesses" first7.Length
first7 |> List.iteri (fun i b ->
    let slug = defaultArg b.slug "(none)"
    printfn "#%d id=%d name=%s slug=%s" (i+1) b.id b.name slug)

printfn "Streaming first 7 accounts for first business'...\n"

let first7accounts =
    let slug = first7.[0].slug
    match slug with
    | Some slug ->
        Nocfo.Domain.Streams.streamAccountsByBusinessSlug http baseUrl token slug
    | None ->
        failwith "No slug found for first business"
    |> AsyncSeq.take 7
    |> AsyncSeq.toListSynchronously

printfn "Fetched %d accounts" first7accounts.Length
first7accounts |> List.iteri (fun i a ->
    match a with
    | Nocfo.Domain.Account.Partial (p, _) ->
        printfn "#%d id=%d number=%s" (i+1) p.id p.number
    | Nocfo.Domain.Account.Full f ->
        printfn "#%d id=%d number=%s" (i+1) f.id f.number
    | _ ->
        failwith "Account is not a partial or full account"
)
