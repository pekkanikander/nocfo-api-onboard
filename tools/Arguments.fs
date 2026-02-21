namespace Nocfo.Tools.Arguments

open System
open System.Reflection
open System.Linq.Expressions
open Argu
open CsvHelper
open CsvHelper.Configuration
open Nocfo.CsvHelpers

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
    | [< NoPrefix; SubCommand >]                  Documents of ParseResults<BusinessScopedArgs>
    | [< NoPrefix; SubCommand >]                  Businesses of ParseResults<BusinessesArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Fields _     -> "Comma-separated list of fields to list/update/... (default: all)."
            | Format _     -> "Input/outputformat (currently only csv)."
            | Accounts _   -> "Accounts of a business."
            | Documents _  -> "Documents of a business."
            | Businesses _ -> "Businesses."

[<RequireSubcommand>]
type MapEntitiesArgs =
    | [< NoPrefix; SubCommand >]                  Accounts of ParseResults<BusinessScopedArgs>
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Accounts _   -> "Map account identifiers between source and target environments."

[<CliPrefix(CliPrefix.None)>]
type DocumentCreateArgs =
    | [< AltCommandLine("-b"); Mandatory >]       BusinessId of string
    | [< AltCommandLine("--account-id-map") >]    AccountIdMap of string
    | [< AltCommandLine("--strict") >]            Strict
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


// -------------------------------
// CSV mapping for -f/--fields
// -------------------------------

module CvsMapping =
    /// Normalize a list into a clean, ordered list.
    let normalizeFields (fields: string list) : string list =
        fields
        |> List.map (fun s -> s.Trim())
        |> List.filter (fun s -> s <> "")

    let private mapProperty<'T> (map: DefaultClassMap<'T>) (p: PropertyInfo) (index: int) (header: string option) =
        let param = Expression.Parameter(typeof<'T>, "x")
        let bodyProp = Expression.Property(param, p) :> Expression
        // Box value types to obj for the non-generic Map overload
        let bodyObj =
            if p.PropertyType.IsValueType then Expression.Convert(bodyProp, typeof<obj>) :> Expression
            else bodyProp
        let lambda : Expression<Func<'T,obj>> = Expression.Lambda<Func<'T, obj>>(bodyObj, param)
        let mm = CsvMapExtensions.MapBoxed(map, lambda).Index(index)
        header |> Option.iter (fun h -> mm.Name(h) |> ignore)
        mm |> ignore

    /// Build a CsvHelper DefaultClassMap<'T> that includes only the selected top-level fields, in the given order.
    /// If the list is empty, all public instance fields of 'T are included in declaration order.
    /// Returns Error with the set of unknown field names if any name doesn't match a field on 'T' (case-insensitive).
    let buildClassMapForFields<'T> (fields: string list) : Result<DefaultClassMap<'T>, string list> =
        let t = typeof<'T>
        let props = t.GetProperties(BindingFlags.Instance ||| BindingFlags.Public)
        let byNameCI =
            props
            |> Array.map (fun p -> p.Name.ToLowerInvariant(), p)
            |> dict
        let wanted =
            match normalizeFields fields with
            | [] -> props |> Array.toList |> List.map (fun p -> p.Name) // default: all
            | xs -> xs

        // Resolve names case-insensitively
        let resolved, missing =
            (([], []), wanted)
            ||> List.fold (fun (accOk, accMissing) name ->
                let key = name.ToLowerInvariant()
                if byNameCI.ContainsKey key then
                    (byNameCI.[key] :: accOk, accMissing)
                else
                    (accOk, name :: accMissing))

        match missing with
        | _::_ ->
            // return unknown names in the order they were requested
            let missingOrdered =
                wanted
                |> List.filter (fun n -> missing |> List.exists (fun m -> String.Equals(m, n, StringComparison.Ordinal)))
                |> List.distinct
            Error missingOrdered
        | [] ->
            // Keep original order: we collected resolved in reverse; reverse back
            let orderedProps = resolved |> List.rev
            let map = DefaultClassMap<'T>()
            orderedProps
            |> List.iteri (fun idx (p: PropertyInfo) ->
                mapProperty map p idx (Some p.Name))
            Ok map
