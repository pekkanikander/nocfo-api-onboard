module Domain.Tests

open System
open Expecto
open FsCheck
open Accounting.Api

[<Tests>]
let tests =
    testList "Domain Types" [
        testList "Business" [
            testCase "should create a Business with all required fields" <| fun _ ->
                let business = Domain.Business.create 1 "holotropic" "Holotropic Oy" "1234567-8" "FI_YHD"

                Expect.equal business.Id 1 "Business ID should match"
                Expect.equal business.Slug "holotropic" "Business slug should match"
                Expect.equal business.Name "Holotropic Oy" "Business name should match"
                Expect.equal business.BusinessId "1234567-8" "Business ID should match"
                Expect.equal business.Form "FI_YHD" "Business form should match"
        ]

        testList "Account" [
            testCase "should create an Account with all required fields" <| fun _ ->
                let account = Domain.Account.create 1 "1000" 1000 "Bank Account" Domain.AccountType.ASS_PAY 1 25.5 "standard" 0.0 1000.0 true true "2024-01-01" "2024-01-01"

                Expect.equal account.Id 1 "Account ID should match"
                Expect.equal account.Number "1000" "Account number should match"
                Expect.equal account.Name "Bank Account" "Account name should match"
                Expect.equal account.AccountType Domain.AccountType.ASS_PAY "Account type should match"
                Expect.equal account.Balance 1000.0 "Account balance should match"
        ]

        testList "AccountType" [
            testCase "should validate AccountType enum values" <| fun _ ->
                Expect.equal Domain.AccountType.ASS Domain.AccountType.ASS "ASS should equal ASS"
                Expect.equal Domain.AccountType.LIA Domain.AccountType.LIA "LIA should equal LIA"
                Expect.equal Domain.AccountType.REV Domain.AccountType.REV "REV should equal REV"
                Expect.equal Domain.AccountType.EXP Domain.AccountType.EXP "EXP should equal EXP"
        ]
    ]

[<Tests>]
let apiTests =
    testList "API Client" [
                        testCase "should stream businesses from API client" <| fun _ ->
            let apiToken = Environment.GetEnvironmentVariable("NOCFO_API_TOKEN")
            if String.IsNullOrEmpty(apiToken) then
                failtest "NOCFO_API_TOKEN environment variable not set"

            let client = NocfoApiClient(apiToken)
            let businessStream = client.GetBusinessesStreamAsync() |> Async.RunSynchronously

            // Convert stream to list for testing
            let businesses = FSharp.Control.AsyncSeq.toListAsync businessStream |> Async.RunSynchronously

            Expect.equal businesses.Length 2 "Should have 2 businesses"
            Expect.equal businesses.[0].Name "Holotropic Breathwork Finland ry" "First business name should match"
            Expect.equal businesses.[1].Name "Puhdas Koti Oy" "Second business name should match"
    ]

[<EntryPoint>]
let main argv = Tests.runTestsInAssemblyWithCLIArgs [||] argv
