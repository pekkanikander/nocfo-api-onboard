#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "bin/Debug/net9.0/hawaii-client.dll"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"

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

let baseUrl = Uri("https://api-tst.nocfo.io")
let token =
    match Environment.GetEnvironmentVariable "NOCFO_TOKEN" with
    | null | "" -> failwith "NOCFO_TOKEN not set"
    | t -> t

let http = Http.createHttpClient baseUrl
let client = NocfoApiClient(http)

let fetchPage (page: int) = async {
    let url = baseUrl.OriginalString.TrimEnd('/') + $"/v1/business/?page_size=5&page={page}"
    let! result = Http.getJson<PaginatedBusinessList> http url (Http.withAuth token)
    match result with
    | Ok payload -> return payload
    | Error e ->
        printfn "HTTP %A while fetching page %d. Content:\n%s" e.statusCode page e.body
        return (failwithf "Unexpected status %A" e.statusCode)
}

let resultsOf (p: PaginatedBusinessList) = p.results
let nextOf (p: PaginatedBusinessList) = p.next

let stream = paginateByPageWithNextOption fetchPage resultsOf nextOf

printfn "Fetching first 7 businesses via AsyncSeq pagination...\n"

let first7 =
    stream
    |> AsyncSeq.take 7
    |> AsyncSeq.toListSynchronously

printfn "Fetched %d businesses" first7.Length
first7 |> List.iteri (fun i b ->
    let slug = defaultArg b.slug "(none)"
    printfn "#%d id=%d name=%s slug=%s" (i+1) b.id b.name slug)
