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
open Nocfo.Domain
open NocfoApi.Types


let baseUrl = Uri("https://api-tst.nocfo.io")
let token =
    match Environment.GetEnvironmentVariable "NOCFO_TOKEN" with
    | null | "" -> failwith "NOCFO_TOKEN not set"
    | t -> t

let http = Http.createHttpContext baseUrl token

printfn "Streaming first 7 businesses...\n"

let first7businesses =
    Nocfo.Domain.Streams.streamBusinesses http
    |> AsyncSeq.take 7
    |> AsyncSeq.toListSynchronously

printfn "Fetched %d businesses" first7businesses.Length
first7businesses |> List.iteri (fun i (result: Result<Nocfo.Domain.Business, DomainError>) ->
    match result with
    | Ok business ->
        match business with
        | Nocfo.Domain.Business.Partial (key, _) ->
            printfn "#%d id=%A slug=%s" (i+1) key.id key.slug
        | Nocfo.Domain.Business.Full full ->
            printfn "#%d id=%A name=%s slug=%s" (i+1) full.key.id full.meta.name full.key.slug
    | Error e ->
        printfn "#%d error=%A" (i+1) e
)

printfn "Streaming first 7 accounts for first business'...\n"

let first7accounts =

    let business =
        match first7businesses.[0] with
        | Ok business -> business
        | Error e -> failwithf "Error fetching business: %A" e

    let context : Nocfo.Domain.BusinessContext = {
        key =
            match business with
            | Nocfo.Domain.Business.Partial (key, _) -> key
            | Nocfo.Domain.Business.Full full -> full.key
        http = http
    }
    let accounts =
        Nocfo.Domain.Streams.streamAccounts context
        |> AsyncSeq.take 7
        |> AsyncSeq.toListSynchronously
    accounts

printfn "Fetched %d accounts" first7accounts.Length
first7accounts |> List.iteri (fun i a ->
    let account =
        match a with
        | Ok account -> account
        | Error e -> failwithf "Error fetching account: %A" e
    match account with
    | Nocfo.Domain.Account.Partial (p, _) ->
        printfn "#%d id=%d number=%s" (i+1) p.id p.number
    | Nocfo.Domain.Account.Full full ->
        printfn "#%d id=%d number=%s" (i+1) full.id full.number
    | _ ->
        failwith "Account is not a partial or full account"
)
