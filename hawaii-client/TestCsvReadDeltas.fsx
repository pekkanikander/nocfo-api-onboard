#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "bin/Debug/net10.0/hawaii-client.dll"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"
#r @"/Users/pnr/.nuget/packages/csvhelper/33.1.0/lib/netstandard2.0/CsvHelper.dll"

open System.IO
open FSharp.Control
open Nocfo.Domain

let private csvText = """id,number,name_translations
1,1500,"[{""key"":""fi"",""value"":""Kassa""}]"
2,,
"""

let private fail label expected actual =
    failwithf "%s failed.\nExpected: %A\nActual:   %A" label expected actual

let deltas =
    use reader = new StringReader(csvText)
    Nocfo.Csv.readDeltas<AccountDelta, NocfoApi.Types.PatchedAccountRequest> reader None
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

match deltas with
| [ first; second ] ->
    if first.id <> 1 then fail "first.id" 1 first.id
    if first.patch.number <> Some "1500" then fail "first.patch.number" (Some "1500") first.patch.number

    let expectedTranslations =
        Some [ NocfoApi.Types.PatchedAccountRequestNametranslations.Create("fi", "Kassa") ]

    if first.patch.name_translations <> expectedTranslations then
        fail "first.patch.name_translations" expectedTranslations first.patch.name_translations

    if second.id <> 2 then fail "second.id" 2 second.id
    if second.patch.number <> None then fail "second.patch.number" None second.patch.number
    if second.patch.name_translations <> None then fail "second.patch.name_translations" None second.patch.name_translations

    printfn "readDeltas regression: ok"
| other ->
    fail "delta row count" 2 other.Length
