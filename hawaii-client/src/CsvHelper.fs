module Nocfo.CsvHelpers
// Minimal CsvHelper support for F#-specific shapes (option<'a>, list<'a>, and JToken)
// Usage:
//   #r "nuget: CsvHelper, 30.0.1"
//   #r "nuget: Newtonsoft.Json, 13.0.1"
//   #load "src/CsvHelper.fs"
//   open Nocfo.CsvHelpers
//   use csv = new CsvWriter(writer, CsvConfiguration(CultureInfo.InvariantCulture))
//   registerFSharpConvertersFor csv (typeof<MyRecord>)
//   csv.WriteHeader<MyRecord>(); csv.NextRecord()
//   csv.WriteRecord(instance); csv.NextRecord()

open System
open System.Reflection
open System.Globalization
open Microsoft.FSharp.Reflection
open CsvHelper
open CsvHelper.Configuration
open CsvHelper.TypeConversion
open Newtonsoft.Json
open Newtonsoft.Json.Linq

// --- Shape detection helpers ---

let private isOption (t: Type) =
    t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

let private optionInner (t: Type) = t.GetGenericArguments().[0]

let private isFSharpList (t: Type) =
    t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<list<_>>

let private listInner (t: Type) = t.GetGenericArguments().[0]

// --- Converters ---

type OptionConverter<'a>() =
    inherit DefaultTypeConverter()
    override _.ConvertToString(value, memberMapData, row) =
        if isNull value then "" else
        let t = value.GetType()
        let (case, fields) = FSharpValue.GetUnionFields(value, t)
        if case.Name = "Some" then
            match box fields.[0] with
            | null -> ""
            | :? IFormattable as f -> f.ToString(null, CultureInfo.InvariantCulture)
            | :? string as s -> s
            | v -> v.ToString()
        else ""

type SeqJoinConverter<'a>() =
    inherit DefaultTypeConverter()
    override _.ConvertToString(value, memberMapData, row) =
        match value with
        | :? System.Collections.IEnumerable as xs ->
            xs
            |> Seq.cast<obj>
            |> Seq.map (function
                | null -> ""
                | :? IFormattable as f -> f.ToString(null, CultureInfo.InvariantCulture)
                | :? string as s -> s
                // Fallback: JSON-encode complex items compactly
                | :? JToken as jt -> jt.ToString(Formatting.None)
                | v -> JsonConvert.SerializeObject(v))
            |> String.concat ";"
        | _ -> ""

type JTokenCompactConverter() =
    inherit DefaultTypeConverter()
    override _.ConvertToString(value, memberMapData, row) =
        match value with
        | :? JToken as jt -> jt.ToString(Formatting.None)
        | null -> ""
        | v -> v.ToString()

/// Inspect the target type and register F#-aware converters for its property types.
/// This avoids CsvHelper's AutoMap (which can trip on F# shapes via constructor parameter mapping).
let registerFSharpConvertersFor (csv: CsvWriter) (t: Type) =
    let cache = csv.Context.TypeConverterCache

    let registerForType (pt: Type) =
        if isOption pt then
            let inner = optionInner pt
            let convT = typedefof<OptionConverter<_>>.MakeGenericType [| inner |]
            let conv  = Activator.CreateInstance(convT) :?> ITypeConverter
            cache.AddConverter(pt, conv) |> ignore
        elif isFSharpList pt then
            let inner = listInner pt
            let convT = typedefof<SeqJoinConverter<_>>.MakeGenericType [| inner |]
            let conv  = Activator.CreateInstance(convT) :?> ITypeConverter
            cache.AddConverter(pt, conv) |> ignore
        elif typeof<JToken>.IsAssignableFrom(pt) then
            cache.AddConverter(pt, JTokenCompactConverter()) |> ignore
        else
            ()

    // Register converters for all public instance properties on the type
    for p in t.GetProperties(BindingFlags.Instance ||| BindingFlags.Public) do
        registerForType p.PropertyType
