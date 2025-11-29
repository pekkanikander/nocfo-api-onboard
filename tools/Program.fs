open System
open System.IO
open Argu
open FSharp.Control
open Nocfo.Domain
open Nocfo.Tools.Arguments
open Nocfo.Tools

let handleEntitiesArgs (args: ParseResults<EntitiesArgs>) =
    let entityTypeAndArgs = args.GetSubCommand()
    let fields = args.GetResult(Fields, defaultValue = [])
    (entityTypeAndArgs, fields)

let listBusinesses (toolContext: ToolContext) (args: ParseResults<BusinessesArgs>) =
    async {
        return 1 // TODO: implement
    }

let private getBusinessContext (toolContext: ToolContext) (args: ParseResults<AccountsArgs>) =
    async {
        let businessId = args.GetResult(BusinessId, defaultValue = "")
        let! businessContext  = BusinessResolver.resolve toolContext.Accounting businessId
        return businessContext
    }

let listAccounts (toolContext: ToolContext) (args: ParseResults<AccountsArgs>) (fieldList: string list) =
    async {
        let output = toolContext.Output
        let writeCsv =
            Nocfo.Tools.Csv.writeCsvGeneric<NocfoApi.Types.Account> output (Some fieldList)
        let! businessContext  = getBusinessContext toolContext args
        match businessContext with
        | Ok businessContext ->
            Streams.streamAccounts businessContext
            |> Streams.hydrateAndUnwrap
            |> AsyncSeq.map (function
                | Ok account -> account
                | Error error -> failwithf "Failed to hydrate account: %A" error)
            |> writeCsv

            return 0
        | Error error ->
            eprintfn "Failed to get business context: %A" error
            return 1
    }

let updateBusinesses (toolContext: ToolContext) (args: ParseResults<BusinessesArgs>) =
    async {
        return 1 // TODO: implement
    }

let updateAccounts (toolContext: ToolContext) (args: ParseResults<AccountsArgs>) (fields: string list) =
    async {
        let f = "id" :: fields
        let input = toolContext.Input
        let csvStream =
            Nocfo.Tools.Csv.readCsvGeneric<NocfoApi.Types.PatchedAccount> input (Some f)
            |> AsyncSeq.map Ok
        let! businessContext  = getBusinessContext toolContext args
        match businessContext with
        | Ok ctx ->
            let accountStream =
                Streams.streamAccounts ctx
                |> Streams.hydrateAndUnwrap
            let commands =
                Account.deltasToCommands accountStream csvStream
            let execution =
                Streams.executeAccountCommands ctx commands

            let! finalState =
                execution
                |> AsyncSeq.foldAsync (fun state result ->
                    async {
                        match state with
                        | Error err -> return Error err
                        | Ok () ->
                            match result with
                            | Ok account ->
                                printfn "Updated account %d (%s)" account.id account.number
                                return Ok ()
                            | Error err ->
                                return Error err
                    }) (Ok ())

            match finalState with
            | Ok () -> return 0
            | Error err ->
                eprintfn "Update failed: %A" err
                return 1
        | Error error ->
            eprintfn "Failed to get business context: %A" error
            return 1
    }

// XXX: TODO: Implement an abstract 'command' type and a map of commands to functions.
let list (toolContext: ToolContext) (args: ParseResults<EntitiesArgs>) =
    async {
        let (entityTypeAndArgs, fields) = handleEntitiesArgs args
        return!
            match entityTypeAndArgs with
            | EntitiesArgs.Businesses args -> listBusinesses toolContext args
            | EntitiesArgs.Accounts args   -> listAccounts toolContext args fields
            | _ -> failwith "Unknown entity type"
    }

let update  (toolContext: ToolContext) (args: ParseResults<EntitiesArgs>) =
    async {
        let (entityTypeAndArgs, fields) = handleEntitiesArgs args
        return!
            match entityTypeAndArgs with
            | EntitiesArgs.Businesses args -> updateBusinesses toolContext args
            | EntitiesArgs.Accounts args   -> updateAccounts toolContext args fields
            | _ -> failwith "Unknown entity type"
    }

[<EntryPoint>]
let main argv =
    async {

        let parser = ArgumentParser.Create<CliArgs>(programName = "nocfo")
        let results: ParseResults<CliArgs> =
            parser.ParseCommandLine(argv, raiseOnUsage = false)

        let input : TextReader =
            match results.TryGetResult CliArgs.In with
            | Some path -> upcast new StreamReader(path)
            | None -> Console.In

        let output : TextWriter =
            match results.TryGetResult CliArgs.Out with
            | Some path -> upcast new StreamWriter(path)
            | None -> Console.Out

        let toolContext = Nocfo.Tools.Runtime.ToolConfig.loadOrFail input output

        let subcommand = results.GetSubCommand()
        return!
            match subcommand with
            | CliArgs.List _  -> list  toolContext (results.GetResult List)
            | CliArgs.Update _ -> update toolContext (results.GetResult Update)
            | _ ->
                eprintfn "%s" (parser.PrintUsage())
                async.Return 1
    } |> Async.RunSynchronously
