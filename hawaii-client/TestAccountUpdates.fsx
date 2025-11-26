#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "bin/Debug/net9.0/hawaii-client.dll"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"

open System
open System.IO
open FSharp.Control
open Nocfo.Domain
open NocfoClient
open NocfoClient.Streams

let nocfoToken =
    match Environment.GetEnvironmentVariable "NOCFO_TOKEN" with
    | null | "" -> failwith "NOCFO_TOKEN is not set."
    | value -> value

let baseUrl =
    match Environment.GetEnvironmentVariable "NOCFO_BASE_URL" with
    | null | "" -> "https://api-tst.nocfo.io"
    | value -> value

let http = Http.createHttpContext (Uri baseUrl) nocfoToken
let accounting = Accounting.ofHttp http

let businessId = "2999322-9"

let businessContext =
    match BusinessResolver.resolve accounting businessId |> Async.RunSynchronously with
    | Ok ctx -> ctx
    | Error err -> failwithf "Failed to resolve business: %A" err

let cachedAccounts =
    Streams.streamAccounts businessContext
    |> Streams.hydrateAndUnwrap
    |> AsyncSeq.cache

let guid = Guid.NewGuid().ToString("N")
let csvPath =
    let file = Path.Combine(Path.GetTempPath(), $"account-deltas-{guid}.csv")
    file

let writeInitialCsv () =
    async {
        use writer = new StreamWriter(csvPath)
        writer.WriteLine("id,number")
        do!
            cachedAccounts
            |> AsyncSeq.iter (function
                | Error err -> raise (DomainStreamException err)
                | Ok account ->
                    writer.WriteLine($"{account.id},{account.number}"))
        writer.Flush()
    }
writeInitialCsv () |> Async.RunSynchronously
printfn "Wrote baseline CSV to %s" csvPath

let firstAccountId =
    cachedAccounts
    |> AsyncSeq.choose (function
        | Ok account -> Some account.id
        | Error _ -> None)
    |> AsyncSeq.tryHead
    |> Async.RunSynchronously
    |> Option.defaultWith (fun () -> failwith "No accounts in stream.")

let csvDeltas =
    File.ReadLines(csvPath)
    |> Seq.skip 1
    |> Seq.map (fun line ->
        let parts = line.Split(',')
        let id = Int32.Parse(parts[0])
        let number =
            if parts.Length > 1 then parts[1] else ""
        let delta = { NocfoApi.Types.PatchedAccount.Create(id) with number = Some number }
        Ok delta)
    |> AsyncSeq.ofSeq


let modifiedDeltas =
    csvDeltas
    |> AsyncSeq.map (Result.map (fun delta ->
        if delta.id = firstAccountId then
            let deltaNumber = defaultArg delta.number ""
            { delta with number = Some $"{deltaNumber}-patched" }
        else
            delta))

let commands =
    Account.deltasToCommands cachedAccounts modifiedDeltas
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

printfn "Commands:"
commands |> List.iter (printfn "  %A")
