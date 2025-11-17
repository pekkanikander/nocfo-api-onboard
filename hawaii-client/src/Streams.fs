namespace NocfoClient

open System
open System.Net.Http
open FSharp.Control
open NocfoApi.Types
open NocfoClient
open NocfoClient.Http
open NocfoClient.AsyncSeqHelpers

module Streams =
    let inline streamPaginated< ^Page, 'Item
        when ^Page : (member results : 'Item list)
         and ^Page : (member next    : string option) >
        (http: HttpContext)
        (relativeForPage: int -> string)
        : AsyncSeq<Result<'Item, HttpError>> =

        let fetchPage (page: int) : Async<Result< ^Page , HttpError>> = async {
            let! result = Http.getJson< ^Page > http (relativeForPage page)
            return result
        }
        paginateByPageSRTP fetchPage
