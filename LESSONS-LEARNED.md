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
3. **F#**: Successfully implemented and working

### F# Implementation Success
- **Domain Types**: Business, Account, AccountType with proper Finnish accounting categories
- **Streaming**: AsyncSeq-based streaming working with real API data
- **TDD Approach**: Tests validating real API integration
- **Console Tool**: Working demonstration of streaming functionality

## What We Know About NOCFO API

### High-Level Understanding
- **Purpose**: Finnish accounting and bookkeeping system
- **Environment**: Test environment available at `https://api-tst.nocfo.io`
- **Authentication**: Token-based (not Bearer) with format `Authorization: Token {token}`
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

## Progress history ##

* August 28, 2025: First version of this document created, with minimal testing of TypeScript, PureScript and F# tested

---

*This document captures the high-level lessons learned during the initial exploration phase. Details and specific technical insights will be added as we continue development.*
