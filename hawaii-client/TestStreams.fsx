#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "bin/Debug/net10.0/hawaii-client.dll"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"

#load "TestSupport.fsx"
open TestSupport

open System
open FSharp.Control
open NocfoClient
open NocfoClient.Streams
open NocfoClient.Http
open Nocfo.Domain
open NocfoApi.Types

printfn "Streaming first 7 businesses...\n"

let first7businesses =
    Nocfo.Domain.Streams.streamBusinesses accounting
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

let businessKey =
    match first7businesses.[0] with
    | Ok (Nocfo.Domain.Business.Partial (key, _)) -> key
    | Ok (Nocfo.Domain.Business.Full full) -> full.key
    | Error e -> failwithf "Error fetching business: %A" e

let businessContext : BusinessContext = { key = businessKey; ctx = accounting }

let first7accounts =
    Nocfo.Domain.Streams.streamAccounts businessContext
    |> AsyncSeq.take 7
    |> AsyncSeq.toListSynchronously

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
)

printfn "\nStreaming first 7 documents for first business...\n"

let first7documents =
    Nocfo.Domain.Streams.streamDocuments businessContext
    |> AsyncSeq.take 7
    |> AsyncSeq.toListSynchronously

printfn "Fetched %d documents" first7documents.Length
first7documents |> List.iteri (fun i d ->
    let document =
        match d with
        | Ok document -> document
        | Error e -> failwithf "Error fetching document: %A" e
    match document with
    | Nocfo.Domain.Document.Partial (p, _) ->
        printfn "#%d id=%d number=%s" (i+1) p.id (defaultArg p.number "<none>")
    | Nocfo.Domain.Document.Full full ->
        printfn "#%d id=%d number=%s" (i+1) full.id (defaultArg full.number "<none>")
)

printfn "\n--- Testing streamPatches: mutate first account number to 9999 and back ---\n"

// Extract first business slug (for URL) and first account id/number (for patching)

let (firstAccountId, originalNumber) =
    match first7accounts.[0] with
    | Ok (Nocfo.Domain.Account.Partial (p, _)) -> p.id, p.number
    | Ok (Nocfo.Domain.Account.Full f) -> f.id, f.number
    | Error e -> failwithf "Error fetching first account: %A" e

let accountPath = sprintf "/business/%s/account/%d/" businessKey.slug firstAccountId
printfn "Target account: business=%s accountId=%d path=%s" businessKey.slug firstAccountId accountPath
printfn "Original number = %s" originalNumber

// Helper to read back the account's current number from the stream
let getCurrentNumber () =
    Nocfo.Domain.Streams.streamAccounts businessContext
    |> AsyncSeq.choose (fun r ->
        match r with
        | Ok (Nocfo.Domain.Account.Partial (p, _)) when p.id = firstAccountId -> Some p.number
        | Ok (Nocfo.Domain.Account.Full f) when f.id = firstAccountId -> Some f.number
        | _ -> None)
    |> AsyncSeq.toListSynchronously
    |> function
       | n :: _ -> n
       | [] -> failwithf "Account %d not found when reloading" firstAccountId

// 1) Patch to 9999
let toNineNineNineNine = AsyncSeq.ofSeq [ {| number = "9999" |} ]
let step1 =
    Streams.streamPatches<{| number: string |}, unit> accounting.http (fun _ -> accountPath) toNineNineNineNine
    |> AsyncSeq.toListSynchronously

step1 |> List.iteri (fun i r ->
    match r with
    | Ok () -> printfn "PATCH step %d succeeded (-> 9999)" (i+1)
    | Error e -> printfn "PATCH step %d failed: %A" (i+1) e)

let afterFirst = getCurrentNumber ()
printfn "After first patch: number = %s (expected 9999)" afterFirst

// 2) Patch back to original
let backToOriginal = AsyncSeq.ofSeq [ {| number = originalNumber |} ]
let step2 =
    streamPatches<{| number: string |}, unit> accounting.http (fun _ -> accountPath) backToOriginal
    |> AsyncSeq.toListSynchronously

step2 |> List.iteri (fun i r ->
    match r with
    | Ok () -> printfn "PATCH step %d succeeded (-> original)" (i+1)
    | Error e -> printfn "PATCH step %d failed: %A" (i+1) e)

let afterSecond = getCurrentNumber ()
printfn "After second patch: number = %s (expected %s)" afterSecond originalNumber
