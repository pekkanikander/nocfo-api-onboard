# F# Research (2024–2025)

_Last updated: 27 Aug 2025_

## Executive summary
F# is stable, modern, and production‑ready on .NET 8/9 (with preview features in .NET 10). The current stable language is **F# 9** (ships with .NET 9), and **F# updates for .NET 10** are available under `<LangVersion>preview</LangVersion>` in 2025. Tooling (VS Code + Ionide, Rider, Visual Studio) is mature. For web backends you can use **Giraffe/Saturn/Falco** atop ASP.NET Core; for the browser you can use **Fable** (F#→JS/TS) or **Bolero/Blazor WASM**. For streaming, **AsyncSeq** and **TaskSeq** provide high‑level pull‑based async sequences with familiar `map/filter/fold` combinators. Testing is well served by **Expecto**, **FsUnit**, and **FsCheck**. If you like a cohesive FP‑first stack, the **SAFE Stack** (Saturn/Giraffe + Fable + Elmish + Azure) remains relevant.

Key references: .NET “What’s new” and language updates, F# releases, Fable docs, Giraffe docs, AsyncSeq/TaskSeq, Ionide and Rider docs. citeturn12search5turn12search8turn13search0turn4search1turn14search0turn14search1turn20search0turn20search1

---

## 1) Language & ecosystem
- **Current version**: F# 9 (Nov 2024, with .NET 9). citeturn12search15
- **Next**: F# updates for .NET 10 (2025) are available as preview features; they become default in .NET 10 projects when released. citeturn12search5
- **Active development**: Ongoing compiler/tooling releases in `dotnet/fsharp`; .NET blog and release notes show regular cadence. citeturn12search8turn12search3
- **Core FP features**: algebraic data types (discriminated unions), pattern matching, type inference, records, computation expressions, pipelines, units of measure, etc. (F# 9 adds nullable reference type support, DU `.Is*` properties, new collection ops). citeturn12search11turn12search13
- **Compared to Haskell/PureScript**: F# is pragmatic/industrial with seamless .NET interop and stable tooling; fewer kind‑level features than Haskell, but ADTs + type providers + CE’s make enterprise FP pleasant. (Primary sources: MS docs + F# 9 materials.) citeturn12search12turn12search11

## 2) Development tools & tooling
- **Build**: .NET SDK/CLI (`dotnet build/test/publish`). For scripted builds use **FAKE** (F# Make). citeturn21search1turn18view0
- **Packages**: **NuGet** by default; **Paket** provides precise lockfiles, groups, and git/HTTP refs (popular in F#). citeturn19search11turn19search0
- **IDE support**:
  - **VS Code**: **Ionide** is the de‑facto plugin (language server, project system, FSI). citeturn20search0
  - **JetBrains Rider**: first‑class F# plugin with ongoing improvements (2025.x). citeturn20search1turn20search3
  - **Visual Studio**: built‑in F# workloads. citeturn12search12
- **Project templates**: `dotnet new console -lang F#`, `dotnet new classlib -lang F#`; community templates include `dotnet new giraffe`, `dotnet new saturn`. citeturn21search2turn21search0turn21search6

## 3) JavaScript/Web compilation
- **Fable status**: Production‑quality F#→JavaScript compiler. Since **Fable 4**, can also target **TypeScript, Rust, Python, Dart** (multi‑backend). citeturn13search0
- **JS output**: Idiomatic ES modules; you typically bundle with Vite/Webpack. Community prefers **Feliz** for React, and **Elmish** for MVU. citeturn13search1
- **WASM**:
  - **Bolero** enables F# on **Blazor WebAssembly** (F# compiled to .NET/WASM). MS docs list F# options for web dev and link to Bolero. citeturn21search8
  - Fable itself targets JS/TS (not WASM), but runs anywhere JS runs (e.g., Cloudflare Workers). citeturn13search0

## 4) Backend & cloud
- **ASP.NET Core**: Fully supported from F#. For functional style use **Giraffe** (HTTP combinators) or **Saturn** (higher‑level MVC atop Giraffe). citeturn4search1turn21search8
- **Serverless**:
  - **Azure Functions**: Use the **.NET isolated worker** model (the in‑process model is being retired; isolated supports .NET 8/9). F# works in isolated worker (it’s just a .NET console host). citeturn22search5turn22search0turn22search3
  - **AWS Lambda**: Use the .NET runtime; F# works as any .NET assembly. citeturn11search3
- **Performance**: Same CLR/JIT as C#; F# web apps on Kestrel (e.g., with Giraffe) achieve competitive throughput. (See community perf write‑ups; general .NET perf applies.) citeturn22search12

## 5) Testing & workflow
- **Unit/Integration**: **Expecto** (async + parallel by default), **xUnit/NUnit** with **FsUnit** syntax enhancements. citeturn15search0turn15search4
- **Property‑based**: **FsCheck** integrates with Expecto/xUnit/NUnit. citeturn15search2
- **Build/CI**: `dotnet test` + GitHub Actions (`actions/setup-dotnet`) + FAKE tasks. citeturn21search1
- **Debugging**: VS Code (Ionide + C# debugger), Rider, Visual Studio—all support breakpoints, FSI, test runners. citeturn20search0turn20search1

## 6) Financial/Accounting & data/streaming
- **Math/statistics**: **Math.NET Numerics**; interoperates cleanly with F#. citeturn17search2
- **Data access & type providers**: **FSharp.Data** (CSV/JSON/XML providers, HTTP helpers). citeturn16search0
- **Data frames**: **Deedle** (series/frames), suitable for ETL/exploration. citeturn16search2
- **Time**: **NodaTime** for correct time‑zones and calendars; JSON/System.Text.Json support packages available. citeturn16search1turn16search17
- **Excel/CSV**: ExcelProvider (read), FsExcel (generate) if needed. citeturn16search7turn16search23
- **Streaming**:
  - **FSharp.Control.AsyncSeq** – pull‑based async sequences with `map/choose/filter/fold` et al; cancellation and composition included. citeturn14search0turn14search4
  - **FSharp.Control.TaskSeq** – `taskSeq {}` over `IAsyncEnumerable<'T>` with rich combinators (`mapAsync`, `filterAsync`, `takeWhileAsync`, …). citeturn14search1turn14search3
- **Charts**: **Plotly.NET**, **ScottPlot**, or **XPlot**. citeturn17search0turn17search1turn17search2

## 7) Deployment options
- **Web apps**: `dotnet publish -c Release` → self‑contained Linux container on Kestrel/ASP.NET Core; reverse proxy with Nginx. Templates exist for Giraffe/Saturn. citeturn21search0
- **Desktop**: WPF/WinForms (Windows); cross‑platform **Avalonia** (not cited here), MAUI (C#/XAML first‑class, F# usable). General .NET docs apply. citeturn12search12
- **Mobile**: MAUI/Xamarin bindings (F# works where .NET works). (General docs.) citeturn12search12
- **Docker**: Standard .NET containerisation (`mcr.microsoft.com/dotnet/aspnet:9.0` etc.). (General .NET docs.) citeturn12search12

## 8) Learning resources
- **Official docs**: Microsoft Learn – F# guide; “What’s new” for .NET; web development scenarios. citeturn12search12turn12search2turn21search8
- **Community**: F# Software Foundation site, Ionide, SAFE Stack docs, Compositional IT blog, F# for Fun and Profit (testing/property‑based series). citeturn20search6turn15search8
- **Recent books/courses**: (Ecosystem light; most classics still relevant. Prefer docs + community patterns.)

## 9) Migration paths
- **From TypeScript**: For browser apps use **Fable** (F#→JS/TS). Elmish/Feliz provide React MVU; code can live alongside TS gradually. citeturn13search0turn13search1
- **From C#**: Same runtime and libraries; call C# from F# and vice‑versa. ASP.NET Core, EF Core, Serilog, etc. interoperate.
- **From Haskell/PureScript/OCaml**: You get ADTs, pattern matching, immutable‑first design; fewer type‑level features than Haskell, richer industrial interop than PureScript. (Use Fable or Bolero for front‑end.) citeturn13search0turn21search8

## 10) Real‑world usage
- **Notable use**: NVIDIA technical blog showcases F# for data wrangling in .NET notebooks; long‑standing enterprise usage in finance, e‑commerce, and consultancy (Compositional IT, SAFE community). (Representative sources.) citeturn10search1

---

## Specific questions for our use case
We want an **Accounting/Bookkeeping system** with REST API, **stream‑based data processing**, both **backend and frontend**, with **TDD**.

### 1) Is F# a good choice?
Yes. You get strong domain modelling (records/DU), excellent composition, high‑quality async/streaming abstractions (AsyncSeq/TaskSeq), and .NET‑grade performance and ops story. For browser UIs, Fable is robust; for backends, Giraffe/Saturn/Falco sit on ASP.NET Core. Azure Functions (isolated) works for serverless. citeturn14search0turn14search1turn4search1turn21search8turn22search5

### 2) Recommended project structure
Monorepo with clear boundaries. Example:
```
repo/
  build/                       # FAKE build scripts
  src/
    Domain/                    # Pure types + business rules
    Accounting.Api/            # Giraffe/Saturn HTTP API
    Accounting.Tasks/          # AsyncSeq/TaskSeq pipelines
    Accounting.Storage/        # DB access (SQL/NoSQL) + migrations
    Accounting.Frontend/       # Fable + Elmish + Feliz
  tests/
    Domain.Tests/              # Expecto/FsCheck unit + property tests
    Api.Tests/                 # Expecto integration tests
  paket.dependencies           # (optional) if using Paket
  global.json                  # pin SDK
  Directory.Build.props        # shared settings
```

### 3) How well does F# handle streaming/async data?
Very well. Use **AsyncSeq** when you want pull‑based async sequences with `map/choose/filter/fold` and cancellation; use **TaskSeq** for first‑class `IAsyncEnumerable<'T>` interop (`taskSeq { ... }` with `let!`, `yield!`). Both give you a high‑level functional API similar to Unix‑pipe style, without back‑pressure foot‑guns of push‑only Observables. citeturn14search0turn14search4turn14search1

### 4) Testing approach
- **Unit**: Expecto or xUnit/NUnit + FsUnit syntax.
- **Property‑based**: FsCheck wired into Expecto/xUnit.
- **Integration**: Host the API in‑memory (ASP.NET Core test host) and test via HTTP; run test suites in parallel (Expecto defaults). citeturn15search0turn15search4turn15search2

### 5) Libraries to consider
- HTTP/API: **Giraffe** or **Saturn** (on ASP.NET Core). citeturn4search1turn21search6
- Serialization: `System.Text.Json` (+ NodaTime converters if you use NodaTime). citeturn16search17
- Data processing: **FSharp.Data**; **Deedle** if you need frames/time series. citeturn16search0turn16search2
- Time: **NodaTime**. citeturn16search1
- Streaming: **AsyncSeq**, **TaskSeq**. citeturn14search0turn14search1
- Frontend: **Fable** + **Elmish/Feliz**; or **Bolero** for WASM. citeturn13search0turn21search8
- Testing: **Expecto**, **FsUnit**, **FsCheck**. citeturn15search0turn15search4turn15search2
- Charts: **Plotly.NET** or **ScottPlot** for reports/dashboards. citeturn17search0turn17search1

---

## Getting started – concrete steps
1) **Scaffold**
```bash
dotnet new sln -n Accounting
# API
dotnet new giraffe -o src/Accounting.Api
# Pipelines lib
dotnet new classlib -lang F# -o src/Accounting.Tasks
# Domain
dotnet new classlib -lang F# -o src/Domain
# Tests
dotnet new console -lang F# -o tests/Domain.Tests
```
citeturn21search0

2) **Wire packages** (NuGet or Paket). Example with NuGet:
```bash
cd src/Accounting.Tasks
 dotnet add package FSharp.Control.AsyncSeq
 dotnet add package FSharp.Control.TaskSeq
cd ../Accounting.Api
 dotnet add package Giraffe
```
citeturn14search0turn14search1

3) **Add a stream** (pull‑based, one JSON object at a time):
```fsharp
open FSharp.Control

let fetchPages (getPage : int -> Async<string list>) : AsyncSeq<string> =
  AsyncSeq.unfoldAsync (fun page -> async {
    let! items = getPage page
    return if List.isEmpty items then None else Some (Seq.ofList items, page + 1)
  }) 1

let process =
  fetchPages getPage
  |> AsyncSeq.map jsonDecode
  |> AsyncSeq.filter isValid
  |> AsyncSeq.map projectToDomain
  |> AsyncSeq.iter persist
```
(Replace `getPage`, `jsonDecode`, `persist` with your functions.) citeturn14search0

4) **Tests** (Expecto + FsCheck):
```fsharp
open Expecto
open FsCheck

[<Tests>]
let tests =
  testList "domain" [
    testProperty "double reverse is identity" <|
      fun (xs:int list) -> xs |> List.rev |> List.rev = xs
  ]

[<EntryPoint>]
let main argv = Tests.runTestsInAssembly defaultConfig argv
```
citeturn15search0turn15search2

5) **Frontend** (Fable + Feliz + Elmish) – optional
```bash
# from repo root
mkdir -p src/Accounting.Frontend && cd $_
dotnet new fable -n Accounting.Frontend  # or follow Fable + Feliz guide
```
Then wire Elmish/Feliz per the getting‑started guides. citeturn13search1

6) **Serverless** (Azure Functions, isolated worker):
```bash
dotnet new azfunc --worker-runtime dotnetIsolated -n Accounting.Func
```
Write functions in F# by creating an F# project that references the worker packages (isolated = regular .NET console host). Use HTTP triggers for webhooks/ingest. citeturn22search0turn22search2

---

## Notes & caveats
- Prefer **TaskSeq** when integrating with `IAsyncEnumerable<'T>` ecosystems; prefer **AsyncSeq** when you want CE‑friendly `Async` composition and cancellation.
- For browser‑side code on **Cloudflare Workers**, prefer **Fable** (the output is JS/TS). WASM (Bolero) runs in browsers but not in Workers’ V8 isolate environment.
- For time/accounting logic, **NodaTime** prevents the classic `DateTime` bugs (time zones, DST). citeturn16search1turn16search13

## Appendix: Template commands
- List installed templates: `dotnet new list`
- Install Giraffe template: `dotnet new --install Giraffe.Template`
- Install Saturn template: `dotnet new --install Saturn.Template`
- Create a new Saturn app: `dotnet new saturn -lang F#`
citeturn21search12turn21search0turn21search6
