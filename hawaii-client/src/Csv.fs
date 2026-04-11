namespace Nocfo

open System
open System.Globalization
open System.IO
open System.Linq.Expressions
open System.Reflection
open CsvHelper
open CsvHelper.Configuration
open FSharp.Control
open Microsoft.FSharp.Reflection
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Nocfo.CsvHelpers
open NocfoApi.Http

module private CsvFieldMapping =
  let normalizeFields (fields: string list) : string list =
    fields
    |> List.collect (fun token -> token.Split(',') |> Array.toList)
    |> List.map (fun s -> s.Trim())
    |> List.filter (fun s -> s <> "")

  let private mapProperty<'T> (map: DefaultClassMap<'T>) (p: PropertyInfo) (index: int) (header: string option) =
    let param = Expression.Parameter(typeof<'T>, "x")
    let bodyProp = Expression.Property(param, p) :> Expression
    let bodyObj =
      if p.PropertyType.IsValueType then Expression.Convert(bodyProp, typeof<obj>) :> Expression
      else bodyProp
    let lambda : Expression<Func<'T, obj>> = Expression.Lambda<Func<'T, obj>>(bodyObj, param)
    let mm = CsvMapExtensions.MapBoxed(map, lambda).Index(index)
    header |> Option.iter (fun h -> mm.Name(h) |> ignore)
    mm |> ignore

  let buildClassMapForFields<'T> (fields: string list) : Result<DefaultClassMap<'T>, string list> =
    let t = typeof<'T>
    let props = t.GetProperties(BindingFlags.Instance ||| BindingFlags.Public)
    let byNameCI =
      props
      |> Array.map (fun p -> p.Name.ToLowerInvariant(), p)
      |> dict

    let wanted =
      match normalizeFields fields with
      | [] -> props |> Array.toList |> List.map (fun p -> p.Name)
      | xs -> xs

    let resolved, missing =
      (([], []), wanted)
      ||> List.fold (fun (accOk, accMissing) name ->
          let key = name.ToLowerInvariant()
          if byNameCI.ContainsKey key then
            (byNameCI.[key] :: accOk, accMissing)
          else
            (accOk, name :: accMissing))

    match missing with
    | _::_ ->
        let missingOrdered =
          wanted
          |> List.filter (fun n -> missing |> List.exists (fun m -> String.Equals(m, n, StringComparison.Ordinal)))
          |> List.distinct
        Error missingOrdered
    | [] ->
        let orderedProps = resolved |> List.rev
        let map = DefaultClassMap<'T>()
        orderedProps
        |> List.iteri (fun idx (p: PropertyInfo) -> mapProperty map p idx (Some p.Name))
        Ok map

module private CsvHeaderValidation =
  open CsvFieldMapping

  let validateHeader<'T> (header: string[]) (fields: string list option) : unit =
    let headerNames =
      header
      |> Array.map (fun s -> s.Trim().ToLowerInvariant())
      |> Set.ofArray

    match fields |> Option.defaultValue [] |> normalizeFields with
    | [] ->
        let t = typeof<'T>
        let propNames =
          t.GetProperties(BindingFlags.Instance ||| BindingFlags.Public)
          |> Array.map (fun p -> p.Name.ToLowerInvariant())
          |> Set.ofArray

        let unknown =
          headerNames
          |> Set.filter (fun h -> not (propNames.Contains h))
          |> Set.toList

        if not unknown.IsEmpty then
          failwithf "Unknown column(s) for %s: %s"
            t.FullName
            (String.Join(", ", unknown))

    | wanted ->
        let wantedCI =
          wanted
          |> List.map (fun s -> s.ToLowerInvariant())

        let missing =
          wantedCI
          |> List.filter (fun w -> not (headerNames.Contains w))

        if not missing.IsEmpty then
          failwithf "CSV is missing required column(s) for %s: %s"
            typeof<'T>.FullName
            (String.Join(", ", missing))

  let validateDeltaHeader<'TPatch> (header: string[]) (fields: string list option) : unit =
    let headerNames =
      header
      |> Array.map (fun s -> s.Trim().ToLowerInvariant())
      |> Set.ofArray

    let patchPropNames =
      typeof<'TPatch>.GetProperties(BindingFlags.Instance ||| BindingFlags.Public)
      |> Array.map (fun p -> p.Name.ToLowerInvariant())
      |> Set.ofArray

    let requested =
      fields
      |> Option.defaultValue []
      |> normalizeFields
      |> List.map (fun s -> s.ToLowerInvariant())

    let unknownRequested =
      requested
      |> List.filter (fun name -> name <> "id" && not (patchPropNames.Contains name))
      |> List.distinct

    if not unknownRequested.IsEmpty then
      failwithf "Unknown field(s) for %s: %s"
        typeof<'TPatch>.FullName
        (String.Join(", ", unknownRequested))

    match requested with
    | [] ->
        if not (headerNames.Contains "id") then
          failwithf "CSV is missing required column(s) for %s: id" typeof<'TPatch>.FullName

        let unknown =
          headerNames
          |> Set.filter (fun name -> name <> "id" && not (patchPropNames.Contains name))
          |> Set.toList

        if not unknown.IsEmpty then
          failwithf "Unknown column(s) for %s: %s"
            typeof<'TPatch>.FullName
            (String.Join(", ", unknown))

    | wanted ->
        let required =
          if wanted |> List.contains "id" then wanted else "id" :: wanted

        let missing =
          required
          |> List.filter (fun name -> not (headerNames.Contains name))

        if not missing.IsEmpty then
          failwithf "CSV is missing required column(s) for %s: %s"
            typeof<'TPatch>.FullName
            (String.Join(", ", missing))

module Csv =
  open CsvFieldMapping
  open CsvHeaderValidation

  type UnknownFieldPolicy =
    | Fail
    | WarnAndDrop

  type IOOptions =
    { Culture            : CultureInfo
      IncludeHeader      : bool
      NewLine            : string
      UnknownFieldPolicy : UnknownFieldPolicy }

  let defaultIOOptions =
    { Culture            = CultureInfo.InvariantCulture
      IncludeHeader      = true
      NewLine            = "\n"
      UnknownFieldPolicy = UnknownFieldPolicy.Fail }

  let private mkCsvWriter (tw: TextWriter) (opts: IOOptions) =
    let cfg = CsvConfiguration(opts.Culture)
    new CsvWriter(tw, cfg)

  let private mkCsvReader (tr: TextReader) (opts: IOOptions) =
    let cfg = CsvConfiguration(opts.Culture)
    cfg.MissingFieldFound <- null
    new CsvReader(tr, cfg)

  let private tryRegisterFieldsMap<'T> (context: CsvContext) (fields: string list option) =
    match fields with
    | None | Some [] -> []
    | Some xs ->
        match buildClassMapForFields<'T> xs with
        | Ok classMap ->
            context.RegisterClassMap(classMap)
            []
        | Error missing ->
            missing

  let writeCsvGeneric<'T>
      (tw: TextWriter)
      (fields: string list option)
      (rows: AsyncSeq<'T>)
    : AsyncSeq<unit> =
    let csv = mkCsvWriter tw defaultIOOptions

    registerFSharpConvertersFor csv.Context typeof<'T>

    let missing = tryRegisterFieldsMap<'T> csv.Context fields
    match missing with
    | (_::_ as miss) ->
        failwithf "Unknown field(s) for %s: %s"
          typeof<'T>.FullName
          (String.Join(", ", miss))
    | _ -> ()

    let mutable disposed = false
    let dispose () =
      if not disposed then
        csv.Flush()
        tw.Flush()
        (csv :> IDisposable).Dispose()
        disposed <- true

    asyncSeq {
      try
        csv.WriteHeader<'T>()
        csv.NextRecord()
        yield ()

        yield! rows |> AsyncSeq.map (fun item ->
          csv.WriteRecord(item)
          csv.NextRecord()
        )
      finally
        dispose ()
    }

  let private tryFindColumnIndex (fieldName: string) (header: string[]) =
    let target = fieldName.Trim().ToLowerInvariant()
    header
    |> Array.tryFindIndex (fun h -> h.Trim().ToLowerInvariant() = target)

  let private collectRecordMetadata (t: Type) headers (fields: string list option) =
    let fieldsInfo = FSharpType.GetRecordFields(t, true)
    let allowedFields =
      fields
      |> Option.map normalizeFields
      |> Option.map (List.map (fun name -> name.Trim().ToLowerInvariant()) >> Set.ofList)

    let isRequested (fieldName: string) =
      match allowedFields with
      | None -> true
      | Some allowed -> allowed.Contains(fieldName.Trim().ToLowerInvariant())

    let columnIndexPerField =
      fieldsInfo
      |> Array.map (fun p ->
          if isRequested p.Name then
            tryFindColumnIndex p.Name headers
          else
            None)

    let makeDefault (ft: Type) : obj =
      if FSharpType.IsUnion(ft, true) then
        let cases = FSharpType.GetUnionCases(ft, true)
        match cases |> Array.tryFind (fun c -> c.Name = "None") with
        | Some noneCase -> FSharpValue.MakeUnion(noneCase, [||], true)
        | None ->
            if ft.IsValueType then Activator.CreateInstance(ft) else null
      elif ft.IsValueType then
        Activator.CreateInstance(ft)
      else
        null

    let defaults =
      fieldsInfo
      |> Array.map (fun p -> makeDefault p.PropertyType)

    (fieldsInfo, columnIndexPerField, defaults)

  let private isOptionType (t: Type) =
    t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

  let private isStringCollectionType (t: Type) =
    if t.IsArray then
      t.GetElementType() = typeof<string>
    elif t.IsGenericType then
      let def = t.GetGenericTypeDefinition()
      let args = t.GetGenericArguments()
      args.Length = 1
      && args.[0] = typeof<string>
      && (
          def = typedefof<list<_>>
          || def = typedefof<System.Collections.Generic.List<_>>
          || def = typedefof<seq<_>>
          || def = typedefof<System.Collections.Generic.IEnumerable<_>>
          || def = typedefof<System.Collections.Generic.IReadOnlyList<_>>
          || def = typedefof<System.Collections.Generic.IList<_>>
      )
    else
      false

  let private isCollectionType (t: Type) =
    if t.IsArray then
      true
    elif t.IsGenericType then
      let def = t.GetGenericTypeDefinition()
      def = typedefof<list<_>>
      || def = typedefof<System.Collections.Generic.List<_>>
      || def = typedefof<seq<_>>
      || def = typedefof<System.Collections.Generic.IEnumerable<_>>
      || def = typedefof<System.Collections.Generic.IReadOnlyList<_>>
      || def = typedefof<System.Collections.Generic.IList<_>>
    else
      false

  let private deserializeJsonValue (rawText: string) (t: Type) =
    JsonConvert.DeserializeObject(rawText, t, Serializer.settings)

  let private setCollectionFieldValue (csv: CsvReader) (fieldName: string) (ft: Type) (colIndex: int) =
    let rawText = csv.GetField(colIndex)
    if String.IsNullOrWhiteSpace rawText then
      None
    else
      let rawTrimmed = rawText.Trim()

      let valueObj : obj =
        if isStringCollectionType ft && not (rawTrimmed.StartsWith("[") || rawTrimmed.StartsWith("{")) then
          let parts =
            rawText.Split(';')
            |> Array.map (fun s -> s.Trim())
            |> Array.filter (fun s -> s <> "")

          if ft.IsArray then
            parts :> obj
          elif ft.IsGenericType && ft.GetGenericTypeDefinition() = typedefof<list<_>> then
            let listType = typedefof<list<_>>.MakeGenericType(typeof<string>)
            let cases = FSharpType.GetUnionCases(listType, true)
            let emptyCase = cases |> Array.find (fun c -> c.Name = "Empty")
            let consCase = cases |> Array.find (fun c -> c.Name = "Cons")
            let mutable listObj = FSharpValue.MakeUnion(emptyCase, [||], true)
            for idx = parts.Length - 1 downto 0 do
              listObj <- FSharpValue.MakeUnion(consCase, [| parts.[idx] :> obj; listObj |], true)
            listObj
          else
            parts :> obj
        elif isCollectionType ft then
          deserializeJsonValue rawText ft
        else
          failwithf "CSV collection field '%s' has unsupported type '%s'." fieldName ft.FullName

      Some valueObj

  let private setOptionFieldValue (csv: CsvReader) (colIndex: int) (ft: Type) =
    let innerType = ft.GetGenericArguments().[0]
    let rawText = csv.GetField(colIndex)
    if String.IsNullOrWhiteSpace rawText then
      None
    else
      let innerValueObj : obj =
        if innerType = typeof<JToken> then
          let s = rawText.Trim()
          try
            if s.StartsWith("{") || s.StartsWith("[") then
              (JToken.Parse(s) :> obj)
            else
              (JValue(s) :> JToken :> obj)
          with _ ->
            (JValue(rawText) :> JToken :> obj)
        elif isStringCollectionType innerType then
          match setCollectionFieldValue csv "(collection)" innerType colIndex with
          | Some v -> v
          | None -> null
        elif isCollectionType innerType then
          match setCollectionFieldValue csv "(collection)" innerType colIndex with
          | Some v -> v
          | None -> null
        else
          csv.GetField(innerType, colIndex)

      let unionCases = FSharpType.GetUnionCases(ft, true)
      let someCase =
        unionCases
        |> Array.find (fun c -> c.Name = "Some")
      let optValue = FSharpValue.MakeUnion(someCase, [| innerValueObj |], true)
      Some optValue

  let private buildRecordFromCsv<'T> (csv: CsvReader) (fieldsInfo: PropertyInfo[]) (columnIndexPerField: int option[]) (defaults: obj[]) =
    let values = Array.copy defaults
    for i = 0 to fieldsInfo.Length - 1 do
      match columnIndexPerField.[i] with
      | Some colIndex ->
          let fi = fieldsInfo.[i]
          let ft = fi.PropertyType

          if isOptionType ft then
            match setOptionFieldValue csv colIndex ft with
            | Some optValue -> values.[i] <- optValue
            | None -> ()
          elif isStringCollectionType ft then
            match setCollectionFieldValue csv fi.Name ft colIndex with
            | Some collValue -> values.[i] <- collValue
            | None -> ()
          elif isCollectionType ft then
            match setCollectionFieldValue csv fi.Name ft colIndex with
            | Some collValue -> values.[i] <- collValue
            | None -> ()
          else
            let v = csv.GetField(ft, colIndex)
            values.[i] <- v
      | None ->
          ()
    values

  let readCsvGeneric<'T> (tr: TextReader) (fields: string list option)
    : AsyncSeq<'T> =
    let csv = mkCsvReader tr defaultIOOptions

    registerFSharpConvertersFor csv.Context typeof<'T>

    let mutable disposed = false
    let dispose () =
      if not disposed then
        (csv :> IDisposable).Dispose()
        disposed <- true

    asyncSeq {
      try
        let t = typeof<'T>
        let isFSharpRecord = FSharpType.IsRecord(t, true)
        if not isFSharpRecord then
          failwithf "Expected a F# record, got %s" t.FullName

        if csv.Read() then
          csv.ReadHeader() |> ignore

          validateHeader<'T> csv.HeaderRecord fields

          let recordFieldInfos, fieldColumnIndex, fieldDefaults =
            collectRecordMetadata t csv.HeaderRecord fields

          while csv.Read() do
            let values = buildRecordFromCsv<'T> csv recordFieldInfos fieldColumnIndex fieldDefaults
            yield (FSharpValue.MakeRecord(t, values, true) :?> 'T)
      finally
        dispose ()
    }

  let readDeltasCore<'TPatch, 'TDelta>
      (mkDelta: int -> 'TPatch -> 'TDelta)
      (tr: TextReader)
      (fields: string list option)
    : AsyncSeq<'TDelta> =
    let csv = mkCsvReader tr defaultIOOptions

    registerFSharpConvertersFor csv.Context typeof<'TPatch>

    let mutable disposed = false
    let dispose () =
      if not disposed then
        (csv :> IDisposable).Dispose()
        disposed <- true

    asyncSeq {
      try
        let patchType = typeof<'TPatch>

        if csv.Read() then
          csv.ReadHeader() |> ignore

          validateDeltaHeader<'TPatch> csv.HeaderRecord fields

          let idColumn =
            tryFindColumnIndex "id" csv.HeaderRecord
            |> Option.defaultWith (fun () -> failwithf "CSV is missing required column 'id' for %s." typeof<'TDelta>.FullName)

          let patchFieldInfos, patchFieldColumns, patchDefaults =
            collectRecordMetadata patchType csv.HeaderRecord fields

          while csv.Read() do
            let idValue = csv.GetField(typeof<int>, idColumn) :?> int
            let patchValues = buildRecordFromCsv<'TPatch> csv patchFieldInfos patchFieldColumns patchDefaults
            let patch = FSharpValue.MakeRecord(patchType, patchValues, true)
            let delta = mkDelta idValue (patch :?> 'TPatch)
            yield delta
      finally
        dispose ()
    }

  let inline readDeltas< ^TDelta, ^TPatch
      when ^TDelta : (static member Create : int * ^TPatch -> ^TDelta) >
      (tr: TextReader)
      (fields: string list option)
    : AsyncSeq< ^TDelta > =
    readDeltasCore< ^TPatch, ^TDelta >
      (fun id patch -> (^TDelta : (static member Create : int * ^TPatch -> ^TDelta) (id, patch)))
      tr
      fields
