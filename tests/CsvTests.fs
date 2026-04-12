module CsvTests

open System.IO
open Xunit
open Swensen.Unquote
open FSharp.Control
open Nocfo

// ── Test record ───────────────────────────────────────────────────────────────

[<CLIMutable>]
type Row = { id: int; name: string; note: string option }

// ── Helpers ───────────────────────────────────────────────────────────────────

let private writeToCsv (fields: string list option) (rows: Row list) : string =
    use writer = new StringWriter()
    Csv.writeCsvGeneric<Row> writer fields (AsyncSeq.ofSeq rows)
    |> AsyncSeq.toListSynchronously
    |> ignore
    writer.ToString()

let private readFromCsv (fields: string list option) (csv: string) : Row list =
    use reader = new StringReader(csv)
    Csv.readCsvGeneric<Row> reader fields
    |> AsyncSeq.toListSynchronously

let private roundTrip (fields: string list option) (rows: Row list) : Row list =
    writeToCsv fields rows |> readFromCsv fields

// ── Round-trip: all fields ────────────────────────────────────────────────────

[<Fact>]
let ``Round-trip: multiple rows, all fields`` () =
    let rows = [
        { id = 1; name = "Alice"; note = Some "first" }
        { id = 2; name = "Bob";   note = None }
        { id = 3; name = "Carol"; note = Some "third" }
    ]
    test <@ roundTrip None rows = rows @>

[<Fact>]
let ``Round-trip: empty input yields empty list`` () =
    test <@ roundTrip None [] = [] @>

// ── Round-trip: field selection ───────────────────────────────────────────────

[<Fact>]
let ``Round-trip: selected fields only`` () =
    let rows = [ { id = 1; name = "Alice"; note = Some "x" } ]
    let result = roundTrip (Some ["id"; "name"]) rows
    // 'note' was not in the selection → defaults to None after read-back
    test <@ result = [ { id = 1; name = "Alice"; note = None } ] @>

// ── Option serialisation ──────────────────────────────────────────────────────

[<Fact>]
let ``Optional field None survives round-trip`` () =
    let rows = [ { id = 1; name = "X"; note = None } ]
    test <@ roundTrip None rows = rows @>

[<Fact>]
let ``Optional field Some survives round-trip`` () =
    let rows = [ { id = 1; name = "X"; note = Some "hello" } ]
    test <@ roundTrip None rows = rows @>

// ── Header validation ─────────────────────────────────────────────────────────

[<Fact>]
let ``Unknown column in CSV raises exception`` () =
    let csv = "id,name,note,bogus\n1,Alice,,\n"
    Assert.Throws<exn>(fun () ->
        readFromCsv None csv |> ignore)
    |> ignore

[<Fact>]
let ``Missing required column in field selection raises exception`` () =
    // Requesting a field that does not exist on the type
    Assert.Throws<exn>(fun () ->
        writeToCsv (Some ["id"; "nonexistent"]) [] |> ignore)
    |> ignore

// ── writeCsvGeneric output shape ──────────────────────────────────────────────

[<Fact>]
let ``Written CSV contains header row`` () =
    let csv = writeToCsv None [ { id = 1; name = "A"; note = None } ]
    let lines = csv.Split('\n') |> Array.filter (fun s -> s.Trim() <> "")
    // at least 2 lines: header + one data row
    test <@ lines.Length >= 2 @>

[<Fact>]
let ``Written CSV header contains expected column names`` () =
    let csv = writeToCsv None []
    let header = csv.Split('\n').[0].ToLowerInvariant()
    test <@ header.Contains "id" @>
    test <@ header.Contains "name" @>
    test <@ header.Contains "note" @>
