namespace Nocfo

// See LESSONS-LEARNED-OPENAPI.md for the rationale and implementation details.
// The implementation normalizes generated patch records against full API records
// using cached reflection so unchanged optional fields can be dropped.

open System
open System.Reflection
open Microsoft.FSharp.Reflection
open NocfoApi.Http

type DeltaFieldDescriptor =
  { FieldName: string
    DeltaField: PropertyInfo
    FullField: PropertyInfo
    DeltaIsOption: bool
    FullIsOption: bool
    DeltaNone: obj option }

module private DeltaShapeInternals =
  let ensureRecord (role: string) (t: Type) =
    if not (FSharpType.IsRecord t) then
      failwithf "%s must be an F# record type (got %s)" role t.FullName

  let isOptionType (t: Type) =
    t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

  let tryOptionalValue (optionType: Type) (value: obj) =
    let case, fields = FSharpValue.GetUnionFields(value, optionType)
    if case.Name = "Some" then Some fields.[0] else None

  let makeNoneValue (optionType: Type) =
    let noneCase = FSharpType.GetUnionCases optionType |> Array.head
    FSharpValue.MakeUnion(noneCase, [||])

  let requireProperty (owner: Type) (name: string) : PropertyInfo =
    match owner.GetProperty(name) with
    | null -> failwithf "%s is missing property '%s'" owner.FullName name
    | prop -> prop

open DeltaShapeInternals

type PatchShape<'Full,'Patch> private () =
  static let descriptors =
    ensureRecord "Full" typeof<'Full>
    ensureRecord "Patch" typeof<'Patch>

    FSharpType.GetRecordFields(typeof<'Patch>, true)
    |> Array.map (fun field ->
        let fullProp = requireProperty typeof<'Full> field.Name
        let patchIsOption = isOptionType field.PropertyType
        let fullIsOption = isOptionType fullProp.PropertyType

        { FieldName = field.Name
          DeltaField = field
          FullField = fullProp
          DeltaIsOption = patchIsOption
          FullIsOption = fullIsOption
          DeltaNone = if patchIsOption then Some (makeNoneValue field.PropertyType) else None })

  static let patchCtor =
    FSharpValue.PreComputeRecordConstructor(typeof<'Patch>)

  static let equivalent (left: obj) (right: obj) =
    match left, right with
    | null, null -> true
    | null, _ | _, null -> false
    | _ -> Serializer.serialize left = Serializer.serialize right

  static member Normalize(full: 'Full, patch: 'Patch) : 'Patch =
    let normalizeField descriptor =
      let original = descriptor.DeltaField.GetValue(patch)
      if not descriptor.DeltaIsOption then
        original
      else
        match tryOptionalValue descriptor.DeltaField.PropertyType original with
        | None -> original
        | Some desired ->
            let fullValue = descriptor.FullField.GetValue(full)
            let matches =
              if descriptor.FullIsOption then
                match tryOptionalValue descriptor.FullField.PropertyType fullValue with
                | Some existing -> equivalent existing desired
                | None -> false
              else
                equivalent fullValue desired

            if matches then
              descriptor.DeltaNone
              |> Option.defaultWith (fun () -> failwithf "Descriptor for '%s' expected optional patch field." descriptor.FieldName)
            else
              original

    descriptors
    |> Array.map normalizeField
    |> patchCtor
    :?> 'Patch

  static member HasChanges(patch: 'Patch) : bool =
    descriptors
    |> Array.exists (fun descriptor ->
        descriptor.DeltaIsOption &&
          (descriptor.DeltaField.GetValue(patch)
           |> tryOptionalValue descriptor.DeltaField.PropertyType
           |> Option.isSome))
