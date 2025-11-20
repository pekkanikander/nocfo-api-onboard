open System
open Argu
open FSharp.Control
open Nocfo.Tools.Arguments
open Nocfo.Domain

let handleEntitiesArgs (args: ParseResults<EntitiesArgs>) =
    let entityTypeAndArgs = args.GetSubCommand()
    let fields = args.GetResult(Fields, defaultValue = [])
    (entityTypeAndArgs, fields)

let listBusinesses (args: ParseResults<BusinessesArgs>) =
    async {
        return 1 // TODO: implement
    }

let listAccounts (args: ParseResults<AccountsArgs>) =
    async {
        let toolContext = Nocfo.Tools.Runtime.ToolConfig.loadOrFail()
        let businessId = args.GetResult(BusinessId, defaultValue = "")
        let! businessContext  = BusinessResolver.resolve toolContext.Accounting businessId
        match businessContext with
        | Ok businessContext ->
            let! accounts =
                Streams.streamAccounts businessContext
                |> AsyncSeq.toListAsync
            eprintfn "accounts: %A" accounts
            return 0
        | Error error ->
            eprintfn "error: %A" error
            return 1
    }

let list (args: ParseResults<EntitiesArgs>) =
    async {
        let (entityTypeAndArgs, fieldList) = handleEntitiesArgs args
        eprintfn "entityTypeAndArgs: %A" entityTypeAndArgs
        eprintfn "fieldList: %A" fieldList
        let fieldClassMap = CvsMapping.buildClassMapForFields<NocfoApi.Types.Account> fieldList
        eprintfn "fieldClassMap: %A" fieldClassMap
        return!
            match entityTypeAndArgs with
            | EntitiesArgs.Accounts args   -> listAccounts args
            | EntitiesArgs.Businesses args -> listBusinesses args
            | _ -> failwith "Unknown entity type"
    }

let patch (args: ParseResults<EntitiesArgs>) =
    async {
        let (entityTypeAndArgs, fields) = handleEntitiesArgs args
        return 0
    }

[<EntryPoint>]
let main argv =
    async {
        let parser = ArgumentParser.Create<CliArgs>(programName = "nocfo")
        let results = parser.ParseCommandLine(argv, raiseOnUsage = false)
        let subcommand = results.GetSubCommand()
        return!
            match subcommand with
            | CliArgs.List _  -> list  (results.GetResult List)
            | CliArgs.Patch _ -> patch (results.GetResult Patch)
            | _ ->
                eprintfn "%s" (parser.PrintUsage())
                async.Return 1
    } |> Async.RunSynchronously
