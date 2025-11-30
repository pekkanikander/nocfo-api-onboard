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
  type IOOptions =
    { Culture            : CultureInfo
      IncludeHeader      : bool
      NewLine            : string
      UnknownFieldPolicy : UnknownFieldPolicy }

  let defaultIOOptions =
    { Culture            = CultureInfo.InvariantCulture
      IncludeHeader      = true
      NewLine            = "\n"              // stable across platforms
      UnknownFieldPolicy = UnknownFieldPolicy.Fail }

  /// Create a CsvWriter bound to an existing TextWriter.
  let private mkCsvWriter (tw: TextWriter) (opts: IOOptions) =
    let cfg = CsvConfiguration(opts.Culture)
    new CsvWriter(tw, cfg)

  let private mkCsvReader (tr: TextReader) (opts: IOOptions) =
    let cfg = CsvConfiguration(opts.Culture)
    // Ignore extra columns in CSV that aren't in the class map
    cfg.MissingFieldFound <- null
    new CsvReader(tr, cfg)

  /// If fields are provided, build and register a map that selects only those properties of 'T'.
  /// Returns the list of unknown field names (if any).
  let private tryRegisterFieldsMap<'T> (context: CsvContext) (fields: string list option) =
    match fields with
    | None | Some ([] ) -> []
    | Some xs ->
        match buildClassMapForFields<'T> xs with
        | Ok classMap ->
            context.RegisterClassMap(classMap)
            []
        | Error missing ->
            missing

  /// Core writer: write an AsyncSeq<'T> to CSV using CsvHelper.
  /// - Registers F# converters.
  /// - Optionally restricts columns via -f/--fields (top-level properties).
  /// - Writes header once (configurable).
  /// - Returns a lazy AsyncSeq<unit> that writes as it's consumed.
  let writeCsvGeneric<'T>
      (tw      : TextWriter)
      (fields  : string list option)
      (rows    : AsyncSeq<'T>)
    : AsyncSeq<unit> =
    let csv = mkCsvWriter tw defaultIOOptions

    // F# shapes (option<>, list<>, JToken)
    registerFSharpConvertersFor csv.Context typeof<'T>

    // Apply field selection map (if any)
    let missing = tryRegisterFieldsMap<'T> csv.Context fields
    match missing with
    | (_::_ as miss) ->
        failwithf "Unknown field(s) for %s: %s"
                  typeof<'T>.FullName (String.Join(", ", miss))
    | _ -> ()

    let mutable disposed = false
    let dispose () =
        if not disposed then
            csv.Flush()
            tw.Flush()
            (csv :> IDisposable).Dispose()
            disposed <- true

    asyncSeq {
        try
            // Header
            csv.WriteHeader<'T>()
            csv.NextRecord()
            yield ()

            // Stream rows
            yield! rows |> AsyncSeq.map (fun item ->
                csv.WriteRecord(item)
                csv.NextRecord()
            )
        finally
            dispose()
    }


  let readCsvGeneric<'T>
      (tr: TextReader)
      (fields: string list option)
    : AsyncSeq<'T> =
    let csv = mkCsvReader tr defaultIOOptions

    registerFSharpConvertersFor csv.Context typeof<'T>

    let missing = tryRegisterFieldsMap<'T> csv.Context fields
    match missing with
    | (_::_ as miss) ->
        failwithf "Unknown field(s) for %s: %s"
                  typeof<'T>.FullName (String.Join(", ", miss))
    | _ -> ()

    let mutable disposed = false
    let dispose () =
        if not disposed then
            (csv :> IDisposable).Dispose()
            disposed <- true

    asyncSeq {
        try
            if csv.Read() then
                csv.ReadHeader() |> ignore
                while csv.Read() do
                    yield csv.GetRecord<'T>()
        finally
            dispose()
    }
