# NoCFO API Tool — Roadmap

Moving from late-exploration to early production quality.
The pattern is settled; this roadmap fills in the gaps that were consciously deferred.

---

## Phase 1 — Solid Foundation (do before anything else)

Goal: make the existing functionality reliable and testable so it is safe to carry forward.

### 1.1 Error handling & retry

File: `hawaii-client/src/Http.fs`

- Add configurable timeout and cancellation token to all HTTP calls
- Return structured error values (distinguish 401, 404, 429, 5xx, JSON parse failure)
  rather than opaque strings — the CLI can then print actionable messages
- Add exponential backoff with jitter for 429 and 5xx responses
  (small helper inside Http.fs; no new library needed)

Why first: everything else depends on reliable HTTP. Without this, any unattended run
against a slightly flaky API is a gamble.

### 1.2 Unit test project for pure modules

New project: `tests/tests.fsproj` (xUnit)

Priority targets (all pure F#, no API access needed):
- `PatchShape` — normalization and HasChanges logic
- `Streams.alignByKey` — alignment of sorted streams
- `Domain` type conversions and delta diffing
- `Csv` — round-trip read/write for typed records

Why now: before migrating to a new repo, we want a test gate that catches regressions.
These tests will also document the invariants of the core library.

### 1.3 Create top level `.fsproj`

Create a top level `.fsproj` file so that both `dotnet build` works (currently it doesn't)
and that running all the tests in `tests` can be done according to the current best F# practices.

### 1.4 Migrate to a new clean repository

The current repo carries four failed iterations (v1-v4), many exploratory FSI scripts,
and a git history that reflects experimentation rather than a product.

What moves to the new repo:
- `hawaii-client/generated/` — generated types and client
- `hawaii-client/src/` — all hand-written source
- `hawaii-client/hawaii-client.fsproj`
- `hawaii-client/nocfo-api-hawaii.json`
- `tools/` — CLI project
- `vendor/Hawaii/` — generator submodule
- `api/openapi.json` — OpenAPI spec
- `tests/` — the new test project from step 1.2
- `requests/` — VS Code REST client files
- `CLAUDE.md`, `ROADMAP.md`, `README.md`, `LESSONS-LEARNED*.md`
- top level `.fsproj` file

What stays behind (this repo becomes the archive):
- `v1-typescript/`, `v2-purescript/`, `v3-fsharp/`, `v4-fsharp/`
- `hawaii-client/*.fsx` exploration scripts (or selectively promote the useful ones)
- `csv/` real data files (sensitive; do not migrate to a public repo)
- `v5-fsharp-hawaii.md` planning notes

Steps:

1. Create a new GitHub repo `nocfo-api-tool`
2. Copy the selected files; do NOT clone the git history
3. Initial commit, with a message like "Bootstrap from the nocfo-api-onboard exploration repo"
4. Update `vendor/Hawaii/` as a submodule in the new repo
5. Verify `dotnet build` and tests pass from scratch

---

## Phase 2 — Usability (make it safe and convenient to run)

### 2.1 Dry-run mode

Flag: `--dry-run` on all mutating commands (`update`, `create`, `delete`, `map`)

Implementation: pass a `dryRun: bool` into `streamChanges`, `streamPatches`,
`streamCreates` in `Streams.fs`; log the intended operation instead of executing it.

Why: this is the single most important safety feature for production use. Without it,
operators cannot preview what a CSV import will change before committing.

### 2.2 Named configuration profiles

File: `~/.config/nocfo/config.toml` (or TOML equivalent)

```toml
[profiles.test]
token = "..."
base_url = "https://api-tst.nocfo.io"

[profiles.prod]
token = "..."
base_url = "https://api-prd.nocfo.io"
```

CLI flag: `--profile <name>` (overrides env vars; env vars still override config file)

Argu supports layered config; this mostly needs plumbing in `Tools.fs` and `ToolConfig`.

Why: juggling multiple env vars for cross-environment operations is fragile and error-prone.

---

## Phase 3 — Distribution (remove the "needs .NET SDK" barrier)

### 3.1 Self-contained binary builds

```bash
dotnet publish tools -r osx-arm64 --self-contained -o dist/osx-arm64
dotnet publish tools -r linux-x64 --self-contained -o dist/linux-x64
dotnet publish tools -r win-x64  --self-contained -o dist/win-x64
```

Before attempting Native AOT: audit `PatchShape.fs` for reflection usage that would
need AOT annotations (`[<DynamicallyAccessedMembers>]` or source generators).

### 3.2 GitHub Actions CI pipeline

Triggers: push to main, pull requests

Jobs:
1. `build` — `dotnet build` for all projects
2. `test` — `dotnet test tests/`
3. `publish` — matrix over three RIDs, upload artifacts to GitHub Release on tag

Why after 3.1: CI is much easier to set up clean in the new repo than to retrofit here.

---

## Phase 4 — Completeness (finish what was deferred during exploration)

### 4.1 Resolve the Hawaii fork

The local fork of Hawaii targets `net6.0` (EOL) and carries three patches.
Options (in order of preference):
1. Open PRs upstream for each patch; depend on a released Hawaii version
2. If upstream is unresponsive: retarget the fork to `net10.0`
3. Evaluate whether the generated code surface still justifies a generator at all
   (the API is larger now, so probably yes)

### 4.2 Complete missing CLI commands

Currently unimplemented or stubbed:
- `update businesses` (returns TODO exit code)
- `create accounts`
- `create businesses`
- `delete` commands for most entity types

### 4.3 Structured logging / observability

- Ensure all progress/status output goes to stderr; data output to stdout (partially done)
- Add `--verbose` flag for HTTP-level debug output (request/response bodies, timing)
- Optional: `--log-format json` for machine-readable stderr (useful when scripting)

---

## Phase 5 — Broader API Coverage (when the above is stable)

The API has grown to 73 paths / 159 schemas. Currently covered: businesses, accounts,
contacts, documents (list/update). Not yet covered:

- Transactions (read-only streaming, useful for reports)
- Accounting periods
- VAT returns
- Attachments (multipart upload — needs special handling)
- Balance sheet / P&L report folds (a `Reports.fs` stub exists)

Prioritise based on user need, not completeness for its own sake.

---

## Sequencing Summary

```
Phase 1: Http errors → Unit tests → New repo
Phase 2: Dry-run → Config profiles
Phase 3: Self-contained binary → CI
Phase 4: Hawaii → Missing commands → Logging
Phase 5: Broader API coverage
```

Phases 1–3 are the path to "early production quality tool".
Phases 4–5 are "mature tool".

---

## Architectural Direction: SRTP over Code Generation

### The option considered and set aside

A "super-Hawaii" generator was considered: a tool that reads an OpenAPI spec and emits a
complete F# CLI with CSV import/export for all entity types, replacing most of the
hand-written code in this repo. Several architectures were evaluated:

- **Extend Hawaii directly** — single tool, but pollutes the generator with two concerns;
  makes upstreaming patches harder; Hawaii is already on an EOL runtime
- **Pipeline (Hawaii + second-pass CLI generator)** — clean separation; Hawaii unchanged;
  new generator emits `Streaming.fs`, `Arguments.fs`, `Dispatch.fs` per entity
- **Full standalone generator** — maximum control, but reimplements everything Hawaii
  already handles (nullable types, enum robustness, JSON quirks)
- **Template-based rendering** — good for simple scaffolding, awkward for type-relationship
  inference; best as a rendering layer within one of the above, not a primary architecture
- **Runtime interpretation** — no code generation, dynamic CLI from config; loses Argu's
  compile-time type safety and makes debugging harder

The pipeline approach (Option B) was the strongest technically: Hawaii handles the hard
API-layer problems; a new tool handles CLI generation; static runtime files (Http.fs,
AsyncSeq.fs, Streams.fs, PatchShape.fs, Csv.fs) are checked in and never generated.
An annotation file alongside the OpenAPI spec would capture what OpenAPI cannot express
(tenant scope parameter, identity keys, CSV exclusions, custom type converters).

### Why it was set aside

The generator is only worth building if it will be applied to multiple different APIs.
For a NoCFO-specific tool the upfront cost outweighs the ongoing savings — especially
since the hand-written code is already small and well-structured.

### The chosen direction instead: SRTP and generic functions

The codebase should evolve toward maximally generic F# using SRTP and `inline` functions,
so that adding a new entity type costs almost nothing — just a type definition and a few
lines of wiring. `paginateByPageSRTP` in `AsyncSeq.fs` is the established proof of concept.

Target end state:

- `Http.fs`, `AsyncSeq.fs`, `Streams.fs`, `PatchShape.fs`, `Csv.fs` — fully generic,
  no entity-specific code
- `Domain.fs` — type definitions and the minimal entity-specific logic that SRTP cannot
  absorb (e.g. semantic field interpretation)
- `Arguments.fs` — NoCFO-specific CLI structure
- `Program.fs` — small orchestrator

Next candidates for genericisation (in rough priority order):

1. Per-entity streaming wiring currently duplicated across entities in `Domain.fs`
2. CSV field mapping in `Csv.fs`
3. `PatchShape.fs` — reflection is acceptable to keep; SRTP for record fields is verbose
   enough that the reflection approach remains more readable

This approach delivers most of the code-generation benefit (new entity = trivial addition)
without building or maintaining a generator.
