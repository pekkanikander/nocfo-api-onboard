#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "bin/Debug/net10.0/hawaii-client.dll"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"


open System

/// Base URL for the NOCFO API; defaults to test if not provided
let baseUrl =
    match Environment.GetEnvironmentVariable("NOCFO_BASE_URL") with
    | null | "" -> Uri("https://api-tst.nocfo.io")
    | v -> Uri(v)

/// Bearer token from environment; exits if missing
let token =
    match Environment.GetEnvironmentVariable("NOCFO_TOKEN") with
    | null | "" ->
        eprintfn "Error: NOCFO_TOKEN environment variable not set"
        eprintfn "Get a token: https://login-tst.nocfo.io/auth/tokens/"
        exit 1
    | t -> t

let context = NocfoClient.Http.createHttpContext baseUrl token
let accounting = Nocfo.Domain.Accounting.ofHttp context
