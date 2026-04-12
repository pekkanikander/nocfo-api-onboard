module PatchShapeTests

open Xunit
open Swensen.Unquote
open Nocfo

// Local record types — keep tests independent of generated NocfoApi types.
// All Patch fields are option so we can exercise every branch of PatchShape.

type Full = { id: int; name: string; note: string option }
type Patch = { name: string option; note: string option }

// ── HasChanges ────────────────────────────────────────────────────────────────

[<Fact>]
let ``HasChanges returns false when all patch fields are None`` () =
    let patch = { name = None; note = None }
    test <@ PatchShape<Full, Patch>.HasChanges patch = false @>

[<Fact>]
let ``HasChanges returns true when one field is Some`` () =
    let patch = { name = Some "new"; note = None }
    test <@ PatchShape<Full, Patch>.HasChanges patch = true @>

[<Fact>]
let ``HasChanges returns true when all fields are Some`` () =
    let patch = { name = Some "a"; note = Some "b" }
    test <@ PatchShape<Full, Patch>.HasChanges patch = true @>

// ── Normalize: strip unchanged fields ────────────────────────────────────────

[<Fact>]
let ``Normalize strips Some when value matches full non-option field`` () =
    let full  = { id = 1; name = "Alice"; note = None }
    let patch = { name = Some "Alice"; note = None }
    let result = PatchShape<Full, Patch>.Normalize(full, patch)
    // name matches → stripped to None; note already None
    test <@ result = { name = None; note = None } @>

[<Fact>]
let ``Normalize keeps Some when value differs from full non-option field`` () =
    let full  = { id = 1; name = "Alice"; note = None }
    let patch = { name = Some "Bob"; note = None }
    let result = PatchShape<Full, Patch>.Normalize(full, patch)
    test <@ result = { name = Some "Bob"; note = None } @>

[<Fact>]
let ``Normalize strips Some when value matches full option field (Some vs Some)`` () =
    let full  = { id = 1; name = "Alice"; note = Some "hello" }
    let patch = { name = None; note = Some "hello" }
    let result = PatchShape<Full, Patch>.Normalize(full, patch)
    test <@ result = { name = None; note = None } @>

[<Fact>]
let ``Normalize keeps Some when value differs from full option field`` () =
    let full  = { id = 1; name = "Alice"; note = Some "old" }
    let patch = { name = None; note = Some "new" }
    let result = PatchShape<Full, Patch>.Normalize(full, patch)
    test <@ result = { name = None; note = Some "new" } @>

[<Fact>]
let ``Normalize keeps Some when full option field is None and patch wants to set it`` () =
    let full  = { id = 1; name = "Alice"; note = None }
    let patch = { name = None; note = Some "brand new" }
    let result = PatchShape<Full, Patch>.Normalize(full, patch)
    test <@ result = { name = None; note = Some "brand new" } @>

[<Fact>]
let ``Normalize leaves None patch fields alone`` () =
    let full  = { id = 1; name = "Alice"; note = Some "existing" }
    let patch = { name = None; note = None }
    let result = PatchShape<Full, Patch>.Normalize(full, patch)
    test <@ result = { name = None; note = None } @>

// ── Normalize + HasChanges together ──────────────────────────────────────────

[<Fact>]
let ``Normalized patch with no actual changes has no changes`` () =
    let full  = { id = 1; name = "Alice"; note = None }
    let patch = { name = Some "Alice"; note = None }
    let normalized = PatchShape<Full, Patch>.Normalize(full, patch)
    test <@ PatchShape<Full, Patch>.HasChanges normalized = false @>

[<Fact>]
let ``Normalized patch with a real change still has changes`` () =
    let full  = { id = 1; name = "Alice"; note = None }
    let patch = { name = Some "Bob"; note = None }
    let normalized = PatchShape<Full, Patch>.Normalize(full, patch)
    test <@ PatchShape<Full, Patch>.HasChanges normalized = true @>
