#!/usr/bin/env dotnet fsi

// Raw HTTP test (no Hawaii code)
// Usage:
//   export NOCFO_TOKEN="your_token"
//   dotnet fsi RawHttpTest.fsx

open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Threading.Tasks

let baseUrl = "https://api-tst.nocfo.io"

let token =
    match Environment.GetEnvironmentVariable("NOCFO_TOKEN") with
    | null | "" ->
        eprintfn "Error: NOCFO_TOKEN not set"
        exit 1
    | t -> t

let authValue = sprintf "Token %s" token
printfn "Base URL: %s" baseUrl
printfn "Token preview: %s***" (authValue.Substring(0, min 15 authValue.Length))

let printResponse (resp: HttpResponseMessage) (body: string) =
    printfn "\nStatus: %A %s" resp.StatusCode resp.ReasonPhrase
    printfn "Response headers:"
    resp.Headers |> Seq.iter (fun kv -> printfn "  %s: %s" kv.Key (String.Join(", ", kv.Value)))
    if not (isNull resp.Content) then
        resp.Content.Headers |> Seq.iter (fun kv -> printfn "  %s: %s" kv.Key (String.Join(", ", kv.Value)))
    printfn "\nBody (first 800 chars):\n%s" (if body.Length > 800 then body.Substring(0,800) else body)

let absoluteUrl = baseUrl.TrimEnd('/') + "/v1/business/?page_size=2&page=1"

// Variant A: DefaultRequestHeaders on HttpClient + GetAsync(absolute URL)
let runVariantA () = task {
    printfn "\n=== Variant A: DefaultRequestHeaders + GetAsync(absolute URL) ==="
    use client = new HttpClient()
    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
    client.DefaultRequestHeaders.Remove("Authorization") |> ignore
    let added = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authValue)
    printfn "Added Authorization to DefaultRequestHeaders: %b" added
    printfn "Sending GET %s" absoluteUrl
    use! resp = client.GetAsync(absoluteUrl)
    let! body = resp.Content.ReadAsStringAsync()
    printResponse resp body
}

// Variant B: Explicit HttpRequestMessage with Authorization header
let runVariantB () = task {
    printfn "\n=== Variant B: HttpRequestMessage + explicit Authorization header ==="
    use client = new HttpClient()
    use req = new HttpRequestMessage(HttpMethod.Get, absoluteUrl)
    req.Headers.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
    ignore (req.Headers.TryAddWithoutValidation("Authorization", authValue))
    printfn "Request headers to be sent:"
    req.Headers |> Seq.iter (fun kv -> printfn "  %s: %s" kv.Key (String.Join(", ", kv.Value)))
    use! resp = client.SendAsync(req)
    let! body = resp.Content.ReadAsStringAsync()
    printResponse resp body
}

// Variant C: BaseAddress + relative path + DefaultRequestHeaders
let runVariantC () = task {
    printfn "\n=== Variant C: BaseAddress + relative path + DefaultRequestHeaders ==="
    use client = new HttpClient()
    client.BaseAddress <- Uri(baseUrl)
    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
    client.DefaultRequestHeaders.Remove("Authorization") |> ignore
    let added = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authValue)
    printfn "Added Authorization to DefaultRequestHeaders: %b" added
    let relative = "/v1/business/?page_size=2&page=1"
    printfn "Sending GET %s%s" baseUrl relative
    use! resp = client.GetAsync(relative)
    let! body = resp.Content.ReadAsStringAsync()
    printResponse resp body
}

// Run all variants sequentially
let main = task {
    do! runVariantA ()
    do! runVariantB ()
    do! runVariantC ()
}

main.GetAwaiter().GetResult()
