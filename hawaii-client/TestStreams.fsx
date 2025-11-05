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
