namespace NocfoClient

open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open FSharp.Control
open NocfoApi.Http

type HttpContext = {
    client: HttpClient
    token: string
}

// TODO: Add a dispose method to the HttpContext to dispose the HttpClient.
// TODO: Add timeount and cancellation handling
// TODO: Consider adding a retry policy.
// TODO: Consider adding a logging policy.
// TODO: Consider adding a caching policy.

module Http =
    type HttpError = {
        statusCode: HttpStatusCode
        body: string
    }

    let ofHttpClient (client: HttpClient) (token: string) =
        { client = client; token = token }

    let createHttpContext (baseAddress: Uri) (token: string) =
        let client = new HttpClient()
        let baseStr = baseAddress.OriginalString.TrimEnd('/')
        // Ensure we always have a /v1 prefix at the HttpClient level
        client.BaseAddress <- Uri(baseStr + "/v1")
        ofHttpClient client token

    let withAuth (httpContext: HttpContext) (request: HttpRequestMessage) =
        request.Headers.Authorization <- AuthenticationHeaderValue("Token", httpContext.token)
        request

    let withAcceptJson (request: HttpRequestMessage) =
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
        request

    let send (httpClient: HttpClient) (request: HttpRequestMessage) = async {
        if request.Headers.Accept.Count = 0 then
            ignore (withAcceptJson request)
        printfn "Sending request: %s %s" request.Method.Method request.RequestUri.OriginalString
        use! response = httpClient.SendAsync(request) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        if response.IsSuccessStatusCode then
            return Ok content
        else
            return Error { statusCode = response.StatusCode; body = content }
    }

    let getJson<'T> (httpContext: HttpContext) (path: string)= async {
        let absoluteUrl = Uri(httpContext.client.BaseAddress.OriginalString + path)
        use req =
            new HttpRequestMessage(HttpMethod.Get, absoluteUrl)
            |> withAuth httpContext
            |> withAcceptJson
        let! result = send httpContext.client req
        match result with
        | Ok body ->
            // Serializer.deserialize exceptions, untyped faults.
            // TODO: Wrap deserialisation in try,
            // return Error (HttpError{ statusCode = 0; body = "...decode error..." })
            // or a dedicated DecodeError in a domain error DU.
            let value = Serializer.deserialize<'T> body
            return Ok value
        | Error e ->
            return Error e
    }
