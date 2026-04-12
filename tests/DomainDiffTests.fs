module DomainDiffTests

open System
open Xunit
open Swensen.Unquote
open NocfoApi.Types
open Nocfo.Domain

// ── Helpers ───────────────────────────────────────────────────────────────────

// Minimal AccountFull using the generated static Create factory (all optionals default to None).
let private makeAccount (id: int) (name: string) : AccountFull =
    Account.Create(
        id              = id,
        created_at      = DateTimeOffset.UtcNow,
        updated_at      = DateTimeOffset.UtcNow,
        number          = "1000",
        padded_number   = 1000,
        name            = name,
        name_translations = [],
        header_path     = [],
        default_vat_rate = 0.0,
        is_shown        = true,
        balance         = 0.0f,
        is_used         = false
    )

// PatchedAccountRequest with all fields None, via the generated factory.
let private emptyPatch : PatchedAccountRequest = PatchedAccountRequest.Create()

// ── Account.diffAccount ───────────────────────────────────────────────────────

[<Fact>]
let ``diffAccount: same id, empty patch → Ok None (no changes)`` () =
    let full  = makeAccount 1 "Revenue"
    let delta : AccountDelta = { id = 1; patch = emptyPatch }
    match Account.diffAccount full delta with
    | Ok None -> ()
    | other -> Assert.Fail $"Expected Ok None, got %A{other}"

[<Fact>]
let ``diffAccount: same id, patch matches existing number → Ok None (normalized away)`` () =
    let full  = makeAccount 1 "Revenue"
    // number "1000" matches what makeAccount set → normalize strips it → no changes
    let patch = { emptyPatch with number = Some "1000" }
    let delta : AccountDelta = { id = 1; patch = patch }
    match Account.diffAccount full delta with
    | Ok None -> ()
    | other -> Assert.Fail $"Expected Ok None, got %A{other}"

[<Fact>]
let ``diffAccount: same id, changed number → Ok (Some (UpdateAccount ...))`` () =
    let full  = makeAccount 1 "Revenue"
    let patch = { emptyPatch with number = Some "9999" }
    let delta : AccountDelta = { id = 1; patch = patch }
    // AccountCommand contains a Hydratable (which has a function field) so equality
    // is not available — use pattern matching instead.
    match Account.diffAccount full delta with
    | Ok (Some (UpdateAccount { id = id })) ->
        test <@ id = 1 @>
    | other ->
        Assert.Fail $"Expected Ok (Some (UpdateAccount ...)), got %A{other}"

[<Fact>]
let ``diffAccount: mismatched ids → Error (Unexpected)`` () =
    let full  = makeAccount 1 "Revenue"
    let delta : AccountDelta = { id = 99; patch = emptyPatch }
    match Account.diffAccount full delta with
    | Error (DomainError.Unexpected _) -> ()
    | other -> Assert.Fail $"Expected Error (Unexpected ...), got %A{other}"

// ── Account.classify ──────────────────────────────────────────────────────────
// Note: Account.classify is an inline SRTP function whose member access uses the
// reserved-word field ``type``. Unquote cannot invoke inline functions dynamically,
// so we pre-compute the result and assert on the plain value.

let private classifyAcc (t: Type92dEnum option) =
    Account.classify { makeAccount 1 "?" with ``type`` = t }

[<Fact>]
let ``classify None → None`` () =
    let result = classifyAcc None
    test <@ result = None @>

[<Fact>]
let ``classify ASS → Asset`` () =
    let result = classifyAcc (Some Type92dEnum.ASS)
    test <@ result = Some Asset @>

[<Fact>]
let ``classify ASS_PAY → Asset`` () =
    let result = classifyAcc (Some Type92dEnum.ASS_PAY)
    test <@ result = Some Asset @>

[<Fact>]
let ``classify LIA → Liability`` () =
    let result = classifyAcc (Some Type92dEnum.LIA)
    test <@ result = Some Liability @>

[<Fact>]
let ``classify LIA_VAT → Liability`` () =
    let result = classifyAcc (Some Type92dEnum.LIA_VAT)
    test <@ result = Some Liability @>

[<Fact>]
let ``classify REV → Income`` () =
    let result = classifyAcc (Some Type92dEnum.REV)
    test <@ result = Some Income @>

[<Fact>]
let ``classify REV_NO → Income`` () =
    let result = classifyAcc (Some Type92dEnum.REV_NO)
    test <@ result = Some Income @>

[<Fact>]
let ``classify EXP → Expense`` () =
    let result = classifyAcc (Some Type92dEnum.EXP)
    test <@ result = Some Expense @>

[<Fact>]
let ``classify EXP_TAX_PRE → Expense`` () =
    let result = classifyAcc (Some Type92dEnum.EXP_TAX_PRE)
    test <@ result = Some Expense @>
