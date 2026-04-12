# CLAUDE.md — nocfo-api-onboard

## What This Is

An F# CLI tool and client library for the [NoCFO](https://nocfo.io) Finnish accounting API.
The repo is in **late exploration / early adoption** stage: the pattern is settled, the code works,
but it is not yet a polished production tool.

The fifth iteration won: **F# + Hawaii OpenAPI generator + lazy `AsyncSeq` streams + pure domain folds**.
Previous iterations (TypeScript, PureScript, F#+NSwag) were discarded; their directories are kept for
reference only — do not modify them.

---

## Repository Layout

```
api/                     NoCFO OpenAPI spec (source of truth for regeneration)
hawaii-client/           The library (used by tools/ and by FSI scripts)
  generated/             Hawaii-generated code — do NOT hand-edit
  src/                   Hand-written domain, streaming, HTTP, CSV logic
  *.fsx                  FSI exploration/test scripts (live API)
tools/                   CLI exe project (depends on hawaii-client)
vendor/Hawaii/           Local fork of Hawaii generator (git submodule)
csv/                     Sample and real CSV files from past runs
requests/                VS Code REST client files for manual HTTP testing
v1-typescript/ … v4-fsharp/  Archived failed attempts — read-only
```

---

## Key Architectural Concepts

### Generated vs. Hand-Written Code

- `hawaii-client/generated/` is fully machine-generated from `api/openapi.json`; never edit it manually.
- All business logic lives in `hawaii-client/src/`.
- `Domain.fs` wraps the generated types into a cleaner domain model.

### Pagination & Streaming

- All list endpoints are paginated (page numbers, not cursors, since the Apr 2026 API update).
- `AsyncSeq.fs` → `paginateByPageSRTP` drives pagination lazily via SRTP constraints.
- `Streams.fs` provides `streamPaginated`, `streamChanges`, `streamPatches`, `streamCreates`.
- Use `AsyncSeq` everywhere; never buffer full lists into memory.

### Full / Patch / Delta Pattern

- API responses have rich *Full* types; PATCH payloads have sparse *Patch* types.
- `PatchShape.fs` uses cached reflection to normalise patch records, strip unchanged fields, and
  detect no-op updates before sending them to the API.
- CSV imports become `*Delta` records (hand-typed subsets of full records); they are diffed against
  current state before generating PATCH calls.

### Authentication

- Token is read from `NOCFO_TOKEN` (or `NOCFO_SOURCE_TOKEN`/`NOCFO_TARGET_TOKEN` for cross-env ops).
- Header format is `Authorization: Token <value>` (NOT `Bearer`).

### CSV Layer

- `CsvHelper` (v33) with semicolon-separated list support.
- `--fields` flag selects which CSV columns to emit or consume.
- Reading: validates headers, maps to typed F# records, ignores unspecified columns.

---

## Build Commands

```bash
# Build everything (from repo root — uses nocfo.sln)
dotnet build

# Run all unit tests
dotnet test

# Build a specific project
dotnet build hawaii-client
dotnet build tools

# Run the CLI
dotnet run --project tools -- <args>

# Run an FSI exploration script
dotnet fsi hawaii-client/TestBalance.fsx

# Regenerate from updated OpenAPI spec
curl -H "Accept: application/vnd.oai.openapi+json;version=3.0" \
  https://api-tst.nocfo.io/openapi/ > api/openapi.json
dotnet ./vendor/Hawaii/src/bin/Release/net6.0/Hawaii.dll \
  --config ./hawaii-client/nocfo-api-hawaii.json --no-logo

# Build the local Hawaii generator (only needed after submodule changes)
dotnet build vendor/Hawaii/src/Hawaii.fsproj -c Release
```

---

## Common CLI Workflows

```bash
# List businesses (with field selection)
dotnet run --project tools -- list businesses --fields "id,name,slug"

# Export accounts to CSV
dotnet run --project tools -- list accounts -b <slug-or-vat> --fields "id,number,name,type"

# Import/update accounts from CSV (idempotent — diffs before patching)
dotnet run --project tools -- update accounts -b <slug-or-vat> --fields "id,number,name" < file.csv

# Cross-environment mapping (maps IDs from source to target)
dotnet run --project tools -- map accounts -b <slug>

# Create documents from CSV
dotnet run --project tools -- create documents -b <slug> < documents.csv
```

Environment variables: `NOCFO_TOKEN`, `NOCFO_BASE_URL` (default tst), and for cross-env:
`NOCFO_SOURCE_TOKEN`, `NOCFO_TARGET_TOKEN`, `NOCFO_SOURCE_BASE_URL`, `NOCFO_TARGET_BASE_URL`.

---

## Source File Compile Order (hawaii-client.fsproj)

F# requires declaration-before-use ordering:

1. `Endpoints.fs` — URL builders
2. `Http.fs` — HTTP client wrapper
3. `AsyncSeq.fs` — Async/Result/AsyncSeq helpers
4. `Streams.fs` — Generic streaming + alignment
5. `PatchShape.fs` — Reflection-based patch normalisation
6. `Domain.fs` — Domain model, hydration, diffing, commands
7. `Reports.fs` — Example fold (trial balance)
8. `CsvHelper.fs` — Custom CsvHelper converters
9. `Csv.fs` — CSV read/write API

---

## Known Limitations & TODOs (as of April 2026)

- **No dry-run mode** — mutations execute immediately.
- **`update businesses` not implemented** (returns TODO exit code).
- **`create accounts/businesses`** not yet implemented.
- **Hawaii fork targets net6.0** (EOL) — upstreaming patches is outstanding work.
- Generated code is **checked in** — regeneration is a manual step when the API spec changes.
- `BusinessIdentifier.type` comes back as a `JToken` (not a closed enum) — string comparison required.

---

## Hawaii Generator Fork

Located at `vendor/Hawaii/` (git submodule).  Local patches add:
- Nullable primitive → `Option<T>` generation (upstream bug)
- Robust enum deserialisation (tolerant converter)
- Operation name normalisation for names with spaces

Do not upgrade Hawaii without verifying these patches still apply or have been upstreamed.

---

## Testing Approach

Two layers:

### 1. xUnit unit tests (`tests/`)

```bash
dotnet test tests
```

Framework: **xUnit** with **Unquote** for assertions (`test <@ expr @>`).
No network access — all tests are pure.

Targets:

- `PatchShapeTests.fs` — `PatchShape.Normalize` and `HasChanges` logic
- `StreamAlignmentTests.fs` — `Streams.alignByKey` merge algorithm
- `DomainDiffTests.fs` — `Account.diffAccount`, `Account.classify`
- `CsvTests.fs` — `Csv.readCsvGeneric` / `writeCsvGeneric` round-trips

**Unquote + `inline` functions:** Unquote cannot dynamically invoke `inline` SRTP
functions via quotation reflection. Pre-compute the result into a `let result = ...`
binding and assert on `result`:

```fsharp
// ✗ fails at runtime — Unquote can't reflect into an inline function
test <@ Account.classify acc = Some Asset @>

// ✓ correct pattern
let result = Account.classify acc
test <@ result = Some Asset @>
```

### 2. Live FSI scripts (`hawaii-client/*.fsx`)

Run against `api-tst.nocfo.io`. Require `NOCFO_TOKEN`.

Safe-to-run (read-only): `TestClient`, `TestPatchShape`, `TestAlignAccountsPermissive`,
`TestCsvReadDeltas`, `TestBalance`.

Mutating: `TestStreams`, `TestAccountUpdates`, `TestCreate*`.
