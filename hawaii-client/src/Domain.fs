namespace Nocfo.Domain

open System
open FSharp.Control
open Nocfo
open NocfoApi.Types
open NocfoClient
open NocfoClient.Http
open NocfoClient.Streams
open NocfoClient

/// Domain-level generics

/// Domain-level error channel (extend as needed)
type DomainError =
  | Http of Http.HttpError
  | Unexpected of string

module DomainError =
  let inline asDomain r = Result.mapError DomainError.Http r

/// Generic hydratable wrapper carrying a partial payload and a fetch that upgrades it
///
/// In this domain, we use Full and Partial only to represent the lifecycle.
/// Hence, we do not use RQA for them, even though LLMs suggest it.
type Hydratable<'Full,'Partial> =
  | Partial of partial: 'Partial * fetch: (unit -> Async<Result<Hydratable<'Full,'Partial>, DomainError>>)
  | Full    of full:    'Full

/// ------------------------------------------------------------
/// Accounting model: Businesses, Accounts, and Reports
/// ------------------------------------------------------------

/// Businesses

/// Businesses are identified by their VAT ID or other similar identifier, and a slug.
/// This identifier is assumed to be serializable, immutable and stable.
/// The slug is fetched lazily from the API on demand.
type BusinessKey = {
  id: NocfoApi.Types.BusinessIdentifier
  slug: string
}

type BusinessMeta = {
  name: string
  country: string option
}

type BusinessFull = {
  key:  BusinessKey
  meta: BusinessMeta
  raw:  NocfoApi.Types.Business
}

/// Business is a hydratable of its full form, with BusinessKey as the partial
type Business = Hydratable<BusinessFull, BusinessKey>
type BusinessDelta = NocfoApi.Types.PatchedBusiness // XXX: Not implemented yet

/// ------------------------------------------------------------
/// Accounts
/// ------------------------------------------------------------

type AccountClass = Asset | Liability | Equity | Income | Expense

type AccountClassTotals = Map<AccountClass, decimal>

/// Accounts are identified by their ID. Each account is associated with a business,
/// but we don't model that relationship yet, as we don't need it yet.
type AccountFull  = NocfoApi.Types.Account
type AccountRow   = NocfoApi.Types.AccountList
type AccountDelta = NocfoApi.Types.PatchedAccount

/// Account is a hydratable of its full form with AccountRow as the partial
type Account  = Hydratable<AccountFull, AccountRow>

/// Domain-level account commands expressing intent before hitting HTTP.
type AccountCommand =
  | CreateAccount of Account
  | UpdateAccount of accountId:int * AccountDelta
  | DeleteAccount of accountId:int

/// Result of executing an account command.
type AccountResult =
  | AccountUpdated of AccountFull
  | AccountDeleted of int

/// ------------------------------------------------------------
/// Documents
/// ------------------------------------------------------------

/// Documents now have separate list/detail DTOs in generated types.
/// For current flows we keep list payloads as both row and full values.
type DocumentFull = NocfoApi.Types.DocumentList
type DocumentRow  = NocfoApi.Types.DocumentList
type Document     = Hydratable<DocumentFull, DocumentRow>

/// ------------------------------------------------------------
/// Domain-level error channel (extend as needed)
/// ------------------------------------------------------------
exception DomainStreamException of DomainError

module AsyncSeqResult =
  let unwrapOrThrow (stream: AsyncSeq<Result<'T, DomainError>>) : AsyncSeq<'T> =
    stream
    |> AsyncSeq.map (function
        | Ok value -> value
        | Error err -> raise (DomainStreamException err))

module Alignment =

  let inline alignEntries< ^L, ^R, 'r
    when ^L : (member id : int)
     and ^R : (member id : int) >
    (onAligned     : ^L -> ^R -> Result<'r, DomainError>)
    (onMissingLeft : ^R -> Result<'r, DomainError>)   // right-only
    (onMissingRight: ^L -> Result<'r, DomainError>)   // left-only
    (left          : AsyncSeq<Result<^L, DomainError>>)
    (right         : AsyncSeq<Result<^R, DomainError>>)
    : AsyncSeq<Result<'r, DomainError>> =

    let computation () =
      let leftPlain  = AsyncSeqResult.unwrapOrThrow left
      let rightPlain = AsyncSeqResult.unwrapOrThrow right

      NocfoClient.Streams.alignByKey
        (fun (l: ^L) -> l.id)
        (fun (r: ^R) -> r.id)
        leftPlain
        rightPlain
      |> AsyncSeq.map (function
          | StreamAlignment.Aligned (l, r) -> onAligned l r
          | StreamAlignment.MissingLeft  r -> onMissingLeft  r
          | StreamAlignment.MissingRight l -> onMissingRight l)

    asyncSeq {
      try
        yield! computation ()
      with
      | DomainStreamException err ->
          yield Error err
    }
  // Old behaviour, preserver for now. TODO: remove.
  let alignAccounts
    (accounts: AsyncSeq<Result<AccountFull, DomainError>>)
    (deltas  : AsyncSeq<Result<AccountDelta, DomainError>>)
    =

    let onAligned account delta =
      Ok (account, delta)

    let onMissingLeft (missingAccount: AccountDelta) =
      let key = missingAccount.id
      Error (DomainError.Unexpected $"Alignment failure: missing CSV row for account id {key}.")

    let onMissingRight (missingAccount: AccountFull) =
      let key = missingAccount.id
      Error (DomainError.Unexpected $"Alignment failure: missing account for CSV id {key}.")

    alignEntries<AccountFull, AccountDelta, AccountFull * AccountDelta>
      onAligned onMissingLeft onMissingRight accounts deltas

  // New behaviour, allow CSV file to have missing rows.
  let alignAccountsPermissive
    (accounts: AsyncSeq<Result<AccountFull, DomainError>>)
    (deltas  : AsyncSeq<Result<AccountDelta, DomainError>>)
    =

    let onAligned (account: AccountFull) (delta: AccountDelta) =
      Ok (Some (account, delta))

    // MissingLeft: we have a delta (CSV row) but no matching account from API -> still an error.
    let onMissingLeft (missingDelta: AccountDelta) =
      let key = missingDelta.id
      Error (DomainError.Unexpected $"Alignment failure: missing account for CSV id {key}.")

    // MissingRight: we have an account from API but no matching CSV row -> allowed, delta = None.
    let onMissingRight (_missingAccount: AccountFull) =
      Ok None

    alignEntries<AccountFull, AccountDelta, Option<AccountFull * AccountDelta>> onAligned onMissingLeft onMissingRight accounts deltas


/// ------------------------------------------------------------
/// Contexts
/// ------------------------------------------------------------

/// AccountingContext
/// AccountingContext represents the repository, which at this point is just a HTTP client
/// that allows us to fetch the business and its associated data.

type AccountingOptions = {
  pageSize: int
  retryPolicy:   Option<unit> // XXX: Not implemented yet
  loggingPolicy: Option<unit> // XXX: Not implemented yet
  cachingPolicy: Option<unit> // XXX: Not implemented yet
}

type AccountingContext = {
  http: HttpContext
  options: AccountingOptions
}

/// BusinessContext

// At runtime we bind everything to a business context.
type BusinessContext = {
  ctx:  AccountingContext
  key:  BusinessKey
}

/// ------------------------------------------------------------
/// Accounting operations
/// ------------------------------------------------------------

module Accounting =
  let ofHttp (http: HttpContext) : AccountingContext =
    {
      http = http
      options = {
        pageSize      = 100
        retryPolicy   = None
        loggingPolicy = None
        cachingPolicy = None
      }
    }

///
/// Business module operations
///

module Business =
  let ofContext (context: BusinessContext) : Business =
    Hydratable.Partial (context.key, fetch = fun () -> async {
      let! result =
        Http.getJson<NocfoApi.Types.Business> context.ctx.http (Endpoints.businessBySlug context.key.slug)
      match result with
      | Result.Ok business ->
          let full : BusinessFull =
            { key  = context.key
              meta = { name = business.name; country = Option.ofObj business.country };
              raw  = business }
          return Ok (Business.Full full)
      | Result.Error httpErr ->
          return Error (DomainError.Http httpErr)
    })

  let ofRaw (raw: NocfoApi.Types.Business) : Business =
    let full : BusinessFull =
      {
        // XXX fixme: what if there are no identifiers or no slug
        key  = { id = raw.identifiers.[0]; slug = defaultArg raw.slug "(none)" }
        meta = { name = raw.name; country = Option.ofObj raw.country };
        raw  = raw
      }
    Business.Full full

  let hydrate (business: Business) : Async<Result<Business, DomainError>> =
    match business with
    | Full _ -> async.Return (Ok business)
    | Partial (key, fetch) -> fetch ()

///
/// Account module operations
///

module Account =
  let mkFetch (context: BusinessContext) (id) : unit -> Async<Result<Account, DomainError>> =
    fun () ->
      async {
        let! result =
          Http.getJson<AccountFull> context.ctx.http (Endpoints.accountById context.key.slug id)
        return (Result.map (Account.Full) >> DomainError.asDomain) result
      }

  let ofRow (context: BusinessContext) (row: AccountRow) : Account =
    Hydratable.Partial (row, fetch = mkFetch context (row.id.ToString()))

  let hydrate (acc: Account) : Async<Result<Account, DomainError>> =
    match acc with
    | Full _ -> async.Return (Ok acc)
    | Partial (_row, fetch) -> fetch ()

  let inline classify< ^Account when ^Account : (member ``type`` : Type92dEnum option) >
    (account: ^Account ) : AccountClass option =
    match account.``type`` with
    | Some Type92dEnum.ASS         -> Some Asset
    | Some Type92dEnum.ASS_DEP     -> Some Asset
    | Some Type92dEnum.ASS_VAT     -> Some Asset
    | Some Type92dEnum.ASS_REC     -> Some Asset
    | Some Type92dEnum.ASS_PAY     -> Some Asset
    | Some Type92dEnum.ASS_DUE     -> Some Asset
    | Some Type92dEnum.LIA         -> Some Liability
    | Some Type92dEnum.LIA_EQU     -> Some Liability
    | Some Type92dEnum.LIA_PRE     -> Some Liability
    | Some Type92dEnum.LIA_DUE     -> Some Liability
    | Some Type92dEnum.LIA_DEB     -> Some Liability
    | Some Type92dEnum.LIA_ACC     -> Some Liability
    | Some Type92dEnum.LIA_VAT     -> Some Liability
    | Some Type92dEnum.REV         -> Some Income
    | Some Type92dEnum.REV_SAL     -> Some Income
    | Some Type92dEnum.REV_NO      -> Some Income
    | Some Type92dEnum.EXP         -> Some Expense
    | Some Type92dEnum.EXP_DEP     -> Some Expense
    | Some Type92dEnum.EXP_NO      -> Some Expense
    | Some Type92dEnum.EXP_50      -> Some Expense
    | Some Type92dEnum.EXP_TAX     -> Some Expense
    | Some Type92dEnum.EXP_TAX_PRE -> Some Expense
    | None  -> None

  let diffAccount (full: AccountFull) (patched: AccountDelta) : Result<AccountCommand option, DomainError> =
    let id = patched.id
    if id <> full.id then
      Error (DomainError.Unexpected $"Patched account id {id} does not match hydrated account id {full.id}.")
    else
      let normalized = DeltaShape<AccountFull, AccountDelta>.Normalize(full, patched)
      if DeltaShape<AccountFull, AccountDelta>.HasChanges normalized then
        Ok (Some (AccountCommand.UpdateAccount (id, normalized)))
      else
        Ok None

  let deltasToCommands
    (accounts: AsyncSeq<Result<AccountFull, DomainError>>)
    (deltas: AsyncSeq<Result<AccountDelta, DomainError>>)
    : AsyncSeq<Result<AccountCommand, DomainError>> =

    Alignment.alignAccountsPermissive accounts deltas
    |> AsyncSeq.collect (function
        | Error err -> AsyncSeq.singleton (Error err)
        | Ok None -> AsyncSeq.empty // No matching account from API -> no command.
        | Ok (Some (account, delta)) ->
            match diffAccount account delta with
            | Ok (Some command) -> AsyncSeq.singleton (Ok command)
            | Ok None -> AsyncSeq.empty // No changes -> no command.
            | Error err -> AsyncSeq.singleton (Error err))

///
/// Document module operations
///

module Document =
  let ofRaw (raw: DocumentFull) : Document =
    Hydratable.Full raw

  let hydrate (doc: Document) : Async<Result<Document, DomainError>> =
    match doc with
    | Full _ -> async.Return (Ok doc)
    | Partial (_row, fetch) -> fetch ()

///
/// Streams module operations —— maybe to be folded to the previous modules
///

module Streams =

  let private mapHttpError (stream: AsyncSeq<Result<'T, Http.HttpError>>) : AsyncSeq<Result<'T, DomainError>> =
    stream
    |> AsyncSeq.map (function
        | Ok value -> Ok value
        | Error err -> Error (DomainError.Http err))

  /// Convert a raw stream into a domain stream
  let toDomain< 'Dom, 'Raw >
    (ofRaw: 'Raw -> 'Dom)
    (stream: AsyncSeq<Result<'Raw, HttpError>>)
    : AsyncSeq<Result<'Dom, DomainError>> =
    stream
    |> AsyncSeq.map (Result.map ofRaw >> DomainError.asDomain)

  /// Domain-level stream of businesses, yielding directly Full businesses
  let streamBusinesses (context: AccountingContext) : AsyncSeq<Result<Business, DomainError>> =
    Streams.streamPaginated<PaginatedBusinessList, NocfoApi.Types.Business>
       context.http
       (fun page -> Endpoints.businessList page)
    |> toDomain Business.ofRaw

  /// Domain-level stream of accounts for a given business, yielding lazy Partials
  /// that can be hydrated to Full on demand
  let streamAccounts (context: BusinessContext) : AsyncSeq<Result<Account, DomainError>> =
    Streams.streamPaginated<PaginatedAccountListList, NocfoApi.Types.AccountList>
       context.ctx.http
       (fun page -> Endpoints.accountsBySlugPage context.key.slug page)
    |> toDomain (Account.ofRow context)

  /// Domain-level stream of documents for a given business.
  let streamDocuments (context: BusinessContext) : AsyncSeq<Result<Document, DomainError>> =
    Streams.streamPaginated<PaginatedDocumentListList, DocumentFull>
       context.ctx.http
       (fun page -> Endpoints.documentsBySlugPage context.key.slug page)
    |> toDomain Document.ofRaw


  let hydrateAndUnwrap<'Full, 'Partial>
    (entity: AsyncSeq<Result<Hydratable<'Full, 'Partial>, DomainError>>)
    : AsyncSeq<Result<'Full, DomainError>> =
    entity
    |> AsyncSeq.mapAsync (fun result ->
      // TODO: replace with a version using higher-order functions
      async {
        match result with
        | Error e -> return Error e
        | Ok (Full full) -> return Ok full
        | Ok (Partial (_, fetch)) ->
            let! hydrated = fetch ()
            match hydrated with
            | Error e -> return Error e
            | Ok (Full full) -> return Ok full
            | Ok (Partial _) -> return Error (DomainError.Unexpected "Entity could not be hydrated")
      })

  let executeAccountCommands
    (context: BusinessContext)
    (commands: AsyncSeq<Result<AccountCommand, DomainError>>)
    : AsyncSeq<Result<AccountResult, DomainError>> =

    let patchPath (delta: AccountDelta) =
      Endpoints.accountById context.key.slug (string delta.id)

    let deletePath (accountId: int) =
      Endpoints.accountById context.key.slug (string accountId)

    let mapCommandToOperation (command: AccountCommand) =
      match command with
      | AccountCommand.UpdateAccount (_, delta) ->
          (fun () ->
                Http.patchJson<AccountDelta, AccountFull> context.ctx.http (patchPath delta) delta
                |> AsyncResult.map AccountUpdated)
      | AccountCommand.DeleteAccount id ->
          (fun () ->
                Http.deleteJson<unit> context.ctx.http (deletePath id)
                |> AsyncResult.map (fun () -> AccountDeleted id))
      | AccountCommand.CreateAccount _ ->
          raise (DomainStreamException (DomainError.Unexpected "CreateAccount is not supported yet."))

    asyncSeq {
      try
        yield!
          commands
          |> AsyncSeqResult.unwrapOrThrow
          |> AsyncSeq.map mapCommandToOperation
          |> NocfoClient.Streams.streamChanges (fun op -> op ())
          |> mapHttpError
      with
      | DomainStreamException err ->
          yield Error err
    }

///
/// BusinessResolver module operations
///

module BusinessResolver =
  /// Build candidate identifiers from a free-form CLI argument.
  /// Supports prefixes like "Y-tunnus" or "VAT-code"; without a prefix we try both.
  let private formIdentifierCandidates (input: string) : BusinessIdentifier list =
    let trimmed = input.Trim()

    [ BusinessIdentifier.Create(0, BusinessIdentifierTypeEnum.Y_tunnus, trimmed)
      BusinessIdentifier.Create(0, BusinessIdentifierTypeEnum.Vat_code, trimmed) ]

  let private identifiersOverlap (candidates: BusinessIdentifier list) (identifier: BusinessIdentifier) =
    candidates
    |> List.exists (fun candidate ->
         candidate.``type`` = identifier.``type`` &&
         String.Equals(candidate.value, identifier.value, StringComparison.OrdinalIgnoreCase))

  let private businessMatches candidates (full: BusinessFull) =
    full.raw.identifiers |> List.exists (identifiersOverlap candidates)

  /// Resolve a business identifier string to a BusinessContext.
  /// Streams businesses, hydrates them, filters by identifier match, and returns the first hit.
  let resolve (context: AccountingContext) (identifierString: string) : Async<Result<BusinessContext, DomainError>> =
    async {
      let candidates = formIdentifierCandidates identifierString
      let! firstMatch =
        Streams.streamBusinesses context
        |> Streams.hydrateAndUnwrap
        |> AsyncSeq.filter (businessMatches candidates)
        |> AsyncSeq.tryHead

      match firstMatch with
      | None ->            return Error (DomainError.Unexpected $"No matching business: {identifierString}")
      | Some (Error e) ->  return Error e
      | Some (Ok full) ->  return Ok { ctx = context; key = full.key }
    }
