namespace Nocfo

open System
open System.Reflection
open Microsoft.FSharp.Reflection

type DeltaFieldDescriptor =
  { FieldName: string
    DeltaField: PropertyInfo
    FullField: PropertyInfo
    DeltaIsOption: bool
    FullIsOption: bool
    BaseType: Type
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

  let baseTypeOf (propertyType: Type) =
    if isOptionType propertyType then
      propertyType.GetGenericArguments().[0], true
    else
      propertyType, false

  let requireProperty (owner: Type) (name: string) : PropertyInfo =
    match owner.GetProperty(name) with
    | null -> failwithf "%s is missing property '%s'" owner.FullName name
    | prop -> prop

open DeltaShapeInternals

type DeltaShape<'Full,'Delta> private () =
  static let descriptors =
    ensureRecord "Full" typeof<'Full>
    ensureRecord "Delta" typeof<'Delta>

    let buildDescriptor (field: PropertyInfo) =
      let fullProp = requireProperty typeof<'Full> field.Name
      let deltaBase, deltaIsOption = baseTypeOf field.PropertyType
      let fullBase, fullIsOption = baseTypeOf fullProp.PropertyType

      if deltaBase <> fullBase then
        failwithf "Field '%s' has incompatible types between %s and %s."
                   field.Name typeof<'Delta>.FullName typeof<'Full>.FullName

      { FieldName = field.Name
        DeltaField = field
        FullField = fullProp
        DeltaIsOption = deltaIsOption
        FullIsOption = fullIsOption
        BaseType = deltaBase
        DeltaNone = if deltaIsOption then Some (makeNoneValue field.PropertyType) else None }

    FSharpType.GetRecordFields(typeof<'Delta>)
    |> Array.map buildDescriptor

  static let deltaCtor =
    FSharpValue.PreComputeRecordConstructor(typeof<'Delta>)

  static member Descriptors : DeltaFieldDescriptor[] = descriptors

  static member Normalize(full: 'Full, delta: 'Delta) : 'Delta =
    let normalizeField descriptor =
      let original = descriptor.DeltaField.GetValue(delta)
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
                | Some existing -> existing.Equals(desired)
                | None -> false
              else
                fullValue.Equals(desired)

            if matches then
              descriptor.DeltaNone
              |> Option.defaultWith (fun () -> failwithf "Descriptor for '%s' expected optional delta field." descriptor.FieldName)
            else
              original

    descriptors
    |> Array.map normalizeField
    |> deltaCtor
    :?> 'Delta

  static member HasChanges(delta: 'Delta) : bool =
    descriptors
    |> Array.exists (fun descriptor ->
        descriptor.DeltaIsOption &&
          (descriptor.DeltaField.GetValue(delta)
           |> tryOptionalValue descriptor.DeltaField.PropertyType
           |> Option.isSome))
