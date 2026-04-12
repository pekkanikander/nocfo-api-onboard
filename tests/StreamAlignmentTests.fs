module StreamAlignmentTests

open Xunit
open Swensen.Unquote
open FSharp.Control
open NocfoClient

// Helper: run alignment synchronously and return a plain list.
let align (lItems: int list) (rItems: int list) : StreamAlignment<int, int> list =
    Streams.alignByKey id id (AsyncSeq.ofSeq lItems) (AsyncSeq.ofSeq rItems)
    |> AsyncSeq.toListSynchronously

// ── Edge cases ────────────────────────────────────────────────────────────────

[<Fact>]
let ``Both sequences empty yields nothing`` () =
    test <@ align [] [] = [] @>

[<Fact>]
let ``Left only yields all MissingRight`` () =
    let result = align [1; 2; 3] []
    test <@ result = [ MissingRight 1; MissingRight 2; MissingRight 3 ] @>

[<Fact>]
let ``Right only yields all MissingLeft`` () =
    let result = align [] [1; 2; 3]
    test <@ result = [ MissingLeft 1; MissingLeft 2; MissingLeft 3 ] @>

[<Fact>]
let ``Single matching element yields one Aligned`` () =
    test <@ align [42] [42] = [ Aligned (42, 42) ] @>

// ── Perfect match ─────────────────────────────────────────────────────────────

[<Fact>]
let ``Identical sequences yield all Aligned in order`` () =
    let result = align [1; 2; 3] [1; 2; 3]
    test <@ result = [ Aligned (1,1); Aligned (2,2); Aligned (3,3) ] @>

// ── Partial overlap ───────────────────────────────────────────────────────────

[<Fact>]
let ``Left has an extra element at the end`` () =
    let result = align [1; 2; 3] [1; 2]
    test <@ result = [ Aligned (1,1); Aligned (2,2); MissingRight 3 ] @>

[<Fact>]
let ``Right has an extra element at the end`` () =
    let result = align [1; 2] [1; 2; 3]
    test <@ result = [ Aligned (1,1); Aligned (2,2); MissingLeft 3 ] @>

// ── Interleaved keys ──────────────────────────────────────────────────────────

[<Fact>]
let ``Interleaved keys align correctly`` () =
    // left:  1   3   5
    // right:   2 3 4
    let result = align [1; 3; 5] [2; 3; 4]
    // left:  1   3   5
    // right:   2 3 4
    // step-by-step:
    //   1 < 2 → MissingRight 1 (advance left)
    //   3 > 2 → MissingLeft  2 (advance right)
    //   3 = 3 → Aligned (3,3)
    //   5 > 4 → MissingLeft  4 (advance right)
    //   5, end → MissingRight 5
    test <@ result = [
        MissingRight 1
        MissingLeft  2
        Aligned (3, 3)
        MissingLeft  4
        MissingRight 5
    ] @>

[<Fact>]
let ``Left key strictly less than all right keys`` () =
    let result = align [1; 2] [10; 20]
    test <@ result = [ MissingRight 1; MissingRight 2; MissingLeft 10; MissingLeft 20 ] @>

[<Fact>]
let ``Right key strictly less than all left keys`` () =
    let result = align [10; 20] [1; 2]
    test <@ result = [ MissingLeft 1; MissingLeft 2; MissingRight 10; MissingRight 20 ] @>
