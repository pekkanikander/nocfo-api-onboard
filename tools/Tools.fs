namespace Nocfo.Tools

open NocfoClient
open Nocfo.Domain
open System
open System.IO


/// Resolved configuration shared by all CLI commands.
type ToolConfig =
    {
        BaseUrl: Uri
        Token: string
        SourceToken: string option
        SourceBaseUrl: Uri
    }

/// Shared runtime context derived from configuration.
type ToolContext =
    {
        Config: ToolConfig
        Accounting: AccountingContext
        Input: TextReader
        Output: TextWriter
    }

/// Errors that can occur while materialising `ToolConfig`.
type ToolConfigError =
    | MissingEnvironmentVariable of string
    | InvalidUri of envVar: string * value: string

module Runtime =

    [<Literal>]
    let private DefaultBaseUrl = "https://api-tst.nocfo.io"
    [<Literal>]
    let private DefaultSourceBaseUrl = "https://api-prd.nocfo.io"
    [<Literal>]
    let private TokenVar = "NOCFO_TOKEN"
    [<Literal>]
    let private TargetTokenVar = "NOCFO_TARGET_TOKEN"
    [<Literal>]
    let private SourceTokenVar = "NOCFO_SOURCE_TOKEN"
    [<Literal>]
    let private BaseUrlVar = "NOCFO_BASE_URL"
    [<Literal>]
    let private TargetBaseUrlVar = "NOCFO_TARGET_BASE_URL"
    [<Literal>]
    let private SourceBaseUrlVar = "NOCFO_SOURCE_BASE_URL"

    let private tryEnv name =
        match Environment.GetEnvironmentVariable(name) with
        | null  -> None
        | ""    -> None
        | value -> Some value

    module ToolConfig =

        let describeError =
            function
            | MissingEnvironmentVariable name ->
                $"Missing required environment variable {name}."
            | InvalidUri (name, value) ->
                $"Environment variable {name} must be an absolute URI (value: {value})."

        let createContext (cfg: ToolConfig) (input: TextReader) (output: TextWriter) : ToolContext =
            let httpContext = Http.createHttpContext cfg.BaseUrl cfg.Token
            let accounting = Accounting.ofHttp httpContext
            { Config = cfg; Accounting = accounting; Input = input; Output = output }

        let fromEnvironment () =
            let parseBaseUrl envVar defaultValue =
                match tryEnv envVar with
                | None -> Ok (Uri defaultValue)
                | Some value ->
                    match Uri.TryCreate(value, UriKind.Absolute) with
                    | true, uri -> Ok uri
                    | false, _ -> Error (InvalidUri (envVar, value))

            let targetBaseUrlResult =
                match tryEnv TargetBaseUrlVar with
                | Some value ->
                    match Uri.TryCreate(value, UriKind.Absolute) with
                    | true, uri -> Ok uri
                    | false, _ -> Error (InvalidUri (TargetBaseUrlVar, value))
                | None -> parseBaseUrl BaseUrlVar DefaultBaseUrl

            let sourceBaseUrlResult =
                match tryEnv SourceBaseUrlVar with
                | None -> Ok (Uri DefaultSourceBaseUrl)
                | Some value ->
                    match Uri.TryCreate(value, UriKind.Absolute) with
                    | true, uri -> Ok uri
                    | false, _ -> Error (InvalidUri (SourceBaseUrlVar, value))

            let targetTokenResult =
                match tryEnv TargetTokenVar with
                | Some token -> Ok token
                | None ->
                    match tryEnv TokenVar with
                    | Some token -> Ok token
                    | None -> Error (MissingEnvironmentVariable $"{TargetTokenVar} (fallback: {TokenVar})")

            let sourceToken = tryEnv SourceTokenVar

            match targetBaseUrlResult, targetTokenResult, sourceBaseUrlResult with
            | Ok baseUrl, Ok token, Ok sourceBaseUrl ->
                Ok { BaseUrl = baseUrl; Token = token; SourceToken = sourceToken; SourceBaseUrl = sourceBaseUrl }
            | _ ->
                let errors =
                    [
                        match targetBaseUrlResult with
                        | Error e -> yield e
                        | _ -> ()
                        match targetTokenResult with
                        | Error e -> yield e
                        | _ -> ()
                        match sourceBaseUrlResult with
                        | Error e -> yield e
                        | _ -> ()
                    ]
                Error errors

        let loadOrFail (input: TextReader) (output: TextWriter): ToolContext =
            match fromEnvironment () with
            | Ok cfg -> createContext cfg input output
            | Error errors ->
                let errorMessages = errors |> List.map describeError |> String.concat "\n"
                failwith $"Tool configuration failed: {errorMessages}"
