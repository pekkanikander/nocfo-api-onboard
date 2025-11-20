open System
open Argu
open Nocfo.Tools.Arguments

type EntitiesArgsResult =
    | Accounts of ParseResults<AccountsArgs>
    | Businesses of ParseResults<BusinessesArgs>
    | Unknown

let handleEntitiesArgs (args: ParseResults<EntitiesArgs>) =
    let entityType = args.GetSubCommand()
    let fields = args.GetResult(Fields, defaultValue = [])
    eprintfn "fields: %A" fields
    match entityType with
    | EntitiesArgs.Accounts args ->
        eprintfn "accounts entity args: %A" args
        (entityType, (Accounts args), fields)
    | EntitiesArgs.Businesses args ->
        eprintfn "businesses entity args: %A" args
        (entityType, (Businesses args), fields)
    | _ ->
        eprintfn "Unknown entity type: %A" entityType
        (entityType, (Unknown), fields)

let list (args: ParseResults<EntitiesArgs>) =
    let (entityType, args, fieldList) = handleEntitiesArgs args
    let fieldClassMap = CvsMapping.buildClassMapForFields<NocfoApi.Types.Account> fieldList
    eprintfn "fieldClassMap: %A" fieldClassMap
    0

let patch (args: ParseResults<EntitiesArgs>) =
    let (entityType, args, fields) = handleEntitiesArgs args
    0

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<CliArgs>(programName = "nocfo")
    let results = parser.ParseCommandLine(argv, raiseOnUsage = false)
    let subcommand = results.GetSubCommand()
    match subcommand with
    | CliArgs.List _  ->   list  (results.GetResult List)
    | CliArgs.Patch _ ->   patch (results.GetResult Patch)
    | _               ->   eprintfn "%s" (parser.PrintUsage()); 1
