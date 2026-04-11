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

let private csvTextWithIgnoredExtraColumns = """id,notes,name_aliases
1,hello,[BROKEN VALUE]
"""

let private csvTextWithStringEnums = """id,type,invoicing_language
1,PERSON,fi
2,BUSINESS,en
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

let contactDeltas =
    use reader = new StringReader(csvTextWithIgnoredExtraColumns)
    Nocfo.Csv.readDeltas<ContactDelta, NocfoApi.Types.PatchedContactRequest> reader (Some [ "id"; "notes" ])
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

match contactDeltas with
| [ first ] ->
    if first.id <> 1 then fail "contact.id" 1 first.id
    if first.patch.notes <> Some "hello" then fail "contact.patch.notes" (Some "hello") first.patch.notes
    if first.patch.name_aliases <> None then fail "contact.patch.name_aliases" None first.patch.name_aliases
    printfn "readDeltas ignored-extra-columns regression: ok"
| other ->
    fail "contact delta row count" 1 other.Length

let enumDeltas =
    use reader = new StringReader(csvTextWithStringEnums)
    Nocfo.Csv.readDeltas<ContactDelta, NocfoApi.Types.PatchedContactRequest> reader (Some [ "id"; "type"; "invoicing_language" ])
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

match enumDeltas with
| [ first; second ] ->
    if first.id <> 1 then fail "enum.first.id" 1 first.id
    if first.patch.``type`` <> Some NocfoApi.Types.ContactTypeEnum.PERSON then
        fail "enum.first.type" (Some NocfoApi.Types.ContactTypeEnum.PERSON) first.patch.``type``
    if first.patch.invoicing_language <> Some NocfoApi.Types.InvoicingLanguageEnum.Fi then
        fail "enum.first.language" (Some NocfoApi.Types.InvoicingLanguageEnum.Fi) first.patch.invoicing_language

    if second.id <> 2 then fail "enum.second.id" 2 second.id
    if second.patch.``type`` <> Some NocfoApi.Types.ContactTypeEnum.BUSINESS then
        fail "enum.second.type" (Some NocfoApi.Types.ContactTypeEnum.BUSINESS) second.patch.``type``
    if second.patch.invoicing_language <> Some NocfoApi.Types.InvoicingLanguageEnum.En then
        fail "enum.second.language" (Some NocfoApi.Types.InvoicingLanguageEnum.En) second.patch.invoicing_language

    printfn "readDeltas string-enum regression: ok"
| other ->
    fail "enum delta row count" 2 other.Length
