namespace Nocfo.Tools

open NocfoClient
open Nocfo.Domain
open System

/// Resolved configuration shared by all CLI commands.
type ToolConfig =
    {
        BaseUrl: Uri
        Token: string
    }

/// Shared runtime context derived from configuration.
type ToolContext =
    {
        Config: ToolConfig
        Accounting: AccountingContext
    }

/// Errors that can occur while materialising `ToolConfig`.
type ToolConfigError =
    | MissingEnvironmentVariable of string
    | InvalidUri of envVar: string * value: string

[<RequireQualifiedAccess>]
module Runtime =

    [<Literal>]
    let private DefaultBaseUrl = "https://api-tst.nocfo.io"
    [<Literal>]
    let private TokenVar = "NOCFO_TOKEN"
    [<Literal>]
    let private BaseUrlVar = "NOCFO_BASE_URL"

    let private tryEnv name =
        match Environment.GetEnvironmentVariable(name) with
        | null  -> None
        | ""    -> None
        | value -> Some value

    [<RequireQualifiedAccess>]
    module ToolConfig =

        let describeError =
            function
            | MissingEnvironmentVariable name ->
                $"Missing required environment variable {name}."
            | InvalidUri (name, value) ->
                $"Environment variable {name} must be an absolute URI (value: {value})."

        let createContext (cfg: ToolConfig) : ToolContext =
            let httpContext = Http.createHttpContext cfg.BaseUrl cfg.Token
            let accounting = Accounting.ofHttp httpContext
            { Config = cfg; Accounting = accounting }

        let fromEnvironment () =
            let baseUrlResult =
                match tryEnv BaseUrlVar with
                | None -> Ok (Uri DefaultBaseUrl)
                | Some value ->
                    match Uri.TryCreate(value, UriKind.Absolute) with
                    | true, uri -> Ok uri
                    | false, _ -> Error (InvalidUri (BaseUrlVar, value))

            let tokenResult =
                match tryEnv TokenVar with
                | Some token -> Ok token
                | None -> Error (MissingEnvironmentVariable TokenVar)

            match baseUrlResult, tokenResult with
            | Ok baseUrl, Ok token ->
                Ok { BaseUrl = baseUrl; Token = token }
            | _ ->
                let errors =
                    [
                        match baseUrlResult with
                        | Error e -> yield e
                        | _ -> ()
                        match tokenResult with
                        | Error e -> yield e
                        | _ -> ()
                    ]
                Error errors

        let loadOrFail (): ToolContext =
            match fromEnvironment () with
            | Ok cfg -> createContext cfg
            | Error errors ->
                let errorMessages = errors |> List.map describeError |> String.concat "\n"
                failwith $"Tool configuration failed: {errorMessages}"
