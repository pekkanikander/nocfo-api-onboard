#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "bin/Debug/net10.0/hawaii-client.dll"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"

open System
open FSharp.Control
open Newtonsoft.Json.Linq
open Nocfo.Domain

let private mkAccount (id: int) (number: string) =
    NocfoApi.Types.Account.Create(
        id = id,
        created_at = DateTimeOffset.UnixEpoch,
        updated_at = DateTimeOffset.UnixEpoch,
        number = number,
        padded_number = id,
        name = JObject.Parse("""{"fi":"x"}"""),
        name_translations = JObject.Parse("""{"fi":"x"}"""),
        header_path = [],
        default_vat_rate = 0.0,
        balance = 0.0f,
        is_used = true
    )

let private mkDelta (id: int) =
    NocfoApi.Types.PatchedAccount.Create(id)

let private accountStream ids =
    ids
    |> Seq.map (fun (id, number) -> Ok (mkAccount id number))
    |> AsyncSeq.ofSeq

let private deltaStream ids =
    ids
    |> Seq.map (fun id -> Ok (mkDelta id))
    |> AsyncSeq.ofSeq

let private normalizeResult (result: Result<Option<NocfoApi.Types.Account * NocfoApi.Types.PatchedAccount>, DomainError>) =
    match result with
    | Ok (Some (account, delta)) -> $"ok-some:{account.id}:{delta.id}"
    | Ok None -> "ok-none"
    | Error (DomainError.Unexpected msg) -> $"error:{msg}"
    | Error (DomainError.Http err) -> $"http:{err.statusCode}"

let private assertEqual label expected actual =
    if expected <> actual then
        failwithf "%s failed.\nExpected: %A\nActual:   %A" label expected actual
    else
        printfn "%s: ok" label

let testFullyAligned () =
    let accounts = accountStream [ (1, "1000"); (2, "2000") ]
    let deltas = deltaStream [ 1; 2 ]
    let actual =
        Alignment.alignAccountsPermissive accounts deltas
        |> AsyncSeq.toListAsync
        |> Async.RunSynchronously
        |> List.map normalizeResult

    let expected =
        [ "ok-some:1:1"
          "ok-some:2:2" ]
    assertEqual "fully-aligned" expected actual

let testMissingRight () =
    let accounts = accountStream [ (1, "1000"); (2, "2000") ]
    let deltas = deltaStream [ 1 ]
    let actual =
        Alignment.alignAccountsPermissive accounts deltas
        |> AsyncSeq.toListAsync
        |> Async.RunSynchronously
        |> List.map normalizeResult

    let expected =
        [ "ok-some:1:1"
          "ok-none" ]
    assertEqual "missing-right" expected actual

let testMissingLeft () =
    let accounts = accountStream [ (1, "1000") ]
    let deltas = deltaStream [ 1; 2 ]
    let actual =
        Alignment.alignAccountsPermissive accounts deltas
        |> AsyncSeq.toListAsync
        |> Async.RunSynchronously
        |> List.map normalizeResult

    if actual |> List.exists (fun item -> item.StartsWith("error:Alignment failure: missing account for CSV id 2.")) |> not then
        failwithf "missing-left failed.\nExpected an error for CSV id 2.\nActual: %A" actual
    else
        printfn "missing-left: ok"

let testStableOrdering () =
    let accounts = accountStream [ (1, "1000"); (3, "3000"); (4, "4000") ]
    let deltas = deltaStream [ 1; 2; 4 ]
    let actual =
        Alignment.alignAccountsPermissive accounts deltas
        |> AsyncSeq.toListAsync
        |> Async.RunSynchronously
        |> List.map normalizeResult

    let expected =
        [ "ok-some:1:1"
          "error:Alignment failure: missing account for CSV id 2."
          "ok-none"
          "ok-some:4:4" ]
    assertEqual "stable-ordering" expected actual

testFullyAligned ()
testMissingRight ()
testMissingLeft ()
testStableOrdering ()
printfn "All alignAccountsPermissive regression checks passed."
