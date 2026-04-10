namespace Nocfo.Tools.Arguments

open Argu

type NoPrefixAttribute() = inherit CliPrefixAttribute(CliPrefix.None)

type BusinessesArgs =
    | [< Hidden >]             Dummy
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Dummy       -> "Dummy argument for BusinessesArgs."

type BusinessScopedArgs =
    | [< AltCommandLine("-b"); Mandatory >]       BusinessId of string
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | BusinessId _ -> "Business identifier (Y-tunnus|VAT-code)."

[<RequireSubcommand>]
type EntitiesArgs =
    | [< AltCommandLine("-i"); Inherit >]         Fields of fields: string list
    | [< AltCommandLine("-f"); Inherit >]         Format of format: string
    | [< NoPrefix; SubCommand >]                  Accounts of ParseResults<BusinessScopedArgs>
    | [< NoPrefix; SubCommand >]                  Contacts of ParseResults<BusinessScopedArgs>
    | [< NoPrefix; SubCommand >]                  Documents of ParseResults<BusinessScopedArgs>
    | [< NoPrefix; SubCommand >]                  Businesses of ParseResults<BusinessesArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Fields _     -> "Comma-separated list of fields to list/update/... (default: all)."
            | Format _     -> "Input/outputformat (currently only csv)."
            | Accounts _   -> "Accounts of a business."
            | Contacts _   -> "Contacts of a business."
            | Documents _  -> "Documents of a business."
            | Businesses _ -> "Businesses."

[<RequireSubcommand>]
type MapEntitiesArgs =
    | [< NoPrefix; SubCommand >]                  Accounts of ParseResults<BusinessScopedArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Accounts _ -> "Map account identifiers between source and target environments."

[<RequireSubcommand>]
type DocumentCreateArgs =
    | [< AltCommandLine("-b"); Mandatory >]       BusinessId of string
    | [< AltCommandLine("-m") >]                  AccountIdMap of string
    | [< AltCommandLine("-s") >]                  Strict
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | BusinessId _   -> "Target business identifier (Y-tunnus|VAT-code)."
            | AccountIdMap _ -> "Optional CSV path with source_id,target_id,number mappings."
            | Strict         -> "Fail a row if blueprint account IDs are not fully mapped."

[<RequireSubcommand>]
type CreateEntitiesArgs =
    | [< AltCommandLine("-i"); Inherit >]         Fields of fields: string list
    | [< NoPrefix; SubCommand >]                  Documents of ParseResults<DocumentCreateArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Fields _     -> "Comma-separated list of fields to read from CSV (default: all create fields)."
            | Documents _  -> "Create documents from CSV input."

[<RequireSubcommand>]
type CliArgs =
    | [< AltCommandLine("-o") >]                  Out    of outPath: string
    | [< AltCommandLine("-i") >]                  In     of inPath: string
    | [< NoPrefix; SubCommand >]                  List   of ParseResults<EntitiesArgs>
    | [< NoPrefix; SubCommand >]                  Update of ParseResults<EntitiesArgs>
    | [< NoPrefix; SubCommand >]                  Delete of ParseResults<EntitiesArgs>
    | [< NoPrefix; SubCommand >]                  Map    of ParseResults<MapEntitiesArgs>
    | [< NoPrefix; SubCommand >]                  Create of ParseResults<CreateEntitiesArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Out _        -> "Optional CSV output path (default stdout)."
            | In _         -> "Optional CSV input path (default stdin)."
            | List _       -> "List entities (businesses, accounts, etc.)."
            | Update _     -> "Update an entity (business, account, etc.)."
            | Delete _     -> "Delete entities (accounts, etc.)."
            | Map _        -> "Map entities between source and target environments."
            | Create _     -> "Create entities from CSV input."
