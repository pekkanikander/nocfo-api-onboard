// ===== deps =====
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

let slug = "demo-720e8e"

// ===== helpers =====
let inline get (m: Map<_,_>) k = m |> Map.tryFind k

let printTotals (totals: AccountClassTotals) =
    let order = [ Asset; Liability; Equity; Income; Expense ]
    for cls in order do
        let v = get totals cls |> Option.defaultValue 0M
        printfn "%-10A %12M" cls v
    let assets      =  get totals Asset     |> Option.defaultValue 0M
    let liabilities =  get totals Liability |> Option.defaultValue 0M
    let equity      =  get totals Equity    |> Option.defaultValue 0M
    let pnl         = (get totals Income    |> Option.defaultValue 0M)
                    - (get totals Expense   |> Option.defaultValue 0M)
    printfn "—"
    printfn "Assets     %12.2M" assets
    printfn "Liab       %12.2M" liabilities
    printfn "Equity     %12.2M" equity
    printfn "P&L        %12.2M" pnl
    printfn "Check (A = L + E)  Δ = %M" (assets - (liabilities + equity))

// ===== run =====

// Find the Business by slug (streamBusinesses -> pick matching Full)
let businessKey =
    Streams.streamBusinesses accounting
    |> AsyncSeq.choose (function
        | Ok (Hydratable.Full b) when b.key.slug = slug -> Some b.key
        | _ -> None)
    |> AsyncSeq.tryHead
    |> Async.RunSynchronously
    |> function
       | Some key -> key
       | None -> failwithf "Business with slug '%s' not found or not accessible." slug

let ctx : BusinessContext = { key = businessKey; ctx = accounting }

// Fold accounts -> class totals (accumulate errors, don’t stop on first)
let step (totals, errs) (r: Result<Account, DomainError>) = async {
    match r with
    | Error e -> return totals, e :: errs
    | Ok acc ->
        let! updated = Reports.addToTotals totals acc
        match updated with
        | Ok t      -> return t, errs
        | Error err -> return totals, err :: errs
}

let totals, errors =
    Streams.streamAccounts ctx
    |> AsyncSeq.foldAsync step (Map.empty, [])
    |> Async.RunSynchronously

printfn "Trial balance for slug=%s\n" slug
printTotals totals

match List.rev errors with
| [] -> ()
| es ->
    printfn "\nWarnings/Errors (%d):" es.Length
    es |> List.truncate 5 |> List.iter (printfn "- %A")
    if es.Length > 5 then printfn "… (%d more)" (es.Length - 5)
