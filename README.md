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
- `requests/` – Raw HTTP checks (VS Code REST client format) used to validate authentication and pagination manually.
- `v1-typescript/`, `v2-purescript/`, `v3-fsharp/`, `v4-fsharp/` – Earlier experiments (kept for historical context; expect incomplete or abandoned code).
- `vendor/Hawaii/` – Forked generator with small fixes for nullable handling, enum tolerance, and operation name cleanup.
- `LESSONS-LEARNED.md`, `hawaii-client/Domain-design.md`, `v5-fsharp-hawaii.md` – Narrative notes about decisions, trade-offs, and follow-up ideas.

## Getting Started (hawaii-client)

1. **Prerequisites**
   - .NET 9 SDK (`dotnet --version` ≥ 9.0).
   - A NoCFO personal access token with API access.
   - macOS or Linux shell (examples assume zsh/bash, but the code itself is cross-platform).

2. **Restore and build**
   ```bash
   cd hawaii-client
   dotnet build
   ```

3. **Provide credentials**
   ```bash
   export NOCFO_TOKEN="paste-your-token"
   ```

4. **Run the trial balance script**
   ```bash
   dotnet fsi TestBalance.fsx
   ```
   The script streams accounts for a demo business, hydrates each account on demand, and folds balances by class before printing a trial balance.

For a more step-by-step walkthrough, see `hawaii-client/README.md`.

## Regenerating the Hawaii Client

We keep the generator output checked in so you can compile immediately. Regeneration is only needed when the NoCFO OpenAPI spec changes.

1. Update `api/openapi.json`.
2. From the repo root, run Hawaii using the curated configuration:
   ```bash
   cd vendor/Hawaii/src
   dotnet run -- --config ../../../hawaii-client/nocfo-api-hawaii.json
   ```
3. Rebuild `hawaii-client/` to ensure the generated assembly still compiles.

Our forked generator (in `vendor/Hawaii`) includes fixes for nullable fields, enum parsing, and operation name normalization that we relied on during November 2025. If you use upstream Hawaii, cross-check that those fixes have landed.

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
