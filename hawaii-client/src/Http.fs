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
    timeout: TimeSpan
}

module Http =

    type HttpError =
        | Unauthorized of url: Uri
        | NotFound of url: Uri
        | RateLimited of url: Uri * retryAfter: int option
        | ServerError of url: Uri * statusCode: HttpStatusCode * body: string
        | ClientError of url: Uri * statusCode: HttpStatusCode * body: string
        | ParseError of url: Uri * message: string

    let ofHttpClient (client: HttpClient) (token: string) =
        { client = client; token = token; timeout = TimeSpan.FromSeconds 30.0 }

    let createHttpContext (baseAddress: Uri) (token: string) =
        let client = new HttpClient()
        let baseStr = baseAddress.OriginalString.TrimEnd '/'
        // Ensure we always have a /v1 prefix at the HttpClient level
        client.BaseAddress <- Uri(baseStr + "/v1")
        client.Timeout <- TimeSpan.FromSeconds 30.0
        ofHttpClient client token

    let withAuth (httpContext: HttpContext) (request: HttpRequestMessage) =
        request.Headers.Authorization <- AuthenticationHeaderValue("Token", httpContext.token)
        request

    let withAcceptJson (request: HttpRequestMessage) =
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue "application/json")
        request

    let private parseRetryAfter (response: HttpResponseMessage) =
        match response.Headers.RetryAfter with
        | null -> None
        | ra when ra.Delta.HasValue -> Some (int ra.Delta.Value.TotalSeconds)
        | ra when ra.Date.HasValue ->
            let seconds = int (ra.Date.Value - DateTimeOffset.UtcNow).TotalSeconds
            Some (max 0 seconds)
        | _ -> None

    let private classifyError (url: Uri) (response: HttpResponseMessage) (body: string) =
        match response.StatusCode with
        | HttpStatusCode.Unauthorized -> Unauthorized url
        | HttpStatusCode.NotFound -> NotFound url
        | HttpStatusCode.TooManyRequests -> RateLimited (url, parseRetryAfter response)
        | code when int code >= 500 -> ServerError (url, code, body)
        | code -> ClientError (url, code, body)

    let send (httpClient: HttpClient) (request: HttpRequestMessage) = async {
        if request.Headers.Accept.Count = 0 then
            ignore (withAcceptJson request)
        eprintfn "Sending request: %s %s" request.Method.Method request.RequestUri.OriginalString
        let! ct = Async.CancellationToken
        use! response = httpClient.SendAsync(request, ct) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        if response.IsSuccessStatusCode then
            return Ok content
        else
            let! requestBody =
                async {
                    if isNull request.Content then
                        return None
                    else
                        let! body = request.Content.ReadAsStringAsync() |> Async.AwaitTask
                        if String.IsNullOrWhiteSpace body then
                            return None
                        else
                            return Some body
                }
            let err = classifyError request.RequestUri response content
            eprintfn "Request failed: %s %s %s %s"
                request.Method.Method
                request.RequestUri.OriginalString
                (response.StatusCode.ToString())
                (content.Substring(0, min 100 content.Length))
            match requestBody with
            | Some body -> eprintfn "Request payload: %s" body
            | None -> ()
            return Error err
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
                    Error (ParseError (url, ex.Message))
        | Error e -> Error e

    let private shouldRetry (err: HttpError) =
        match err with
        | RateLimited _ | ServerError _ -> true
        | _ -> false

    let private retryDelay (attempt: int) (baseDelay: TimeSpan) (err: HttpError) =
        let retryAfterMs =
            match err with
            | RateLimited (_, Some seconds) -> seconds * 1000
            | _ -> 0
        let backoffMs = int baseDelay.TotalMilliseconds * pown 2 attempt
        let jitterMs = Random.Shared.Next(0, int baseDelay.TotalMilliseconds)
        TimeSpan.FromMilliseconds(float (max retryAfterMs (backoffMs + jitterMs)))

    let private retry
        (maxAttempts: int)
        (baseDelay: TimeSpan)
        (action: unit -> Async<Result<'T, HttpError>>)
        : Async<Result<'T, HttpError>> =
        let rec loop attempt = async {
            let! result = action ()
            match result with
            | Error err when shouldRetry err && attempt < maxAttempts - 1 ->
                let delay = retryDelay attempt baseDelay err
                eprintfn "Retrying in %.1fs (attempt %d/%d)..." delay.TotalSeconds (attempt + 1) maxAttempts
                do! Async.Sleep(int delay.TotalMilliseconds)
                return! loop (attempt + 1)
            | other -> return other
        }
        loop 0

    let private sendJson<'Response>
        (httpContext: HttpContext)
        (method: HttpMethod)
        (path: string)
        (configure: HttpRequestMessage -> HttpRequestMessage)
        =
        let absoluteUrl = Uri(httpContext.client.BaseAddress.OriginalString + path)
        retry 3 (TimeSpan.FromSeconds 1.0) (fun () -> async {
            use request =
                new HttpRequestMessage(method, absoluteUrl)
                |> withAuth httpContext
                |> withAcceptJson
                |> configure
            let! result = send httpContext.client request
            return deserialize<'Response> absoluteUrl result
        })

    let private withJsonContent (payload: 'T) (req: HttpRequestMessage) =
        let json = Serializer.serialize payload
        req.Content <- new StringContent(json, Text.Encoding.UTF8, "application/json")
        req

    let getJson<'T> (httpContext: HttpContext) (path: string) =
        sendJson<'T> httpContext HttpMethod.Get path id

    /// PATCH with JSON payload and decode a JSON response body.
    /// Use when the API returns the updated resource (e.g., 200 + body).
    let patchJson<'Payload, 'Response> (httpContext: HttpContext) (path: string) (payload: 'Payload) =
        sendJson<'Response> httpContext HttpMethod.Patch path (withJsonContent payload)

    /// POST with JSON payload and decode a JSON response body.
    let postJson<'Payload, 'Response> (httpContext: HttpContext) (path: string) (payload: 'Payload) =
        sendJson<'Response> httpContext HttpMethod.Post path (withJsonContent payload)

    /// DELETE with JSON response body (e.g., when API returns a payload on delete success)
    let deleteJson<'Response> (httpContext: HttpContext) (path: string) =
        sendJson<'Response> httpContext HttpMethod.Delete path id
