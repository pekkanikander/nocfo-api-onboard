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
        url: Uri
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
        eprintfn "Sending request: %s %s" request.Method.Method request.RequestUri.OriginalString
        use! response = httpClient.SendAsync(request) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        if response.IsSuccessStatusCode then
            return Ok content
        else
            eprintfn "Request failed: %s %s %s %s"
                request.Method.Method
                request.RequestUri.OriginalString
                (response.StatusCode.ToString())
                (content.Substring(0, min 100 content.Length))
            return Error {
                url = request.RequestUri
                statusCode = response.StatusCode
                body = content
            }
    }

    let private deserialize<'T> (url: Uri) (result: Result<string, HttpError>) =
        match result with
        | Ok body ->
            if typeof<'T> = typeof<unit> then
                Ok (Unchecked.defaultof<'T>) // ()
            else
                try
                    let value = Serializer.deserialize<'T> body
                    Ok value
                with ex ->
                    Error {
                        url = url
                        statusCode = enum<HttpStatusCode>(0)
                        body = $"Decode error: {ex.Message}\nBody: {body}"
                    }
        | Error e -> Error e

    let private sendJson<'Response>
        (httpContext: HttpContext)
        (method: HttpMethod)
        (path: string)
        (configure: HttpRequestMessage -> HttpRequestMessage)
        =
        async {
            let absoluteUrl = Uri(httpContext.client.BaseAddress.OriginalString + path)
            use request =
                new HttpRequestMessage(method, absoluteUrl)
                |> withAuth httpContext
                |> withAcceptJson
                |> configure
            let! result = send httpContext.client request
            return deserialize<'Response> absoluteUrl result
        }

    let private withJsonContent (payload: 'T) (req: HttpRequestMessage) =
        let json = Serializer.serialize payload
        req.Content <- new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        req

    let getJson<'T> (httpContext: HttpContext) (path: string) =
        sendJson<'T> httpContext HttpMethod.Get path id

    /// PATCH with JSON payload and decode a JSON response body.
    /// Use when the API returns the updated resource (e.g., 200 + body).
    let patchJson<'Payload, 'Response> (httpContext: HttpContext) (path: string) (payload: 'Payload) =
        sendJson<'Response> httpContext HttpMethod.Patch path (withJsonContent payload)

    /// DELETE with JSON response body (e.g., when API returns a payload on delete success)
    let deleteJson<'Response> (httpContext: HttpContext) (path: string) =
        sendJson<'Response> httpContext HttpMethod.Delete path id
