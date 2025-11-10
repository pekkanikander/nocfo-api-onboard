namespace Nocfo.Domain

open System
open FSharp.Control
open NocfoClient
open NocfoClient.Endpoints
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
type Business =
  | Partial of key:   BusinessKey
  | Full    of full:  BusinessFull
  | Error   of error: Http.HttpError

// At runtime we bind everything to a business context.
// This represents the repository, which at this point is just a HTTP client
// that allows us to fetch the business and its associated data.
type BusinessContext = {
  key:  BusinessKey
  http: HttpContext
}

module Business =
  let hydrate (context: BusinessContext) = async {
    let! result =
      Http.getJson<NocfoApi.Types.Business> context.http (Endpoints.businessBySlug context.key.slug)
    match result with
    | Result.Ok business ->
      return Business.Full {
        key = context.key
        meta = {
          name = business.name
          country = Option.ofObj business.country
        }
        raw = business
      }
    | Result.Error error ->
      return Business.Error error
  }

/// Accounts are identified by their ID. Each account is associated with a business,
/// but we don't model that relationship yet, as we don't need it yet.
type AccountFull  = NocfoApi.Types.Account
type AccountRow   = NocfoApi.Types.AccountList

type Account =
  | Partial of partial: AccountRow * fetch: (unit -> Async<Account>)
  | Full    of full:    AccountFull
  | Error   of error:   Http.HttpError

module Account =
  let hydrate acc =
    match acc with
    | Full _             -> async.Return acc
    | Partial (_, fetch) -> fetch ()
    | Error e            -> async.Return (Account.Error e)

module Streams =
  /// Domain-level stream of accounts for a given businessSlug, yielding lazy Partials that can be hydrated to Full on demand
  let streamAccounts (context: BusinessContext) : AsyncSeq<Account> =
    NocfoClient.Streams.streamAccountListsByBusinessSlug context.http context.key.slug
    |> AsyncSeq.map (fun (row: AccountRow) ->
    Account.Partial (row, fetch = fun () -> async {
        let! result =
          Http.getJson<AccountFull>
            context.http
            (Endpoints.accountById
            context.key.slug
            (row.id.ToString()))
        match result with
        | Result.Ok full -> return Account.Full full
        | Result.Error error -> return Account.Error error
      })
    )
