# Accounting Tools, starting with CSV

## Overall objective

Study how to build CLI tools with F#, for accounting and financing.

### First concrete objective
- Ship a near-production-ready CLI that streams all accounts for a business and writes every field defined in the OpenAPI schema to CSV.

### Existing Assets

- Domain plumbing already streams businesses/accounts and hydrates to full records: `hawaii-client/src/{Streams,Domain}.fs`
- `Nocfo.CsvHelpers` registers the converters we need for CsvHelper.
- `TestCreateCSV.fsx` proves the pipeline; we now lift that logic into a real toolset.

### Deliverables

- `tools/Tools.fsproj` (console) referencing `hawaii-client`, intended to be publishable as a `dotnet tool`.
- Namespace structure `Nocfo.Tools` with a `Nocfo.Tools.Runtime` module exposing `ToolConfig` (parsed CLI/env) and `ToolContext` (http + accounting + common services, such as logging in the future).
- First command: `nocfo list --format csv accounts --business-id <id> [--out <path>] [--fields ...]`; it will evolve but should already feel production-ready (help text, exit codes, validation)
  - It is expected that there will soon be other `list` commands, such as `businesses`
  - It is expected that there will be other formats in addition to `csv`, such as plain text (perhaps the default later)
  - Note the order of the options and subcommands.

### Functional Requirements

- Business lookup:
  - Accept a business identifier, in the form of `Y-tunnus|VAT-code <number>`.
    - If feasible, these formats should be derived from the OpenAPI types instead of manually coding
  - Fetch the business list, keep only matches, pick the first result, extract slug internally, and fail with a clear message when nothing matches.
  - Encapsulate the logic in a `BusinessResolver` utility.

- Streaming + CSV:
  - Use existing domain streams to walk all accounts for the resolved business.
  - Hydrate every account before serialization.
  - By default serialize every field the generated `Account` type exposes
    - Adopt the CsvHelper to limit/order which fields to export; by default export all fields.
    - Generate a CSVHelper ClassMap from the CLI option to limit which fields to export (and later also how, but not yet)
      - Later these maps may be read from a configuration file(s)
  - Write to a file when `--out` is specified, otherwise to stdout; ensure UTF‑8 and deterministic headers.

- CLI scaffolding:
  - Introduce a reusable option parsing setup (Argu or similar) inside `Nocfo.Tools`.
  - Validate inputs (required options, writable destination) and surface errors via exit codes/messages only—no logging yet.

- Environment + context:
  - Centralize env handling (token, base URL, future knobs) inside `Nocfo.Tools.Runtime`.
  - Build `HttpContext` and `AccountingContext` once per invocation and pass through `ToolContext`.

### Non-Functional Requirements
- No logging for now; stderr is only for fatal errors/help text.
- Keep dependencies minimal; reuse the main project packages.
- Defensive programming: short-circuit when env vars or HTTP calls fail, with brief actionable messages. Unix-style.

### Deferred / Future Work

- Retry/logging/caching policies on `AccountingContext`.
- Additional subcommands and richer CLI UX once the shared runtime solidifies.

## Appendix I: Initial format for ClassMap

Here is an initial idea for a format of reading a CSVHelper ClassMap from a file:
```
{
  "version": 1,
  "targetType": "NocfoApi.Types.Account",
  "columns": [
    { "path": "id",          "header": "id",          "index": 0 },
    { "path": "number",      "header": "number",      "index": 1 },
    { "path": "name",        "header": "name",        "index": 2 },
    { "path": "type",        "header": "type",        "index": 3 },
    { "path": "balance",     "header": "balance",     "index": 4, "format": "G" }
  ]
}
```

Pass the format to pass to IFormattable.ToString(format, InvariantCulture).

A sketch to build it:
```
open CsvHelper.Configuration
open System.Reflection

let buildMap<'T> (cfg: ConfigRecord) =
    let map = DefaultClassMap<'T>()
    for col in cfg.columns |> List.sortBy (fun c -> c.index) do
        // Find property by name; for dot-paths, walk the chain and fall back to .Convert
        let p = typeof<'T>.GetProperty(col.path, BindingFlags.Instance ||| BindingFlags.Public)
        if isNull p then failwithf "No property '%s' on %O" col.path typeof<'T>
        let mm = map.Map(p).Name(col.header).Index(col.index)
        // Optional: apply per-column converter/format if you add those to the schema
        ignore mm
    map
```
