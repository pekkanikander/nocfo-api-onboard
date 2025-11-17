open System
open Argu

type AccountsArgs =
    | [< AltCommandLine("-b"); Mandatory >]  BusinessId of string
    | [< AltCommandLine("-o") >]             Out of outPath: string
    | [< AltCommandLine("-f") >]             Fields of fields: string list
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | BusinessId _ -> "Business identifier (Y-tunnus|VAT-code)."
            | Out _        -> "Optional CSV output path (default stdout)."
            | Fields _     -> "Comma-separated list of fields to export (default all)."

type ListArgs =
    | [< AltCommandLine("-f") >]                  Format of format: string
    | [< CliPrefix(CliPrefix.None); SubCommand >] Accounts of ParseResults<AccountsArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Format _     -> "Output format (currently only csv)."
            | Accounts _   -> "List accounts for a resolved business."

type CliArgs =
    | [<CliPrefix(CliPrefix.None); SubCommand>] List of ParseResults<ListArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | List _       -> "List entities (businesses, accounts, etc.)."

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<CliArgs>(programName = "nocfo")
    let _results = parser.ParseCommandLine(argv, raiseOnUsage = true)
    printfn "Command line arguments parsed successfully."
    0
