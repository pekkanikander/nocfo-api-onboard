#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"
#load "src/Endpoints.fs"
#load "src/Http.fs"
#load "src/AsyncSeq.fs"
#load "src/Streams.fs"
#load "src/PatchShape.fs"
#load "src/Domain.fs"

open System
open System.Net
open System.Net.Http
open System.Text
open System.Threading
open System.Threading.Tasks
open System.Collections.Generic
open FSharp.Control
open Newtonsoft.Json.Linq
open Nocfo.Domain
open NocfoClient
open NocfoApi.Http
open NocfoApi.Types

type RecordedRequest =
    { method: string
      path: string
      body: string option }

let private accountPathPrefix = "/v1/business/test-slug/account/"
let private contactPathPrefix = "/v1/business/test-slug/contacts/"

let private mkResponse (statusCode: HttpStatusCode) (body: string) =
    let response = new HttpResponseMessage(statusCode)
    response.Content <- new StringContent(body, Encoding.UTF8, "application/json")
    response

let private mkNotFound () =
    mkResponse HttpStatusCode.NotFound """{"detail":"Not found."}"""

let private tryParseId (prefix: string) (path: string) =
    let trimmed = path.TrimEnd('/')
    if trimmed.StartsWith(prefix, StringComparison.Ordinal) then
        trimmed.Substring(prefix.Length) |> int |> Some
    else
        None

let private toAccountTranslations (items: PatchedAccountRequestNametranslations list) =
    items |> List.map (fun item -> Nametranslations.Create(item.key, item.value))

let private applyAccountPatch (current: NocfoApi.Types.Account) (patch: PatchedAccountRequest) =
    { current with
        number = defaultArg patch.number current.number
        name_translations =
            match patch.name_translations with
            | Some value -> toAccountTranslations value
            | None -> current.name_translations
        header_id = if patch.header_id.IsSome then patch.header_id else current.header_id
        description = if patch.description.IsSome then patch.description else current.description
        ``type`` = if patch.``type``.IsSome then patch.``type`` else current.``type``
        default_vat_code = if patch.default_vat_code.IsSome then patch.default_vat_code else current.default_vat_code
        default_vat_rate_label = if patch.default_vat_rate_label.IsSome then patch.default_vat_rate_label else current.default_vat_rate_label
        opening_balance = if patch.opening_balance.IsSome then patch.opening_balance else current.opening_balance }

let private applyContactPatch (current: NocfoApi.Types.Contact) (patch: PatchedContactRequest) =
    { current with
        customer_id = if patch.customer_id.IsSome then patch.customer_id else current.customer_id
        ``type`` = if patch.``type``.IsSome then patch.``type`` else current.``type``
        name = defaultArg patch.name current.name
        name_aliases = if patch.name_aliases.IsSome then patch.name_aliases else current.name_aliases
        contact_business_id = if patch.contact_business_id.IsSome then patch.contact_business_id else current.contact_business_id
        notes = if patch.notes.IsSome then patch.notes else current.notes
        phone_number = if patch.phone_number.IsSome then patch.phone_number else current.phone_number
        is_invoicing_enabled = if patch.is_invoicing_enabled.IsSome then patch.is_invoicing_enabled else current.is_invoicing_enabled
        invoicing_email = if patch.invoicing_email.IsSome then patch.invoicing_email else current.invoicing_email
        invoicing_einvoice_address = if patch.invoicing_einvoice_address.IsSome then patch.invoicing_einvoice_address else current.invoicing_einvoice_address
        invoicing_einvoice_operator = if patch.invoicing_einvoice_operator.IsSome then patch.invoicing_einvoice_operator else current.invoicing_einvoice_operator
        invoicing_tax_code = if patch.invoicing_tax_code.IsSome then patch.invoicing_tax_code else current.invoicing_tax_code
        invoicing_street = if patch.invoicing_street.IsSome then patch.invoicing_street else current.invoicing_street
        invoicing_city = if patch.invoicing_city.IsSome then patch.invoicing_city else current.invoicing_city
        invoicing_postal_code = if patch.invoicing_postal_code.IsSome then patch.invoicing_postal_code else current.invoicing_postal_code
        invoicing_country = if patch.invoicing_country.IsSome then patch.invoicing_country else current.invoicing_country
        invoicing_language = if patch.invoicing_language.IsSome then patch.invoicing_language else current.invoicing_language }

type FakeHandler(initialAccounts: NocfoApi.Types.Account seq, initialContacts: NocfoApi.Types.Contact seq) =
    inherit HttpMessageHandler()

    let accounts = Dictionary<int, NocfoApi.Types.Account>()
    let contacts = Dictionary<int, NocfoApi.Types.Contact>()
    let requests = ResizeArray<RecordedRequest>()

    do
        for account in initialAccounts do
            accounts[account.id] <- account
        for contact in initialContacts do
            contacts[contact.id] <- contact

    member _.Requests = requests |> Seq.toList
    member _.Account(id: int) = accounts[id]
    member _.Contact(id: int) = contacts[id]

    override _.SendAsync(request: HttpRequestMessage, _cancellationToken: CancellationToken) =
        task {
            let! body =
                task {
                    if isNull request.Content then
                        return None
                    else
                        let! content = request.Content.ReadAsStringAsync()
                        return Some content
                }

            let path = request.RequestUri.AbsolutePath
            requests.Add({ method = request.Method.Method; path = path; body = body })

            match request.Method.Method, tryParseId accountPathPrefix path, tryParseId contactPathPrefix path with
            | "GET", Some accountId, _ ->
                match accounts.TryGetValue accountId with
                | true, account ->
                    return mkResponse HttpStatusCode.OK (Serializer.serialize account)
                | false, _ ->
                    return mkNotFound ()
            | "PATCH", Some accountId, _ ->
                match accounts.TryGetValue accountId, body with
                | (true, account), Some content ->
                    let patch = Serializer.deserialize<PatchedAccountRequest> content
                    let updated = applyAccountPatch account patch
                    accounts[accountId] <- updated
                    return mkResponse HttpStatusCode.OK (Serializer.serialize updated)
                | _ ->
                    return mkNotFound ()
            | "GET", _, Some contactId ->
                match contacts.TryGetValue contactId with
                | true, contact ->
                    return mkResponse HttpStatusCode.OK (Serializer.serialize contact)
                | false, _ ->
                    return mkNotFound ()
            | "PATCH", _, Some contactId ->
                match contacts.TryGetValue contactId, body with
                | (true, contact), Some content ->
                    let patch = Serializer.deserialize<PatchedContactRequest> content
                    let updated = applyContactPatch contact patch
                    contacts[contactId] <- updated
                    return mkResponse HttpStatusCode.OK (Serializer.serialize updated)
                | _ ->
                    return mkNotFound ()
            | _ ->
                return mkResponse HttpStatusCode.InternalServerError """{"detail":"Unhandled request."}"""
        }

let private mkBusinessKey () =
    { id = BusinessIdentifier.Create(1, JToken.FromObject("y_tunnus"), "2999322-9")
      slug = "test-slug" }

let private mkContext (handler: HttpMessageHandler) =
    let client = new HttpClient(handler)
    client.BaseAddress <- Uri("https://example.test/v1")
    let http = Http.ofHttpClient client "test-token"
    { ctx = Accounting.ofHttp http
      key = mkBusinessKey () }

let private mkAccount (id: int) (number: string) =
    NocfoApi.Types.Account.Create(
        id = id,
        created_at = DateTimeOffset.UnixEpoch,
        updated_at = DateTimeOffset.UnixEpoch,
        number = number,
        padded_number = id,
        name = $"Account {id}",
        name_translations = [ Nametranslations.Create("fi", $"Account {id}") ],
        header_path = [],
        default_vat_rate = 0.0,
        is_shown = true,
        balance = 0.0f,
        is_used = true
    )

let private mkContact (id: int) (name: string) (email: string option) =
    { NocfoApi.Types.Contact.Create(
        id = id,
        created_at = DateTimeOffset.UnixEpoch,
        updated_at = DateTimeOffset.UnixEpoch,
        name = name,
        can_be_invoiced = true,
        can_be_invoiced_via_email = true,
        can_be_invoiced_via_einvoice = false
      ) with
        invoicing_email = email }

let private mkAccountDelta (id: int) (number: string option) : AccountDelta =
    { id = id
      patch = { PatchedAccountRequest.Create() with number = number } }

let private mkContactDelta (id: int) (name: string option) (email: string option) : ContactDelta =
    { id = id
      patch =
        { PatchedContactRequest.Create() with
            name = name
            invoicing_email = email } }

let private assertEqual label expected actual =
    if expected <> actual then
        failwithf "%s failed.\nExpected: %A\nActual:   %A" label expected actual

let private assertTrue label value =
    if not value then
        failwithf "%s failed." label

let private patchBodies prefix (handler: FakeHandler) =
    handler.Requests
    |> List.choose (fun request ->
        if request.method = "PATCH" && request.path.StartsWith(prefix, StringComparison.Ordinal) then
            request.body
        else
            None)

let private runAccountUpdates (handler: FakeHandler) deltas =
    Account.executeDeltaUpdates (mkContext handler) (deltas |> Seq.map Ok |> AsyncSeq.ofSeq)
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

let private runContactUpdates (handler: FakeHandler) deltas =
    Contact.executeDeltaUpdates (mkContext handler) (deltas |> Seq.map Ok |> AsyncSeq.ofSeq)
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

let testAccountOutOfOrder () =
    let handler = new FakeHandler([ mkAccount 1 "1000"; mkAccount 2 "2000" ], [])
    let results =
        runAccountUpdates handler
            [ mkAccountDelta 2 (Some "2500")
              mkAccountDelta 1 (Some "1500") ]

    let updatedIds =
        results
        |> List.choose (function | Ok (AccountUpdated account) -> Some account.id | _ -> None)

    assertEqual "account-out-of-order-ids" [ 2; 1 ] updatedIds
    assertEqual "account-out-of-order-state-1" "1500" (handler.Account(1)).number
    assertEqual "account-out-of-order-state-2" "2500" (handler.Account(2)).number

let testAccountMissingIdContinues () =
    let handler = new FakeHandler([ mkAccount 1 "1000" ], [])
    let results =
        runAccountUpdates handler
            [ mkAccountDelta 999 (Some "9999")
              mkAccountDelta 1 (Some "1100") ]

    match results with
    | [ Error (DomainError.Unexpected msg); Ok (AccountUpdated account) ] ->
        assertTrue "account-missing-id-error" (msg.Contains("missing account for CSV id 999"))
        assertEqual "account-missing-id-follow-up" "1100" account.number
    | other ->
        failwithf "account-missing-id-continuation failed: %A" other

let testAccountDuplicateIdSequential () =
    let handler = new FakeHandler([ mkAccount 1 "1000" ], [])
    let results =
        runAccountUpdates handler
            [ mkAccountDelta 1 (Some "2000")
              mkAccountDelta 1 (Some "3000") ]

    let updatedNumbers =
        results
        |> List.choose (function | Ok (AccountUpdated account) -> Some account.number | _ -> None)

    assertEqual "account-duplicate-sequential-results" [ "2000"; "3000" ] updatedNumbers
    assertEqual "account-duplicate-sequential-state" "3000" (handler.Account(1)).number
    assertEqual "account-duplicate-sequential-patch-count" 2 (patchBodies accountPathPrefix handler |> List.length)

let testAccountDuplicateIdNoOpSecondPatch () =
    let handler = new FakeHandler([ mkAccount 1 "1000" ], [])
    let results =
        runAccountUpdates handler
            [ mkAccountDelta 1 (Some "2000")
              mkAccountDelta 1 (Some "2000") ]

    let updatedNumbers =
        results
        |> List.choose (function | Ok (AccountUpdated account) -> Some account.number | _ -> None)

    assertEqual "account-duplicate-noop-results" [ "2000" ] updatedNumbers
    assertEqual "account-duplicate-noop-patch-count" 1 (patchBodies accountPathPrefix handler |> List.length)

let testContactOutOfOrder () =
    let handler = new FakeHandler([], [ mkContact 1 "Alice" (Some "alice@example.test"); mkContact 2 "Bob" None ])
    let results =
        runContactUpdates handler
            [ mkContactDelta 2 (Some "Bobby") None
              mkContactDelta 1 None (Some "alice+new@example.test") ]

    let updatedIds =
        results
        |> List.choose (function | Ok (ContactUpdated contact) -> Some contact.id | _ -> None)

    assertEqual "contact-out-of-order-ids" [ 2; 1 ] updatedIds
    assertEqual "contact-out-of-order-name" "Bobby" (handler.Contact(2)).name
    assertEqual "contact-out-of-order-email" (Some "alice+new@example.test") (handler.Contact(1)).invoicing_email

let testContactMissingIdContinues () =
    let handler = new FakeHandler([], [ mkContact 1 "Alice" None ])
    let results =
        runContactUpdates handler
            [ mkContactDelta 999 (Some "Ghost") None
              mkContactDelta 1 (Some "Alice Updated") None ]

    match results with
    | [ Error (DomainError.Unexpected msg); Ok (ContactUpdated contact) ] ->
        assertTrue "contact-missing-id-error" (msg.Contains("missing contact for CSV id 999"))
        assertEqual "contact-missing-id-follow-up" "Alice Updated" contact.name
    | other ->
        failwithf "contact-missing-id-continuation failed: %A" other

let testContactDuplicateIdSequentialAndNoOp () =
    let handler = new FakeHandler([], [ mkContact 1 "Alice" None ])
    let results =
        runContactUpdates handler
            [ mkContactDelta 1 (Some "Alice One") None
              mkContactDelta 1 (Some "Alice Two") None
              mkContactDelta 1 (Some "Alice Two") None ]

    let updatedNames =
        results
        |> List.choose (function | Ok (ContactUpdated contact) -> Some contact.name | _ -> None)

    assertEqual "contact-duplicate-sequential-results" [ "Alice One"; "Alice Two" ] updatedNames
    assertEqual "contact-duplicate-sequential-state" "Alice Two" (handler.Contact(1)).name
    assertEqual "contact-duplicate-sequential-patch-count" 2 (patchBodies contactPathPrefix handler |> List.length)

testAccountOutOfOrder ()
testAccountMissingIdContinues ()
testAccountDuplicateIdSequential ()
testAccountDuplicateIdNoOpSecondPatch ()
testContactOutOfOrder ()
testContactMissingIdContinues ()
testContactDuplicateIdSequentialAndNoOp ()
printfn "All update-by-id regression checks passed."
