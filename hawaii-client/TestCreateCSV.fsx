#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "nuget: CsvHelper, 30.0.1"


#r "bin/Debug/net9.0/hawaii-client.dll"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"

// Load shared env + basics (token, baseUrl)
#load "TestSupport.fsx"
open TestSupport

open System
open System.IO
open FSharp.Control
open NocfoClient
open NocfoClient.Streams
open Nocfo.Domain
open Nocfo.CsvHelpers

open CsvHelper
open CsvHelper.Configuration
open System.Globalization


// ---- tiny CLI parsing for: --out <path> ----
let args = fsi.CommandLineArgs |> Array.skip 1
let outPathOpt =
    let rec loop i =
        if i >= args.Length then None
        elif args.[i] = "--out" && i + 1 < args.Length then Some args.[i + 1]
        else loop (i + 1)
    loop 0

// ---- helper: get BusinessKey from Hydratable, ignore errors ----
let tryKey =
    function
    | Ok (Business.Full b)         -> Some b.key
    | Ok (Business.Partial (p, _)) -> Some p
    | Error _                      -> None
let private replaceNewlines (s: string) =
    s.Replace("\r", "").Replace("\n", ";")

let csvConfig = CsvConfiguration(CultureInfo.InvariantCulture)

// Helper: extract only Ok values from a stream of Results, dropping all errors
let unwrap<'T> (stream: AsyncSeq<Result<'T, DomainError>>) : AsyncSeq<'T> =
    stream |> AsyncSeq.choose (function
      | Ok value -> Some value
      | Error _ -> None)

let testing<'T> (sw: StreamWriter) (stream: AsyncSeq<'T>) =
    printfn "Type: %A" (typeof<'T>)
    printfn "IsGenericType: %A" (typeof<'T>.IsGenericType)
    printfn "IsGenericTypeDefinition: %A" (typeof<'T>.IsGenericTypeDefinition)
    printfn "Properties: %A" (typeof<'T>.GetProperties() |> Array.map (fun p -> p.Name, p.PropertyType))

let writeCsvGeneric<'T> (sw: StreamWriter) (stream: AsyncSeq<'T>) =
    use csv = new CsvWriter(sw, csvConfig)
    // Prepare mapping with F# converters for the target type
    registerFSharpConvertersFor csv (typeof<NocfoApi.Types.Business>)
    csv.WriteHeader<'T>()
    csv.NextRecord()
    stream
    |> AsyncSeq.iter (fun k ->
         csv.WriteRecord(k)
         csv.NextRecord())
    |> Async.RunSynchronously
    csv.Flush()

// Create a specialized writeCsv function for BusinessFull
let writeCsv sw =
    writeCsvGeneric<NocfoApi.Types.Business> sw
       (Nocfo.Domain.Streams.streamBusinesses accounting
        |> Nocfo.Domain.Streams.hydrateAndUnwrap
        |> unwrap
        |> AsyncSeq.map (fun b -> b.raw))

// ---- run: decide destination and write ----
match outPathOpt with
| Some path ->
    // Write to file (create/overwrite)
    use sw = new StreamWriter(path, false, System.Text.Encoding.UTF8)
    writeCsv sw
    eprintfn "Wrote businesses CSV to %s" path
| None ->
    // Write to stdout. Disposing CsvWriter may close Console.Out, which is fine at script end.
    writeCsv (new StreamWriter(Console.OpenStandardOutput(), System.Text.Encoding.UTF8))
