open System
open Argu

type NoPrefixAttribute() = inherit CliPrefixAttribute(CliPrefix.None)

type BusinessesArgs =
    | [< AltCommandLine("-f") >]                  Fields of fields: string list
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Fields _     -> "Comma-separated list of fields to export (default all)."

type AccountsArgs =
    | [< AltCommandLine("-b"); Mandatory >]       BusinessId of string
    | [< AltCommandLine("-f") >]                  Fields of fields: string list
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | BusinessId _ -> "Business identifier (Y-tunnus|VAT-code)."
            | Fields _     -> "Comma-separated list of fields to export (default all)."

type EntitiesArgs =
    | [< AltCommandLine("-f") >]                  Format of format: string
    | [< NoPrefix; SubCommand >]                  Accounts of ParseResults<AccountsArgs>
    | [< NoPrefix; SubCommand >]                  Businesses of ParseResults<BusinessesArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Format _     -> "Output format (currently only csv)."
            | Accounts _   -> "Accounts of a business."
            | Businesses _ -> "Businesses."

type CliArgs =
    | [< AltCommandLine("-o") >]                  Out   of outPath: string
    | [< AltCommandLine("-i") >]                  In    of inPath: string
    | [< NoPrefix; SubCommand >]                  List  of ParseResults<EntitiesArgs>
    | [< NoPrefix; SubCommand >]                  Patch of ParseResults<EntitiesArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Out _        -> "Optional CSV output path (default stdout)."
            | In _         -> "Optional CSV input path (default stdin)."
            | List _       -> "List entities (businesses, accounts, etc.)."
            | Patch _      -> "Patch an entity (business, account, etc.)."

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<CliArgs>(programName = "nocfo")
    let _results = parser.ParseCommandLine(argv, raiseOnUsage = true)
    printfn "Command line arguments parsed successfully."
    0
