namespace Nocfo.Domain

open System
open System.Reflection
open FSharp.Control
open Microsoft.FSharp.Reflection
open NocfoApi.Types
open NocfoClient
open NocfoClient.Http

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

  let private isOptionType (t: Type) =
    t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

  // None is the first case of the option type, hence Array.head is safe
  let private makeNoneValue (optionType: Type) =
    let noneCase = FSharpType.GetUnionCases optionType |> Array.head
    FSharpValue.MakeUnion(noneCase, [||])

  let private tryOptionalValue (optionType: Type) (value: obj) =
    let caseInfo, fields = FSharpValue.GetUnionFields(value, optionType)
    if caseInfo.Name = "Some" then Some fields.[0] else None

  let private tryOptionValue (optionType: Type) (value: obj) =
    if isOptionType optionType then
      tryOptionalValue optionType value
    else
      None

  let private requireProperty<'T> (name: string) : PropertyInfo =
    match typeof<'T>.GetProperty(name) with
    | null -> failwithf "%s is missing property '%s'" typeof<'T>.Name name
    | prop -> prop

  let private propertyMatches<'Full> (full: 'Full) (fieldName: string) (desired: obj) =
    let prop = requireProperty<AccountFull> fieldName
    let cval = prop.GetValue(full) // Current value at the server side
    if isOptionType prop.PropertyType then
      match tryOptionalValue prop.PropertyType cval with
      | Some existing -> existing.Equals(desired)
      | None -> false
    else
      cval.Equals(desired)

  let private normalizeDelta< 'Full, 'Delta > (full: 'Full) (delta: 'Delta) =
    let recordType  = typeof< 'Delta >
    let fields      = FSharpType.GetRecordFields(recordType)
    let constructor = FSharpValue.PreComputeRecordConstructor(recordType)

    let normalizeField (field: PropertyInfo) =
      let original = field.GetValue(delta)
      if not (isOptionType field.PropertyType) then
        original
      else
        match tryOptionalValue field.PropertyType original with
        | None -> original
        | Some desired ->
            if propertyMatches< 'Full > full field.Name desired then
              makeNoneValue field.PropertyType
            else
              original

    fields
    |> Array.map normalizeField
    |> constructor
    :?> AccountDelta

  let private deltaHasChanges (delta: AccountDelta) =
    let recordType = typeof<AccountDelta>
    FSharpType.GetRecordFields(recordType)
    |> Array.exists (fun field ->
        field.Name <> "id"
        && isOptionType field.PropertyType
        && (field.GetValue(delta)
            |> tryOptionValue field.PropertyType
            |> Option.isSome))

  let diffAccount (full: AccountFull) (patched: AccountDelta) : Result<AccountCommand option, DomainError> =
    let id = patched.id
    if id <> full.id then
      Error (DomainError.Unexpected $"Patched account id {id} does not match hydrated account id {full.id}.")
    else
      let normalized = normalizeDelta full patched
      if deltaHasChanges normalized then
        Ok (Some (AccountCommand.UpdateAccount (id, normalized)))
      else
        Ok None

///
/// Streams module operations —— maybe to be folded to the previous modules
///

module Streams =

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

  /// Domain-level stream of accounts for a given business, yielding lazy Partials that can be hydrated to Full on demand
  let streamAccounts (context: BusinessContext) : AsyncSeq<Result<Account, DomainError>> =
    Streams.streamPaginated<PaginatedAccountListList, NocfoApi.Types.AccountList>
       context.ctx.http
       (fun page -> Endpoints.accountsBySlugPage context.key.slug page)
    |> toDomain (Account.ofRow context)


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

///
/// BusinessResolver module operations
///

module BusinessResolver =
  /// Build candidate identifiers from a free-form CLI argument.
  /// Supports prefixes like "Y-tunnus" or "VAT-code"; without a prefix we try both.
  let private formIdentifierCandidates (input: string) : BusinessIdentifier list =
    let trimmed = input.Trim()

    [ BusinessIdentifier.Create(BusinessIdentifierTypeEnum.Y_tunnus, trimmed)
      BusinessIdentifier.Create(BusinessIdentifierTypeEnum.Vat_code, trimmed) ]

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
