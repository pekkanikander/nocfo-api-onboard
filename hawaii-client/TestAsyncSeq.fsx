#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "bin/Debug/net10.0/hawaii-client.dll"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"

#load "TestSupport.fsx"
open TestSupport


open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open FSharp.Control
open NocfoClient
open NocfoClient.AsyncSeqHelpers
open NocfoClient.Http
open NocfoApi
open NocfoApi.Types

let fetchPage (page: int) =
    getJson<PaginatedBusinessList> context (Endpoints.businessList page)

let stream = paginateByPageSRTP fetchPage

printfn "Fetching first 7 businesses via AsyncSeq pagination...\n"

let first7 =
    stream
    |> AsyncSeq.take 7
    |> AsyncSeq.toListSynchronously

printfn "Fetched %d businesses" first7.Length
first7 |> List.iteri (fun i result ->
    let business =
        match result with
        | Ok business -> business
        | Error e -> failwithf "Error fetching business: %A" e
    let slug = defaultArg business.slug "(none)"
    printfn "#%d id=%d name=%s slug=%s" (i+1) business.id business.name slug)
