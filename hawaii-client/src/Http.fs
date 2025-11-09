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

module Http =
    type HttpError = {
        statusCode: HttpStatusCode
        body: string
    }

    let createHttpContext (baseAddress: Uri) (token: string) =
        let client = new HttpClient()
        client.BaseAddress <- Uri(baseAddress.OriginalString.TrimEnd('/'))
        { client = client; token = token }

    let withAuth (httpContext: HttpContext) (request: HttpRequestMessage) =
        request.Headers.Authorization <- AuthenticationHeaderValue("Token", httpContext.token)
        request
    let withAcceptJson (request: HttpRequestMessage) =
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
        request

    let send (httpClient: HttpClient) (request: HttpRequestMessage) = async {
        if request.Headers.Accept.Count = 0 then
            ignore (withAcceptJson request)
        use! response = httpClient.SendAsync(request) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        if response.IsSuccessStatusCode then
            return Ok content
        else
            return Error { statusCode = response.StatusCode; body = content }
    }

    let getJson<'T> (httpContext: HttpContext) (absoluteUrl: string)= async {
        use req =
            new HttpRequestMessage(HttpMethod.Get, absoluteUrl)
            |> withAuth httpContext
            |> withAcceptJson
        let! result = send httpContext.client req
        match result with
        | Ok body ->
            let value = Serializer.deserialize<'T> body
            return Ok value
        | Error e ->
            return Error e
    }
