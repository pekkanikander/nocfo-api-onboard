# Hawaii Client (Iteration 5)

> **Status:** Works with the live NoCFO test environment as of November 2025.

This folder contains the fifth iteration of our NoCFO API explorations: an F# façade over Hawaii-generated types with lazy streams, hydratable domain entities, and a handful of F# script sandboxes. It is intentionally lightweight so the patterns are easy to lift into other projects.

## Layout

```
hawaii-client/
├── generated/              # Hawaii output we keep committed for convenience
│   ├── Types.fs            # DTOs (Paginated* etc.)
│   ├── Client.fs           # Auto-generated client (unused directly; we wrap it)
│   ├── OpenApiHttp.fs      # Serializer + tolerant enum helpers
│   └── StringEnum.fs
├── src/
│   ├── Endpoints.fs        # Centralised path builders (v1/business/…)
│   ├── Http.fs             # Token-aware HttpClient wiring
│   ├── AsyncSeq.fs         # Pagination helpers with AsyncSeq<Result<_,_>>
│   ├── Streams.fs          # Low-level streamers over paginated endpoints
│   └── Domain.fs           # Hydratable Business/Account + folds
├── Test*.fsx               # Script sandboxes (streams, balances, etc.)
├── RawHttpTest.fsx         # Minimal reproduction of live HTTP issues
├── Domain-design.md        # Notes on the domain model direction
└── api-spec-test.sh        # Optional spec-vs-server drift signal (Schemathesis + Dredd)
```

Scripts with the `Test*.fsx` prefix expect the compiled library in `bin/Debug` plus the generated DLLs under `generated/bin`. They are small, self-contained experiments rather than polished CLI tools.

## Prerequisites

- .NET 9 SDK (`dotnet --version` ≥ 9.0).
- Access to the NoCFO API and a valid personal access token.
- `NOCFO_TOKEN` exported in your shell.
- The base URL defaults to `https://api-tst.nocfo.io`, the test environment.

Token management portals:
- Test: <https://login-tst.nocfo.io/auth/tokens/>
- (Production: <https://login.nocfo.io/auth/tokens/>)

## Build Once, Then Explore

```bash
cd hawaii-client
dotnet build
export NOCFO_TOKEN="paste-your-token"
```

After the build, the scripts can be run in place with `dotnet fsi <script.fsx>`. Highlights:

- `TestStreams.fsx` – Streams the first few businesses, then accounts, using the `Domain.Streams` wrappers.
- `TestBalance.fsx` – Computes a simple trial balance by hydrating accounts lazily and folding by `AccountClass`.
- `TestClient.fsx` – Raw HTTP smoke test that bypasses the higher-level abstractions.
- `RawHttpTest.fsx` / `STRPTest.fsx` – Repro harnesses from earlier debugging sessions; consult the comments before using.

Some legacy scripts (`TestAsyncSeq.fsx`, etc.) capture older experiments and may need small adjustments if you want to re-run them with the current `Http` module.

## How the Pieces Fit

- `Http.createHttpContext` attaches the `Authorization: Token <value>` header per request and normalises the base URL (`/v1` suffix).
- `AsyncSeqHelpers.paginateByPageSRTP` materialises paginated endpoints as `AsyncSeq<Result<'Item, HttpError>>`.
- `Streams` modules wrap the generator output into our own domain surface, enforcing business scoping and yielding hydratable entities.
- `Domain.Hydratable` defers expensive fetches (Account detail, Business metadata) until explicitly hydrated.
- `Reports.addToTotals` demonstrates how pure folds sit on top of the streaming layer.

Use `Domain-design.md` if you need broader architectural context before changing or extracting code.

## Regenerating the Hawaii Output

We keep the generated code checked in so you can build immediately. Regenerate only when the upstream OpenAPI spec changes.

1. Update `../api/openapi.json` with the latest spec.
2. From `vendor/Hawaii/src`, run the Hawaii CLI against `hawaii-client/nocfo-api-hawaii.json` (see the repository root README for exact commands and the patches we rely on).
3. Rebuild this project (`dotnet build`) to ensure the regenerated DLL still works with the domain layer.

Our forked Hawaii generator includes fixes for nullable Option generation, enum coercion, and operation name cleanup. If you switch to upstream Hawaii, confirm those changes have landed or copy them across.

## Optional: Spec Drift Check

`api-spec-test.sh` can run Schemathesis and Dredd against a live server to spot drift between the spec and production. It is a convenience wrapper; you need `NOCFO_TOKEN` plus local installations (or fallback commands) for Schemathesis, Dredd, `uvx`, and `python3`.

Note that running Schemathesis may pollute you server environment. Be careful.

```bash
./api-spec-test.sh --token "$NOCFO_TOKEN" \
  --spec ../api/openapi.json \
  --base-url https://api-tst.nocfo.io \
  --out ../api/reports/api-spec
```

The script produces a short Markdown summary alongside the raw JUnit/HAR artifacts.

## What’s Next (if you continue)

- Harden error handling in `Http.getJson` (decode errors, retries, structured failures).
- Extend `Domain.Streams` to other endpoints (transactions, documents).
- Introduce property-based regression tests around pagination and idempotent hydration.
- Upstream the Hawaii generator patch set instead of pinning to the local fork.

Until then, treat this folder as a living notebook of the first workable Hawaii-based NoCFO client. Lift ideas, refactor freely, and keep the scripts runnable so future explorers can reproduce the flows quickly.
