namespace Nocfo.Domain

open System
open System.Net.Http
open FSharp.Control
open NocfoApi.Types
open NocfoClient
open NocfoClient.Http

// Abbreviations: zero-cost renames
type AccountFull  = NocfoApi.Types.Account
type AccountRow   = NocfoApi.Types.AccountList

type Account =
  | Partial of partial: AccountRow * fetch: (unit -> Async<Account>)
  | Full    of full: AccountFull

module Account =
  let id = function
    | Partial (p, _) -> p.id
    | Full f         -> f.id

  /// Upgrade to Full (no-op if already Full)
  let hydrate acc =
    match acc with
    | Full _ -> async.Return acc
    | Partial (_, fetch) -> fetch ()

module Streams =
  /// Domain-level stream of accounts for a given businessSlug, yielding lazy Partials that can be hydrated to Full on demand
  let streamAccountsByBusinessSlug (http: HttpClient) (baseUrl: Uri) (token: string) (businessSlug: string) : AsyncSeq<Account> =
    NocfoClient.Streams.streamAccountListsByBusinessSlug http baseUrl token businessSlug
    |> AsyncSeq.map (fun (row: AccountRow) ->
        let fetch () = async {
          let url = baseUrl.OriginalString + $"/v1/business/{businessSlug}/account/{row.id}/"
          let! r = Http.getJson<AccountFull> http url (Http.withAuth token)
          match r with
          | Ok full -> return Account.Full full
          | Error e -> return (failwithf "HTTP %A while fetching account %d: %s" e.statusCode row.id e.body)
        }
        Account.Partial (row, fetch)
    )
