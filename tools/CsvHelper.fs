namespace Nocfo.Tools

open System
open System.Globalization
open System.IO
open System.Reflection
open System.Text
open CsvHelper
open CsvHelper.Configuration
open FSharp.Control
open Nocfo.Tools.Arguments.CvsMapping

// Depends on your existing helpers:
open Nocfo.CsvHelpers                 // registerFSharpConvertersFor
open Nocfo.Tools.Arguments            // buildClassMapForFields<'T>

module Csv =

  /// Policy when some field names passed via -f/--fields don’t exist on 'T.
  type UnknownFieldPolicy =
    | Fail
    | WarnAndDrop

  /// Writer options with sensible defaults.
  type WriteOptions =
    { Culture            : CultureInfo
      IncludeHeader      : bool
      NewLine            : string
      UnknownFieldPolicy : UnknownFieldPolicy }

  let defaultWriteOptions =
    { Culture            = CultureInfo.InvariantCulture
      IncludeHeader      = true
      NewLine            = "\n"              // stable across platforms
      UnknownFieldPolicy = UnknownFieldPolicy.Fail }

  /// Create a CsvWriter bound to an existing TextWriter.
  let private mkCsvWriter (tw: TextWriter) (opts: WriteOptions) =
    let cfg = CsvConfiguration(opts.Culture)
    cfg.NewLine <- opts.NewLine
    new CsvWriter(tw, cfg)

  /// Register F#-specific converters (option<>, list<>, JToken) for the target type.
  let private registerConverters<'T> (csv: CsvWriter) =
    registerFSharpConvertersFor csv typeof<'T>

  /// If fields are provided, build and register a map that selects only those properties of 'T'.
  /// Returns the list of unknown field names (if any).
  let private tryRegisterFieldsMap<'T> (csv: CsvWriter) (fields: string list option) =
    match fields with
    | None | Some ([] ) -> []
    | Some xs ->
        match buildClassMapForFields<'T> xs with
        | Ok classMap ->
            csv.Context.RegisterClassMap(classMap)
            []
        | Error missing ->
            missing

  /// Core writer: write an AsyncSeq<'T> to CSV using CsvHelper.
  /// - Registers F# converters.
  /// - Optionally restricts columns via -f/--fields (top-level properties).
  /// - Writes header once (configurable).
  let writeCsvGeneric<'T>
      (tw      : TextWriter)
      (fields  : string list option)
      (rows    : AsyncSeq<'T>)
    =
    use csv = mkCsvWriter tw defaultWriteOptions

    // F# shapes (option<>, list<>, JToken)
    registerConverters<'T> csv

    // Apply field selection map (if any)
    let missing = tryRegisterFieldsMap<'T> csv fields
    match missing with
    | (_::_ as miss) ->
        failwithf "Unknown field(s) for %s: %s"
                  typeof<'T>.FullName (String.Join(", ", miss))
    | _ -> ()

    // Header
    csv.WriteHeader<'T>()
    csv.NextRecord()

    // Stream rows
    rows
    |> AsyncSeq.iter (fun item ->
        csv.WriteRecord(item)
        csv.NextRecord())
    |> Async.RunSynchronously

    csv.Flush()
    tw.Flush()


(*
Usage examples (sketch):

// 1) Businesses → CSV (all columns), to stdout
Csv.writeCsvStdout<NocfoApi.Types.Business>(
  businessesAsyncSeq |> AsyncSeq.map (fun b -> b.raw))

// 2) Accounts → CSV with selected columns, to file
let fields = ["id"; "number"; "name"; "balance"]
Csv.writeCsvFile<NocfoApi.Types.Account>(
  "out/accounts.csv",
  accountsAsyncSeq |> AsyncSeq.map id,       // already Account
  fields = fields,
  options = { Csv.defaultWriteOptions with NewLine = "\n"; UnknownFieldPolicy = Csv.UnknownFieldPolicy.WarnAndDrop })

*)
