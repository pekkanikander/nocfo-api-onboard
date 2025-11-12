namespace Nocfo.Domain

open System
open FSharp.Control
open NocfoApi.Types
open NocfoClient
open NocfoClient.Http

/// Domain-level error channel (extend as needed)
type DomainError =
  | Http of Http.HttpError
  | Unexpected of string

/// Generic hydratable wrapper carrying a partial payload and a fetch that upgrades it
///
/// In this domain, we use Full and Partial only to represent the lifecycle.
/// Hence, we do not use RQA for them, even though LLMs suggest it.
type Hydratable<'Full,'Partial> =
  | Partial of partial: 'Partial * fetch: (unit -> Async<Result<Hydratable<'Full,'Partial>, DomainError>>)
  | Full    of full:   'Full

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

/// Business is now a hydratable of its full form with BusinessKey as the partial
type Business = Hydratable<BusinessFull, BusinessKey>

// At runtime we bind everything to a business context.
// This represents the repository, which at this point is just a HTTP client
// that allows us to fetch the business and its associated data.
type BusinessContext = {
  key:  BusinessKey
  http: HttpContext
}

type AccountClass = Asset | Liability | Equity | Income | Expense

type AccountClassTotals = Map<AccountClass, decimal>

/// Accounts are identified by their ID. Each account is associated with a business,
/// but we don't model that relationship yet, as we don't need it yet.
type AccountFull  = NocfoApi.Types.Account
type AccountRow   = NocfoApi.Types.AccountList

/// Account is a hydratable of its full form with AccountRow as the partial
type Account  = Hydratable<AccountFull, AccountRow>

///
/// Business module operations
///

module Business =
  let ofContext (context: BusinessContext) : Business =
    Hydratable.Partial (context.key, fetch = fun () -> async {
      let! result =
        Http.getJson<NocfoApi.Types.Business> context.http (Endpoints.businessBySlug context.key.slug)
      match result with
      | Result.Ok business ->
          let full : BusinessFull =
            { key  = context.key
              meta = { name = business.name; country = Option.ofObj business.country };
              raw  = business }
          return Ok (Hydratable.Full full)
      | Result.Error httpErr ->
          return Error (DomainError.Http httpErr)
    })

  let hydrate (business: Business) : Async<Result<Business, DomainError>> =
    match business with
    | Full _ -> async.Return (Ok business)
    | Partial (key, fetch) -> fetch ()

///
/// Account module operations
///

module Account =
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

///
/// Streams module operations —— maybe to be folded to the previous modules
///

module Streams =

  let private asDomain r = Result.mapError DomainError.Http r

  /// Domain-level stream of businesses, yielding directly Full businesses
  let streamBusinesses (http: HttpContext) : AsyncSeq<Result<Business, DomainError>> =
    let toDomain (business: NocfoApi.Types.Business) : Business =
      let slug = defaultArg business.slug "(none)" // XXX fixme
      Business.Full {
        // XXX fixme: what if there are no identifiers?
        key = { id = business.identifiers.[0]; slug = slug };
        meta = { name = business.name; country = Option.ofObj business.country }
        raw  = business
      }
    NocfoClient.Streams.streamBusinessesRaw http
    |> AsyncSeq.map (Result.map toDomain >> asDomain)

  /// Domain-level stream of accounts for a given businessSlug, yielding lazy Partials that can be hydrated to Full on demand
  let streamAccounts (context: BusinessContext) : AsyncSeq<Result<Account, DomainError>> =
    let toPartial (row: AccountRow) : Account =
      Account.Partial (row, fetch = fun () -> async {
        let! result =
          Http.getJson<AccountFull>
            context.http
            (Endpoints.accountById context.key.slug (row.id.ToString()))
        return (Result.map (Account.Full) >> asDomain) result
      })
    NocfoClient.Streams.streamAccountListsByBusinessSlug context.http context.key.slug
    |> AsyncSeq.map (Result.map toPartial >> asDomain)

module Reports =
  let addToTotals (totals: AccountClassTotals) (account: Account) : Async<Result<AccountClassTotals, DomainError>> =
    Account.hydrate account
    |> AsyncResult.bind (fun hydrated ->
      match hydrated with
      | Full full ->
        let cls = Account.classify full
        match cls with
        | Some cls -> Ok (Map.change cls (fun old -> Some ((defaultArg old 0M) + (decimal full.balance))) totals)
        | None     -> Error (DomainError.Unexpected "Account type is required")
      | Partial _  -> Error (DomainError.Unexpected "Account is not hydrated")
    )
