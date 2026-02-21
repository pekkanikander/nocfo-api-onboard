# NoCFO API Onboarding Experiments

> **Status:** Exploration project. The code works, but the repository will not be maintained.

This project documents five approaches we explored while building a small,
functional-streaming-friendly client for the [NoCFO](https://nocfo.io/) accounting API.
The fifth iteration `hawaii-client/` — which pairs [F#](https://fsharp.org)
and the [Hawaii](https://github.com/pekkanikander/Hawaii) OpenAPI generator —
is the first version we are somewhat satisfied with.
The earlier attempts remain in the repo so future readers
can see what worked, what failed, and why.

If you are evaluating NoCFO, experimenting with functional programming against financial APIs, or comparing OpenAPI toolchains, this repository is intended as a concise field report.

## Repository Tour

- `hawaii-client/` – F# library + scripts using Hawaii-generated types, lazy `AsyncSeq` streams, and a thin domain layer. Start here.
- `api/openapi.json` – The upstream NoCFO OpenAPI document used for generation.
- `tools/` – CSV-first CLI (“nocfo”) built on top of the Hawaii F# client library; see `tools/README.md`.
- `requests/` – Raw HTTP checks (VS Code REST client format) used to validate authentication and pagination manually.
- `v1-typescript/`, `v2-purescript/`, `v3-fsharp/`, `v4-fsharp/` – Earlier experiments (kept for historical context; expect incomplete or abandoned code).
- `vendor/Hawaii/` – Forked generator with small fixes for nullable handling, enum tolerance, and operation name cleanup.
- `LESSONS-LEARNED.md`, `hawaii-client/Domain-design.md`, `v5-fsharp-hawaii.md` – Narrative notes about decisions, trade-offs, and follow-up ideas.

## Quick Start: `nocfo` CLI (tools/)

The CLI in `tools/` is the easiest way to interact with the API. It streams
entities, writes them as CSV, and can reconcile edited rows back to the server.

1. **Prerequisites**
   - .NET 9 SDK (`dotnet --version` ≥ 9.0)
   - macOS or Linux shell (the code itself is cross-platform)
  - `NOCFO_TARGET_TOKEN` (or fallback `NOCFO_TOKEN`) exported; optionally `NOCFO_TARGET_BASE_URL` (defaults to `https://api-tst.nocfo.io`)
  - `NOCFO_SOURCE_TOKEN` only when running dual-environment commands like `map accounts`

   ```bash
   export NOCFO_TOKEN="paste-your-token"
   ```

2. **Build once** (from repo root):
   ```bash
   dotnet build hawaii-client
   ```

3. **List businesses**:
   ```bash
   dotnet run --project tools -- \
     list businesses --fields "id,name,slug" > businesses.csv
   ```

4. **List accounts for a business** (rows ordered by `id`):
   ```bash
   dotnet run --project tools -- list accounts \
     -b <business-id> \
     --fields "id,number,name,type" > accounts.csv
   ```

5. **List documents for a business**:
   ```bash
   dotnet run --project tools -- list documents \
     -b <business-id> \
     --fields "id,number,date,balance" > documents.csv
   ```

6. **Update accounts** by editing the CSV (keep `id` and ordering) and piping it
   back in:
   ```bash
   dotnet run --project tools -- update accounts \
     -b <business-id> \
     --fields "id,number,name" < accounts.csv
   ```

7. **Delete accounts** by piping a CSV with the account `id`s you want to drop:
   ```bash
   dotnet run --project tools -- delete accounts \
     -b <business-id> < ids-to-delete.csv
   ```

8. **Map account IDs between environments** (source -> target by account `number`):
   ```bash
   NOCFO_SOURCE_TOKEN="paste-source-token" \
   dotnet run --project tools -- map accounts \
     -b <business-id> > csv/account-id-map.csv
   ```

9. **Create minimal documents in target**:
   ```bash
   dotnet run --project tools -- create documents \
     -b <target-business-id> \
     --account-id-map csv/account-id-map.csv \
     < csv/documents-create.csv
   ```

### CLI Notes

- `--fields` controls both which columns are emitted and which columns are read back.
  `id` is always required when executing updates or deletes.
- Output defaults to stdout and input defaults to stdin; `--out`/`--in` override
  those streams without shell redirection.
- Currently implemented verbs: `list`, `update accounts`, `delete accounts`, `map accounts`,
  and minimal `create documents`.
- Errors and HTTP traces go to stderr so you can keep piping stdout to files.

See `tools/README.md` for a deeper dive into configuration, CSV expectations,
and the internal architecture.

## Working Directly with `hawaii-client`

If you want to hack on the streaming library itself (e.g., extend the domain
model or write new folds), the various test scripts under `hawaii-client/` may be useful.

1. **Prerequisites** – same as above.
2. **Build**:
   ```bash
   cd hawaii-client
   dotnet build
   ```
3. **Set your token**:
   ```bash
   export NOCFO_TOKEN="paste-your-token"
   ```
4. **Run a script**:
   ```bash
   dotnet fsi TestBalance.fsx
   ```
   The script streams accounts for a demo business, hydrates each account on demand,
   and folds balances by class before printing a trial balance.

Consult `hawaii-client/README.md` for a tour of the modules and guidance on
regeneration, extending AsyncSeq wrappers, or writing new reports.

## Regenerating the Hawaii Client

In the future, we will keep the generator output checked in so you can compile immediately.
At this point, before the first real release, you still need to generate the Hawaii-generated code yourself.
You also need your own Hawaii build.

In general, regeneration is only needed when the NoCFO OpenAPI spec changes.

1. Update `api/openapi.json`.
2. From the repo root, run Hawaii using the curated configuration:
   ```bash
   hawaii --config hawaii-client/nocfo-api-hawaii.json
   ```
3. Rebuild `hawaii-client/` to ensure the generated assembly still compiles.

For a more step-by-step build instructions, see `hawaii-client/README.md`.

## What Our Approach Demonstrates

- **Lazy streaming over paginated endpoints** – `AsyncSeq` wrappers provide pull-based
  iteration over businesses and accounts while preserving the original pagination semantics.
  `Streams.streamBusinesses` and `Streams.streamAccounts` adapt the generated client into domain-friendly shapes.
- **Hydratable domain model** – `Hydratable` discriminated unions keep initial payloads
  lightweight and defer full record loading until needed by folds or reports.
  `Domain.Business` and `Domain.Account` showcase the pattern.
- **Token-based auth done correctly** – All HTTP helpers attach the
  `Authorization: Token <value>` header per request,
   preventing subtle errors with .NET’s default header validation.
- **Thin, testable folds** – Sample scripts (`TestBalance.fsx`, `TestStreams.fsx`)
  emphasize pure folds on top of streams rather than mutable accumulation.

## If You Only Have a Minute

- Skim `LESSONS-LEARNED.md` for the narrative of what we tried and why we settled on Hawaii.
- Open `hawaii-client/TestBalance.fsx` to see how little code is needed to compute a trial balance over the live API.
- Browse `vendor/Hawaii` if you are interested in generator customization for F#.

## Status and Next Steps

We consider this iteration “good enough to remember.”
At this point, future improvements (if someone picks it up) would include:

- Robust error decoding (HTTP + JSON), possibly with retry policies.
- AsyncSeq wrappers for more endpoints (documents, transactions).
- Folding functions for balance sheets and cash-flow reports.
- Upstreaming the Hawaii patches instead of relying on a local fork.

If you use any part of this repo, please do so at your own discretion.
Contributions are welcome, but we may not respond quickly.
