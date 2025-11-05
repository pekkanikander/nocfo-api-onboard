namespace NocfoClient

open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open NocfoApi.Http

module Http =
    type HttpError = {
        statusCode: HttpStatusCode
        body: string
    }

    let createHttpClient (baseAddress: Uri) =
        let client = new HttpClient()
        client.BaseAddress <- baseAddress
        client

    let withAcceptJson (request: HttpRequestMessage) =
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
        request

    let withAuth (token: string) (request: HttpRequestMessage) =
        request.Headers.Authorization <- AuthenticationHeaderValue("Token", token)
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

    let getJson<'T> (httpClient: HttpClient) (absoluteUrl: string) (configure: HttpRequestMessage -> HttpRequestMessage) = async {
        use req =
            new HttpRequestMessage(HttpMethod.Get, absoluteUrl)
            |> withAcceptJson
            |> configure
        let! result = send httpClient req
        match result with
        | Ok body ->
            let value = Serializer.deserialize<'T> body
            return Ok value
        | Error e ->
            return Error e
    }
