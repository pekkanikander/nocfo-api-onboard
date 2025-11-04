## Hawaii AsyncSeq Exploration (v5)

### Context
- Current repo: `nocfo-api-onboard`, OpenAPI source at `api/openapi.json` (large schema with extensive pagination + filtering).
- Previous attempts (PureScript, TypeScript, LLM F#, NSwag C#) highlighted drawbacks: lack of concise functional style, tooling gaps, oversized generated code, poor streaming ergonomics.
- New goal: reuse Hawaii (dotnet CLI) to generate minimal F# client and adapt emitted code to support `AsyncSeq` streaming + lazy paginated pull, with minimal LLM-authored code and a flat project layout.

### Known About Hawaii (10/31/2025)
- Install via `dotnet tool install -g hawaii`.
- Generates full F# projects (`netstandard2.0`) from OpenAPI/OData; removes and recreates target output directory on each run.
- Configuration via `hawaii.json`:
  - Required: `schema`, `project`, `output`.
  - Optional: `target` (`fsharp` or `fable`), `synchronous` (default false), `asyncReturnType` (`"async"` default or `"task"`), `resolveReferences`, `emptyDefinitions`, `overrideSchema`, `filterTags`.
- CLI extras (`hawaii --help`, `hawaii --show-tags`) can inspect available operations.
- No documented first-class support for `AsyncSeq`; default async return type is `Async<'T>` or `Task<'T>`.
- Hawaii emits discriminated unions for endpoint responses and DTO modules for schema objects; historically produces `Client` modules with helpers wrapping `System.Net.Http.HttpClient`.

### Open Questions / Research Targets
- Can Hawaii be configured to emit only specific tags (e.g., `Accounts`, `Documents`) to reduce surface area?
- Does latest Hawaii offer hooks (custom templates, interceptors) to change method signatures (e.g., return AsyncSeq) without post-processing?
- Structure of generated project: file layout, naming, presence of partial modules we can extend without editing generated files.
- Pagination helpers: does Hawaii infer pagination from OpenAPI metadata, or do we compose manual loops?
- Error handling: how are non-2xx responses represented in generated discriminated unions, and how do we integrate them with streaming sequences?
- Tooling compatibility: target framework vs repo baseline, dependency footprint, integration with FAKE/dotnet CLI.

### Immediate Tasks
1. **Environment Prep**
   - Confirm dotnet SDK version on host (`dotnet --info`).
   - Install/upgrade Hawaii tool to latest (`dotnet tool update -g hawaii`).
   - Capture `hawaii --help` and version for reference in docs.

2. **Baseline Generation**
   - Author `hawaii.json` pointing to absolute path `/Users/pnr/Documents/Holotropic/test/nocfo-api-onboard/api/openapi.json`.
   - Choose minimal output directory (e.g., `./hawaii-client`). Ensure clean git-ignore if needed.
   - Run `hawaii` generation and inspect emitted project structure (modules, fsproj, package refs).
   - Record CLI logs + generated file list.

3. **Structural Review**
   - Identify modules that encapsulate HTTP calls (likely `Client.fs`, `Dto.fs`, etc.).
   - Map functions corresponding to paginated endpoints (look for parameters `page`, `page_size`).
   - Document how responses are represented (records, DU for `Either` success/error).

4. **AsyncSeq Integration Experiments**
   - Prototype wrapper module (manual code) that converts repeated `Async<'TPage>` calls into `AsyncSeq<'TItem>` using `AsyncSeq.unfoldAsync` or `AsyncSeq.delay`. Keep this module small and isolated (hand-written or lightly generated with template strings).
   - Investigate possibility of editing Hawaii templates (if repo allows custom templates) vs. post-generation wrappers.
   - Evaluate laziness + backpressure: ensure page fetch triggered only when sequence enumerated.
   - Consider supporting optional `CancellationToken` or timeouts consistent with Hawaii HTTP client design.

5. **Iteration & Testing**
   - For each viable approach (template override, wrapper module, partial module), run small integration tests (FsUnit/xUnit + `AsyncSeq.toListAsync`, limited to 1–2 pages to avoid heavy traffic).
   - Compare ergonomics: signature clarity, need for manual DTO mapping, error propagation semantics.
   - Measure generated code size, DI needs, compile warnings.

### Investigation Backlog / To-Study Items
- Hawaii template customization: check repository docs/issues for `--template-path` or similar feature.
- Binding to `FSharp.Control.AsyncSeq`: ensure NuGet dependency added either by generator or by manual fsproj tweak.
- Options for streaming endpoints returning binary data: confirm interplay with AsyncSeq wrappers.
- Evaluate doc coverage for `resolveReferences` with our multi-file OpenAPI (currently single JSON but references under `components`); confirm we don't need extra preprocess.
- Determine approach for auth header injection (PAT token). Verify how Hawaii handles global headers; ensure wrappers propagate config.

### Risks / Mitigations
- **Large Schema Output**: Generation might produce huge modules; we may need to filter tags or split project to keep diff manageable.
- **Tool Limitations**: If Hawaii cannot alter return types, we rely on manual wrappers; document and keep minimal.
- **AsyncSeq Semantics**: Ensure pagination terminates when `next` link null; handle HTTP errors mid-stream gracefully.
- **Regeneration Safety**: Generated directory wiped on each run—keep custom wrappers outside output (or add post-gen copy step scripted).

### Deliverables
- Generated Hawaii client project checked into repo (possibly under `hawaii-client/`).
- Wrapper module(s) enabling `AsyncSeq` streams for selected paginated endpoints.
- Iteration notes capturing command invocations, observations, and measurement of code size/complexity.
- Minimal tests (e.g., verifying `AsyncSeq` laziness using stubbed HTTP or dry-run with sample data if live API unavailable).
- Updated `LESSONS-LEARNED.md` summarizing insights vs previous attempts (pending after experimentation).

### Next Concrete Step
Proceed with Task 1: verify dotnet toolchain + install/update Hawaii, then capture baseline CLI info before running generation.
