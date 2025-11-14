#!/usr/bin/env dotnet fsi

// Clean, minimal test of NOCFO API using raw HttpClient for requests
// and the generated Types + Serializer for deserialization
//
// Usage:
//   export NOCFO_TOKEN="your_token"
//   # optional: export NOCFO_BASE_URL="https://api-prd.nocfo.io"
//   dotnet fsi TestClient.fsx

#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"

#load "generated/StringEnum.fs"
#load "generated/OpenApiHttp.fs"    // for Serializer
#load "generated/Types.fs"

#load "TestSupport.fsx"
open TestSupport

open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open NocfoApi.Types
open NocfoApi.Http

let authValue = sprintf "Token %s" token

let safeList (xs: 'a list) = if isNull (box xs) then [] else xs

let sendGet (absoluteUrl: string) =
    task {
        use client = new HttpClient()
        use req = new HttpRequestMessage(HttpMethod.Get, absoluteUrl)
        req.Headers.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
        ignore (req.Headers.TryAddWithoutValidation("Authorization", authValue))
        use! resp = client.SendAsync(req)
        let! body = if isNull resp.Content then task { return "" } else resp.Content.ReadAsStringAsync()
        return resp, body
    }

let printSummary (title: string) (resp: HttpResponseMessage) (body: string) =
    printfn "\n=== %s ===" title
    printfn "Status: %A %s" resp.StatusCode resp.ReasonPhrase
    printfn "Body (first 5000 chars):\n%s" (if body.Length > 5000 then body.Substring(0, 5000) else body)

let testBusinesses () =
    task {
        let url = baseUrl.OriginalString.TrimEnd('/') + "/v1/business/?page=1&page_size=5"
        let! (resp, body) = sendGet url
        printSummary "Businesses" resp body

        if resp.StatusCode <> HttpStatusCode.OK then
            return Error (sprintf "HTTP %A" resp.StatusCode)
        else
            let page = Serializer.deserialize<PaginatedBusinessList> body
            printfn "\nCount: %d" page.count
            printfn "Next: %s" (page.next |> Option.defaultValue "none")
            printfn "Prev: %s" (page.prev |> Option.defaultValue "none")

            let results = safeList page.results
            printfn "Results on this page: %d" results.Length
            for i, b in results |> List.indexed do
                printfn "- [%d] id=%d name=%s slug=%s" i b.id b.name (b.slug |> Option.defaultValue "(none)")

            return Ok page
    }

let testAccounts (businessSlug: string) =
    task {
        let url = baseUrl.OriginalString.TrimEnd('/') + $"/v1/business/{businessSlug}/account/?page=1&page_size=5"
        let! (resp, body) = sendGet url
        printSummary "Accounts" resp body

        if resp.StatusCode <> HttpStatusCode.OK then
            return Error (sprintf "HTTP %A" resp.StatusCode)
        else
            let page = Serializer.deserialize<PaginatedAccountListList> body
            printfn "\nCount: %d" page.count
            let results = safeList page.results
            printfn "Results on this page: %d" results.Length
            for i, a in results |> List.indexed do
                printfn "- [%d] id=%d number=%s" i a.id a.number
            return Ok page
    }

let main = task {
    printfn "Base URL: %s" baseUrl.OriginalString
    printfn "Token preview: %s***" (authValue.Substring(0, min 12 authValue.Length))

    let! biz = testBusinesses ()
    match biz with
    | Ok page ->
        let results = safeList page.results
        match results with
        | b :: _ ->
            match b.slug with
            | Some slug ->
                let! _ = testAccounts slug
                return ()
            | None ->
                printfn "\nNo slug on first business; skipping accounts test"
                return ()
        | [] ->
            printfn "\nNo businesses returned"
            return ()
    | Error e ->
        printfn "\nBusinesses test failed: %s" e
        return ()
}

main.GetAwaiter().GetResult()
