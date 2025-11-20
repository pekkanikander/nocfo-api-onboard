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

let listBusinesses (args: ParseResults<BusinessesArgs>) =
    async {
        return 1 // TODO: implement
    }

let listAccounts (toolContext: ToolContext) (args: ParseResults<AccountsArgs>) (fieldList: string list) =
    async {
        let output = toolContext.Output
        let writeCsv =
            Nocfo.Tools.Csv.writeCsvGeneric<NocfoApi.Types.Account>
                output (Some fieldList)
        let businessId = args.GetResult(BusinessId, defaultValue = "")
        let! businessContext  = BusinessResolver.resolve toolContext.Accounting businessId
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
            return 1
    }

let list (toolContext: ToolContext) (args: ParseResults<EntitiesArgs>) =
    async {
        let (entityTypeAndArgs, fieldList) = handleEntitiesArgs args
        eprintfn "entityTypeAndArgs: %A" entityTypeAndArgs
        eprintfn "fieldList: %A" fieldList
        return!
            match entityTypeAndArgs with
            | EntitiesArgs.Accounts args   -> listAccounts toolContext args fieldList
            | EntitiesArgs.Businesses args -> listBusinesses args
            | _ -> failwith "Unknown entity type"
    }

let patch  (toolContext: ToolContext) (args: ParseResults<EntitiesArgs>) =
    async {
        let (entityTypeAndArgs, fields) = handleEntitiesArgs args
        return 0
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
            | CliArgs.Patch _ -> patch toolContext (results.GetResult Patch)
            | _ ->
                eprintfn "%s" (parser.PrintUsage())
                async.Return 1
    } |> Async.RunSynchronously
