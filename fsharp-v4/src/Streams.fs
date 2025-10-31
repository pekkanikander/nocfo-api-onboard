module Streams

open System
open System.Net.Http
open System.Threading
open FSharp.Control

/// Returns an AsyncSeq of AccountList items for a given business slug.
/// apiToken: NOCFO API token (Authorization: Token <token>)
/// businessSlug: slug of the business to query
let getAccountsStream (apiToken: string) (businessSlug: string) : AsyncSeq<Nocfo.Generated.AccountList> =
    // Single HttpClient instance for the whole stream
    let httpClient = new HttpClient()
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {apiToken}")

    let client = new Nocfo.Generated.NocfoClient(httpClient)

    let rec loop (page: int) : AsyncSeq<Nocfo.Generated.AccountList> = asyncSeq {
        // Call the C# client with pagination
        // We only pass page and page_size - all other params are null/not used
        let! resp =
            client.Accounts__ListAsync(
                business_slug = businessSlug,
                is_shown = System.Nullable<bool>(),
                is_used = System.Nullable<bool>(),
                page = System.Nullable<int>(page),
                page_size = System.Nullable<int>(100),
                search = null,
                tags = System.Nullable<double>(),
                type = null,
                cancellationToken = CancellationToken.None
            )
            |> Async.AwaitTask

        for acc in resp.Results do
            yield acc

        match resp.Next with
        | null -> ()
        | _ -> yield! loop (page + 1)
    }

    loop 1
