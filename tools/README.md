# `nocfo` CLI (tools/)

Thin Argu-based CLI that exercises the `hawaii-client` library (without touching
the F# scripts there.)

Treat this as the user-facing path for listing businesses/documents/accounts and updating/deleting accounts via CSV.

## Quick start

```bash
dotnet build ../hawaii-client
export NOCFO_TOKEN="paste-your-token"
export NOCFO_SOURCE_TOKEN="paste-your-prod-token"   # needed for `map accounts`

dotnet run --project tools -- list businesses \
  --fields "id,name,slug" > businesses.csv

dotnet run --project tools -- list accounts \
  -b <business-id> --fields id number name type > accounts.csv

dotnet run --project tools -- list documents \
  -b <business-id> --fields "id,number,date,balance" > documents.csv

# edit accounts.csv keeping rows ordered by `id`
dotnet run --project tools -- update accounts \
  -b <business-id> --fields "id,number,name" < accounts.csv

# map account IDs across environments by account number
dotnet run --project tools -- map accounts \
  -b <business-id> > csv/account-id-map.csv

# create documents in target environment from minimal CSV
dotnet run --project tools -- create documents \
  -b <target-business-id> \
  --account-id-map csv/account-id-map.csv \
  < csv/documents-create.csv
```

Requirements:

- .NET 9 SDK
- `NOCFO_TARGET_TOKEN` (preferred) or fallback `NOCFO_TOKEN` (required)
- `NOCFO_TARGET_BASE_URL` (optional), fallback `NOCFO_BASE_URL`, default `https://api-tst.nocfo.io`
- `NOCFO_SOURCE_TOKEN` (required for `map accounts`)
- `NOCFO_SOURCE_BASE_URL` (optional, default `https://api-prd.nocfo.io`)
- Build artifacts from `hawaii-client` (`dotnet build hawaii-client`)

## Command surface

| Command | Description | Notes |
| --- | --- | --- |
| `list businesses [--fields …]` | Streams every business the token can access and writes CSV | Default columns are the DTO fields; use `--fields` to select a subset |
| `list accounts -b <id> [--fields …]` | Resolves the business (Y-tunnus or VAT code), streams accounts, hydrates them, writes CSV ordered by API `id` | Rows are emitted in ascending `id` order |
| `list documents -b <id> [--fields …]` | Resolves the business (Y-tunnus or VAT code), streams documents, hydrates them, writes CSV | Documents are currently list-only in the CLI |
| `update accounts -b <id> [--fields …]` | Reads CSV from stdin (or `--in`), aligns each row by `id`, emits PATCH commands for changed fields only | CSV **must** include `id` and remain ordered to match the streamed accounts |
| `delete accounts -b <id>` | Reads a CSV containing `id` values and issues DELETE calls sequentially | Extra columns are ignored |
| `delete documents -b <id>` | Reads a CSV containing `id` values and issues DELETE calls sequentially | Extra columns are ignored |
| `map accounts -b <id>` | Resolves source+target business contexts and emits `source_id,target_id,number` matches by account `number` | Prints missing-target warnings to stderr; exit code is `EX_DATAERR` when warnings exist |
| `create documents -b <id> [--account-id-map <path>] [--strict] [--fields …]` | Reads minimal document-create CSV and POSTs documents sequentially | Optional blueprint account-id remap via mapping CSV |

Unimplemented (exit code `1` with a TODO):

- `update businesses`
- `update documents`
- `create accounts` / `create businesses` (alignment currently assumes “desired state” rather than inserts)

## CSV expectations

- `--fields` accepts top-level DTO property names as comma-separated and/or space-separated input (for example `--fields "id,name"` or `--fields id name`). The same selection applies to both output and input.
- `id` is always required when reading updates or deletes; `Program.fs` prepends it for you even if `--fields` omits it.
- Account rows must remain ordered by `id`. `Account.deltasToCommands` aligns the live stream with the CSV by walking both sequences in lockstep; reordering breaks alignment.
- Collections of strings are stored as `;`-separated lists. `option<_>` values use empty cells for `None`.
- Extra columns in the input are ignored when `--fields` is present; otherwise we validate that every header maps to a property.

Use `--out`/`--in` if you prefer explicit file paths over shell redirection.

## Configuration and runtime

`Nocfo.Tools.Runtime.ToolConfig` reads the environment and builds a `ToolContext`:

- Target/default context:
  - `NOCFO_TARGET_TOKEN` (fallback: `NOCFO_TOKEN`)
  - `NOCFO_TARGET_BASE_URL` (fallback: `NOCFO_BASE_URL`, default `https://api-tst.nocfo.io`)
- Source context (used by dual-environment commands):
  - `NOCFO_SOURCE_TOKEN`
  - `NOCFO_SOURCE_BASE_URL` (default `https://api-prd.nocfo.io`)

The context wraps the shared `Http.createHttpContext` and `Accounting.ofHttp` from
`hawaii-client`, plus the active stdin/stdout handles.

## Internals

- **Arguments** (`Arguments.fs`): Argu discriminated unions define `list`, `update`, `delete`, and nested subcommands (`businesses`, `accounts`, `documents`). `--fields` and `--format` live at the entity level.
- **Runtime + Streams** (`Tools.fs`): resolves env vars, builds `AccountingContext`, and routes CSV readers/writers.
- **CSV helpers** (`CsvHelper.fs`): bridges CsvHelper with F# records, ensuring the CLI and scripts share the same converters.
- **Program flow** (`Program.fs`):
  - `list` commands: stream via `Streams.streamBusinesses`, `Streams.streamAccounts`, or `Streams.streamDocuments`, hydrate rows (`Streams.hydrateAndUnwrap`), write CSV lazily.
  - `update` accounts: read CSV into `AsyncSeq<Result<PatchedAccount,_>>`, align with live accounts using `Account.deltasToCommands`, execute sequentially, print per-account status.
  - `delete` accounts: map CSV rows to `AccountCommand.DeleteAccount` and reuse the same execution + folding machinery.
  - `map accounts`: align source/target account streams by `number` and output `source_id,target_id,number`.
  - `create documents`: read minimal create payload rows, optionally rewrite blueprint account IDs, then POST documents sequentially.

Everything runs on `AsyncSeq`, so listing scales to large datasets without holding them all in memory.

## Limitations / future work

- No retries or backoff; transient HTTP failures halt the stream.
- No dry-run mode—`update` and `delete` execute immediately once the CSV is read.
- Business updates and account creation are placeholders.
- Alignment requires sorted CSV rows; future work could introduce a keyed lookup to relax that constraint.
- `--format` is hard-wired to CSV; JSON or Parquet would require additional mapping.
- Packaging as a standalone `nocfo` binary is planned but not implemented.

See `../hawaii-client/README.md` for deeper implementation notes and ideas for extending the domain model.
