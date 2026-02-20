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

let listBusinesses (toolContext: ToolContext) (args: ParseResults<BusinessesArgs>) (fields: string list) =
    async {
        let output = toolContext.Output
        let rows =
            Streams.streamBusinesses toolContext.Accounting
            |> Streams.hydrateAndUnwrap
            |> AsyncSeq.map (function
                | Ok business -> business.raw
                | Error error -> failwithf "Failed to get business: %A" error)
        let writeCsv =
            Nocfo.Tools.Csv.writeCsvGeneric<NocfoApi.Types.Business> output (Some fields) rows
        do! writeCsv |> AsyncSeq.iter ignore
        return 0
    }

let private getBusinessContext (toolContext: ToolContext) (args: ParseResults<BusinessScopedArgs>) =
    async {
        let businessId = args.GetResult(BusinessId, defaultValue = "")
        let! businessContext  = BusinessResolver.resolve toolContext.Accounting businessId
        return businessContext
    }

let listAccounts (toolContext: ToolContext) (args: ParseResults<BusinessScopedArgs>) (fields: string list) =
    async {
        let output = toolContext.Output
        let! businessContext  = getBusinessContext toolContext args
        match businessContext with
        | Ok businessContext ->
            let rows =
                Streams.streamAccounts businessContext
                |> Streams.hydrateAndUnwrap
                |> AsyncSeq.map (function
                    | Ok account -> account
                    | Error error -> failwithf "Failed to hydrate account: %A" error)
            let writeCsv =
                Nocfo.Tools.Csv.writeCsvGeneric<NocfoApi.Types.Account> output (Some fields) rows
            do! writeCsv |> AsyncSeq.iter ignore
            return 0
        | Error error ->
            eprintfn "Failed to get business context: %A" error
            return 1
    }

let listDocuments (toolContext: ToolContext) (args: ParseResults<BusinessScopedArgs>) (fields: string list) =
    async {
        let output = toolContext.Output
        let! businessContext = getBusinessContext toolContext args
        match businessContext with
        | Ok businessContext ->
            let rows =
                Streams.streamDocuments businessContext
                |> Streams.hydrateAndUnwrap
                |> AsyncSeq.map (function
                    | Ok document -> document
                    | Error error -> failwithf "Failed to hydrate document: %A" error)
            let writeCsv =
                Nocfo.Tools.Csv.writeCsvGeneric<NocfoApi.Types.DocumentList> output (Some fields) rows
            do! writeCsv |> AsyncSeq.iter ignore
            return 0
        | Error error ->
            eprintfn "Failed to get business context: %A" error
            return 1
    }

let updateBusinesses (toolContext: ToolContext) (args: ParseResults<BusinessesArgs>) =
    async {
        return 1 // TODO: implement
    }

let foldCommandResults (results: AsyncSeq<Result<AccountResult, DomainError>>) : Async<int> =
    async {
        let! errorCount =
            results
            |> AsyncSeq.fold (fun errorCount result ->
                match result with
                | Ok (AccountUpdated account) ->
                    printfn "Updated account %d (%s)" account.id account.number
                    errorCount
                | Ok (AccountDeleted accountId) ->
                    printfn "Deleted account %d" accountId
                    errorCount
                | Error err ->
                    printfn "Command failed: %A" err
                    errorCount + 1) 0
        return if errorCount > 0 then 1 else 0
    }

let updateAccounts (toolContext: ToolContext) (args: ParseResults<BusinessScopedArgs>) (fields: string list) =
    async {
        let f = "id" :: fields
        let input = toolContext.Input
        let! businessContext  = getBusinessContext toolContext args
        match businessContext with
        | Ok ctx ->
            // The desired state of accounts from the CSV file.
            let csvStream =
                Nocfo.Tools.Csv.readCsvGeneric<NocfoApi.Types.PatchedAccount> input (Some f)
                |> AsyncSeq.map Ok
            // The current state of accounts from the API.
            let accountStream =
                Streams.streamAccounts ctx
                |> Streams.hydrateAndUnwrap
            // Compute the deltas between the desired and current state.
            // Convert the deltas to commands. Execute the commands. Return the exit code.
            return!
                Account.deltasToCommands accountStream csvStream
                |> Streams.executeAccountCommands ctx
                |> foldCommandResults
        | Error error ->
            eprintfn "Failed to get business context: %A" error
            return 1
    }

let deleteAccounts (toolContext: ToolContext) (args: ParseResults<BusinessScopedArgs>) (fields: string list) =
    async {
        let f = "id" :: fields
        let input = toolContext.Input
        let csvStream =
            Nocfo.Tools.Csv.readCsvGeneric<NocfoApi.Types.PatchedAccount> input (Some f)
            |> AsyncSeq.map Ok
        let! businessContext = getBusinessContext toolContext args
        match businessContext with
        | Ok ctx ->
            let commands =
                csvStream
                |> AsyncSeq.map (Result.map (fun account -> AccountCommand.DeleteAccount account.id))
            return!
                commands
                |> Streams.executeAccountCommands ctx
                |> foldCommandResults
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
            | EntitiesArgs.Businesses args -> listBusinesses toolContext args fields
            | EntitiesArgs.Accounts args   -> listAccounts toolContext args fields
            | EntitiesArgs.Documents args  -> listDocuments toolContext args fields
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

let delete (toolContext: ToolContext) (args: ParseResults<EntitiesArgs>) =
    async {
        let (entityTypeAndArgs, fields) = handleEntitiesArgs args
        return!
            match entityTypeAndArgs with
            | EntitiesArgs.Accounts args -> deleteAccounts toolContext args fields
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
            | CliArgs.List _   -> list   toolContext (results.GetResult List)
            | CliArgs.Update _ -> update toolContext (results.GetResult Update)
            | CliArgs.Delete _ -> delete toolContext (results.GetResult Delete)
            | _ ->
                eprintfn "%s" (parser.PrintUsage())
                async.Return 1
    } |> Async.RunSynchronously
