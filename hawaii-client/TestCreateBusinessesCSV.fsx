#r "nuget: FSharp.Control.AsyncSeq, 3.2.1"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"

#r "bin/Debug/net10.0/hawaii-client.dll"
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

// -----------------------------------------------------------------------------
// Goal: stream businesses and write the minimal CSV: id,slug
//
// Design choices (kept minimal and explicit):
// - We stream with AsyncSeq and write rows as they arrive (no buffering).
// - We write only BusinessKey fields (id, slug) so both Full and Partial items work.
// - We accept a single CLI option: --out <path>. If missing, write to stdout.
// - We use a tiny manual CSV writer (RFC4180-style) to keep dependencies minimal.
// -----------------------------------------------------------------------------

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

// ---- write CSV given a TextWriter ----
// Minimal RFC4180-compatible writer: quotes fields that contain comma, quote, or newline.
let private escapeCsv (s: string) =
    let s = if isNull s then "" else s
    let needsQuotes =
        s.IndexOfAny([|','; '"'; '\r'; '\n'|]) >= 0
    let s' = s.Replace("\"", "\"\"")
    if needsQuotes then "\"" + s' + "\"" else s'

let private writeRow (tw: TextWriter) (cells: string array) =
    cells |> Array.map escapeCsv |> String.concat "," |> tw.WriteLine

let private replaceNewlines (s: string) =
    s.Replace("\r", "").Replace("\n", ";")
let writeCsv (tw: TextWriter) =
    // Header
    writeRow tw [| "id"; "slug" |]

    // Data rows streamed as they arrive
    Streams.streamBusinesses accounting
    |> AsyncSeq.choose tryKey
    |> AsyncSeq.iter (fun k ->
         [| string k.id; k.slug |] |> Array.map replaceNewlines |> writeRow tw)
    |> Async.RunSynchronously

    tw.Flush()

// ---- run: decide destination and write ----
match outPathOpt with
| Some path ->
    // Write to file (create/overwrite)
    use sw = new StreamWriter(path, false, System.Text.Encoding.UTF8)
    writeCsv sw
    eprintfn "Wrote businesses CSV to %s" path
| None ->
    // Write to stdout. Disposing CsvWriter may close Console.Out, which is fine at script end.
    writeCsv Console.Out
