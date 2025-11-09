namespace NocfoClient

open System
open System.Net.Http
open FSharp.Control
open NocfoClient
open NocfoClient.Http
open NocfoClient.AsyncSeqHelpers
open NocfoApi.Types

module Streams =
    let inline streamPaginated< ^Page, 'Item
        when ^Page : (member results : 'Item list)
         and ^Page : (member next    : string option) >
        (http: HttpContext)
        (label: string)
        (relativeForPage: int -> string)
        : AsyncSeq<'Item> =

        let fetchPage (page: int) : Async< ^Page > = async {
            let url     = http.client.BaseAddress.OriginalString + relativeForPage page
            let! result = Http.getJson< ^Page > http url
            match result with
            | Ok payload -> return payload
            | Error e ->
                let msg = sprintf "HTTP %A while fetching %s page %d: %s" e.statusCode label page e.body
                return (raise (System.Exception msg) : ^Page)
        }
        paginateByPageSRTP fetchPage

    let streamBusinesses (http: HttpContext) : AsyncSeq<Business> =
        streamPaginated<PaginatedBusinessList, Business>
            http
            "businesses"
            (fun page -> $"/v1/business/?page_size=100&page={page}")

    let streamAccountListsByBusinessSlug (http: HttpContext) (businessSlug: string) : AsyncSeq<AccountList> =
        streamPaginated<PaginatedAccountListList, AccountList>
            http
            "accounts"
            (fun page -> $"/v1/business/{businessSlug}/account/?page_size=100&page={page}")
