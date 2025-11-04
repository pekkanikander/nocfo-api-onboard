# Lessons Learned - NOCFO API Exploration Project

## Project Overview

This project explores the **NOCFO API** - a Finnish accounting and bookkeeping system - to understand how to build functional, stream-based applications for financial data processing.  More generally, if feasible, the same programmnig approaches should be applicable also to other APIs, such as the PSD2 or Enable Banking APIs.

## Project Goals

### Primary Objective

Explore the possibilities for extending a working accounting system with extensions that are based on **a functional programming, stream-based approach** that can efficiently process financial data, e.g. from the NOCFO API, using modern functional programming techniques.

The goal is to be able to let an LLM to generate and a human to write minimal, high-level code that packs a lot with minimal amount of code.

In the best possible world, this would enable a genre of high-level open source accounting tools that might transform the accounting industry by  providing high-level, simple open source tools.  However, this is — of course — very unlikely.

## Programming Approaches Explored

### Initial Attempts
1. **TypeScript with Custom Streams**: Built working streaming abstractions but wanted more functional approach
2. **PureScript**: Attempted but failed due to LLM competence limitations
3. **F#**: Successfully implemented and initially working, but decided to move on using tools
4. **F# with OpenAPI Generator and nswag**: Failed, mainly due to complexities caused with integrating C#
5. **F# with Hawaii**: Partial success so far (as of Nov 4)

### F# Implementation
- **Domain Types**: Business, Account, AccountType with proper Finnish accounting categories
- **Streaming**: AsyncSeq-based streaming working with real API data
- **TDD Approach**: Tests validating real API integration
- **Console Tool**: Working demonstration of streaming functionality

## What We Know About NOCFO API

### High-Level Understanding
- **Purpose**: Finnish accounting and bookkeeping system
- **Environment**: Test environment available at `https://api-tst.nocfo.io`
- **Authentication**: Token-based (not Bearer) with format `Authorization: Token {token}` (stored as NOCFO_TOKEN in `.env`)
- **Data Structure**: RESTful API with pagination (`results` array, not `data`)
- **Business Focus**: Handles businesses, accounts, transactions, and financial data

### API Characteristics
- **Finnish Accounting Standards**: Supports Finnish chart of accounts and VAT configurations
- **Business Entities**: Multiple businesses can be managed (we found 2 in test env)
- **Rich Metadata**: Extensive business configuration including VAT rates, currency, language settings
- **Structured Responses**: Well-defined JSON schemas with consistent patterns

## Current Status

### What We've Built
- **Domain Layer**: Core business types and logic
- **API Layer**: HTTP client with streaming capabilities
- **Tool Layer**: Console application demonstrating functionality
- **Test Layer**: End-to-end validation of real API integration

### What Works so far
- ✅ Real HTTP API calls to NOCFO
- ✅ Authentication and data retrieval
- ✅ AsyncSeq streaming of business data
- ✅ Comprehensive test suite
- ✅ Clean project architecture

## Key Technical Insights

### API Integration Lessons

- Some of the current LLMs used seem to be making a lot of simplistic errors
  - This may require more testing with different LLM models
  - As an example, the Header formats really matter (Token vs Bearer, capitalization) but the LLMs tend not to be too careful here
  - As another example, the LLMs used tended to "forget" URL structure details, e.g. trailing slashes
- The LLM assumptions about e.g. JSON response structures can be wrong

### Tooling Insights (OpenAPI vs NSwag)
- OpenAPI Generator (v7.16.0) does not provide an F# client generator; `fsharp-functions` is a server generator (Azure Functions), not suitable for client code in this project.
- For F# client consumption, NSwag can generate a C# client (`NocfoClient.cs`) and DTOs which F# can reference directly, preserving minimalism.
- NSwag’s default output is a single large C# file (~11k LOC) whose DTOs mirror the full schema (including optional params like `tags`), so we must isolate it in its own project/reference and accept that the generator exposes every field from the spec.

In general, the LLM assistant needs to be vigilent about such details, as the learning sets don't seem to emphasise such vigilence enough.

### F# Strengths
- Strong type system prevents runtime errors
- AsyncSeq provides excellent streaming abstractions
- .NET tooling integration is straightforward
- Good error messages and debugging support

### LLM Competence Reality
- PureScript was beyond current training, leading to very inefficient and bug rich coding
- F# seems to be on a right level of complexity for the current LLM models
  - LLMs assistants do make errors (see above) but are relatively efficient fixing them
- Language choice significantly impacts productivity
  - e.g. LLMs are much stronger in TypeScript, but tend to produce a lot of code — too much for a human to review easily

## Next Exploration Areas

### Potential Directions
1. **Account Streaming**: Fetch and stream accounts for each business
2. **Transaction Processing**: Handle financial transactions with streaming
3. **Complex Stream Operations**: Filtering, mapping, aggregation
4. **Other APIs**, such as Enable Banking or directly PSD2
5. **REST API Building**: Giraffe/Saturn web framework integration
6. **Error Handling**: Production-ready error handling and retry logic

## Hawaii + NOCFO API (Nov 2025) — Lessons Learned

- **Toolchain-first approach**: Prefer fixing generators/tooling over maintaining preprocessors/workarounds when feasible. Temporary workarounds are acceptable only if fixes are too costly; fixes should be upstreamed where possible.
- **Hawaii fixes we implemented**:
  - **Nullable semantics**: Hawaii previously ignored `property.Nullable` when the field was listed as `required`. We changed the required check to `required && not Nullable` so nullable primitives (e.g., `period_id`, `vat_period_id`) become `Option<T>`.
  - **Enum deserialization robustness**: Added a tolerant converter in `HttpLibrary.fs` that normalizes incoming strings and maps to F# DU cases (works with the minimal `generated/StringEnum.fs` stub). This avoids brittle behavior with values like `y_tunnus`, `zero`, `reduced_a`.
  - **Operation name normalization**: Updated `cleanOperationName` to treat space as a separator and drop whitespace-only parts. This removes double spaces and yields clean PascalCase member names without preprocessing.
- **OpenAPI schema deviations and overrides**:
  - Live API returns `prev` (not `previous`) in paginated responses. We added `prev` to all `Paginated*` schemas via `overrideSchema` (kept `nullable: true`, `format: uri`).
  - Some integer fields can be `null` at runtime even if marked required; with the Hawaii fix above, `nullable: true` now generates `Option<int>` as expected. The extra `size` field in responses is ignored safely by the deserializer.
- **Operational notes (Hawaii)**:
  - Location: `vendor/Hawaii` (as a git submodule)
  - Build (for macOS): `dotnet publish ./src/Hawaii.fsproj -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true`
  - Run: `<repo>/vendor/Hawaii/src/bin/Release/net6.0/osx-arm64/publish/Hawaii --config ./nocfo-api-hawaii.json`
  - With the fixes in place, we no longer needed any preprocessing step; config `overrideSchema` handled API deviations.
- **API integration notes**:
  - Auth header is `Authorization: Token <token>` (not Bearer). Raw `HttpClient` tests were invaluable to confirm headers and payloads. The LLM didn't notice this first.
  - Be defensive with pagination fields: treat `results` as potentially `null` and default to empty list.
- **Status**: First reasonable Hawaii-based F# client achieved with minimal custom code and a flat structure. We haven’t yet reproduced the TS/PureScript/pure F# (without tools) functionality (listing accounts and computing balances) but are set up to proceed cleanly.
- **Next**:
  - Add lazy paged pull (AsyncSeq) wrappers on top of generated calls.
  - Implement account listing + balance computation.
  - Consider upstream PRs to Hawaii (nullable fix, operationId normalization, tolerant enum converter).

  ## Progress history ##

* August 28, 2025: First version of this document created, with minimal testing of TypeScript, PureScript and F# tested
* November 4, 2025: Second version of this document, with partual success using F# and Hawaii

---

*This document captures the high-level lessons learned during the initial exploration phase. Details and specific technical insights will be added as we continue development.*
