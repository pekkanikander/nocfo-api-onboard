# Lessons Learned: Full vs. Delta Types in OpenAPI and F#

This document explains a design pattern encountered while integrating the NoCFO API with a strongly typed F# domain model.
The topic is general and not specific to NoCFO: it concerns how to represent **full** resource objects and
their **partial/delta** update equivalents in a statically typed language,
and what OpenAPI can and cannot express in this regard.

The intended audience:
- understands F# and .NET reflection,
- knows OpenAPI at a practical level,
- but is not familiar with OpenAPI vendor extensions,

The goal is to document a key structural lesson for future projects.

---

## 1. The Problem: API Returns *Full Objects*, But Updates Expect *Partial Objects*

Many HTTP APIs (including NoCFO, but also GitHub, Stripe, etc.) follow the
pattern of Full vs. Delta (Entity vs. PatchedEntity) objects.

With the current Hawaii, they are represented as completely separate types:

- When you **GET** a resource, you receive a *full* representation.
  ```fsharp
  type AccountFull =
    { id: int
      number: string
      other_fields ... }
  ```

- When you **PATCH** a resource, you send a *partial* object containing only the fields you want to modify:
  ```fsharp
  type AccountDelta =
    { id: int
      number: string option }
  ```

We call these *Full* and *Delta* types.  At the HTTP API level, they are usually called
`Entity` and `PatchedEntity` types.

Conceptually, a Delta type is a *projection* of the Full type where fields are made optional.
Almost every API ecosystem follows this pattern,
yet **OpenAPI has no first-class mechanism to declare this relationship**.

This leads to duplicated type definitions, duplicated validation logic,
and room for drift (e.g., `Account` changes in the API, but `PatchedAccount` is not updated accordingly).

---

## 2. What We Want the Type System to Guarantee

The desired invariant is:

> **A `Full` type must be an extension of its corresponding `Delta` type.**
> More precisely:
> - Every field in `Delta` must exist in `Full`.
> - The base (non-option) types must match.
> - Extra fields in `Full` are allowed.

Example of a valid Full/Delta pair:

```fsharp
type AccountFull =
  { id: int
    number: string
    extra: string }

type AccountDelta =
  { id: int
    number: string option }
```

Example of an *invalid* pair:

```fsharp
type BadFull  = { id: string }
type BadDelta = { id: int }      // mismatched base types → invalid
```

### Important point

F# **cannot express this invariant at compile time**.
Neither can OpenAPI.
This is the fundamental structural gap.

---

## 3. Why This Matters

When making a HTTP `PATCH` over an API, it is advisable to be parsimonius,
i.e. to update only those fields that actually have changed.
(This is not atomic, but if there is a lot of data, this may be important for performance.)

If the system does not know which fields have changed, the system needs to:
1. Compare a Delta to the corresponding Full value.
2. Remove (“normalise away”) fields that do not represent real changes.
3. Ensure the mapping between Full and Delta remains correct even when the API schema evolves.

Doing this check **per value** at runtime is expensive and unnecessary.
Doing it **per type pair** (once) is correct.
Doing it **at compile time** would be ideal — but is not possible in pure F#.

This leads to the next question:

> **Where *should* the invariant live?**

And the answer is:
**In the schema.**
But OpenAPI as it is today cannot express it.

### Community experience

This Full/Delta difficulty is widely recognised across API ecosystems:

- In OpenAPI discussions (StackOverflow, GitHub issues), people consistently note that PATCH semantics require either *duplicated schemas* or *manual composition* via `allOf`. Typical answers state that OpenAPI cannot express “same as X but optional fields” and that the usual workaround is to split properties into a shared `Properties` schema and recombine with separate `required` lists.

- For JSON Merge Patch and JSON Patch, the community repeatedly points out that OpenAPI cannot declare that a patch document corresponds structurally to a base type. PATCH bodies are typically documented as `type: object` without static linkage to the resource type.

- Most languages (C#, Java, Go, TypeScript at the OpenAPI level) tolerate weaker typing: PATCH DTOs with nullable fields, `Map<string,any>`, or pointer‑based partial structs. The structural relationship is rarely enforced or even expressible at the schema level.

These recurring complaints indicate that the limitation is inherent to OpenAPI rather than any particular tool such as Hawaii.

---

## 4. Why OpenAPI Cannot Express Full/Delta Relations

OpenAPI 3.x describes JSON structures using:
- `type`
- `properties`
- `nullable`
- `required`

But it *cannot* say:

> “Type X is exactly the same as type Y, except with all fields optional.”

Nor can it say:

> “Type X must contain this subset of fields from type Y.”

Even if the relationship is conceptually clear to humans and to the backend implementation, the OpenAPI tooling lacks a declarative mechanism.

This is a frequent limitation in real-world API design.

### Evidence from the ecosystem

The lack of a native Full/Delta relation has been noted repeatedly:

- Community answers emphasise that OpenAPI supports composition (`allOf`) but not “subset of another schema”. Even recommended patterns (e.g. factoring out a `*Properties` schema) are explicitly described as partial or hacky, especially when nullability or nested objects are involved.

- The situation is the same for JSON Patch / Merge Patch: OpenAPI can describe the media type but cannot guarantee alignment between patch paths and the underlying schema.

- OData/OpenAPI alignment work has proposed extensions such as `x-baseType` for expressing inheritance-like relationships—convergent with what is needed here—but this is not part of the core specification.

---

## 5. The Correct Fix: Vendor Extensions

OpenAPI *does* allow vendor-specific extensions using the `x-...` namespace. For example:

```yaml
AccountPatched:
  type: object
  x-baseType: Account
  properties:
    id: { type: integer }
    number:
      type: string
      nullable: true
```

A hypothetical improved code generator (such as Hawaii) could read this and:
- verify that `AccountDelta` corresponds structurally to `AccountFull`,
- generate Delta types automatically,
- generate normalisation functions,
- emit compile-time or build-time errors when the invariant breaks.

This solves the problem at the correct level.

However, at the time of this writing, making such a large extension to Hawaii falls outside the scope of this project.

### Community practice around vendor extensions

Vendor extensions (`x-*`) are the established mechanism by which API designers express structure the core spec cannot model. Major generators (OpenAPI Generator, AutoRest, Redocly, Speakeasy) already rely on their own `x-*` flags for behaviour that exceeds OpenAPI’s expressiveness.

The idea of annotating a schema with something like `x-baseType` has direct precedents in the OData/OpenAPI work, where similar extensions are used to encode inheritance constraints not representable in vanilla OpenAPI. Applying this pattern to express the Full/Delta relationship would therefore be aligned with existing practice rather than an unusual design.

Therefore, we need a practical solution inside F#.  And probably quite a few people do.

---

## 6. A Practical Solution in F#: One-Time Reflection + Caching

Our chosen practical approach is as follows:

1. Perform a **single reflection check** per each (`Full`, `Delta`) pair.
2. Cache the resulting “shape descriptor”.
3. Use this descriptor for all future normalisation operations.

This yields:
- A safety check (run once),
- Almost zero redundant cost in the hot path,
- A separation of correctness (type-level shape) from behaviour (normalisation logic).

This pattern is implemented as:

### `DeltaShape<'Full,'Delta>`

A generic static container that:
- Runs reflection *exactly once* when the type pair is first referenced,
- Verifies the extension relation (or trusts it, depending on settings),
- Precomputes field alignments,
- Exposes fast property getters / optionality information to the normaliser.

This matches how high-performance serialisation libraries operate (e.g., Utf8Json, MessagePack, FsPickler).

---

## 7. Deployment and Testing Strategy

There could be three modes of operation:

### 1. Prototype Mode (current)
- Skip the reflection check entirely or run it only in DEBUG.
- Runtime performance is optimal.

### 2. Maintainability Mode
- A tiny unit test triggers the static initialiser:
  ```fsharp
  let _ = DeltaShape<AccountFull,AccountDelta>.Descriptors
  ```
- If the API schema drifts, the test fails immediately.

### 3. Future “Correct” Mode
- If the API eventually supports an `x-baseType` extension in OpenAPI,
- And Hawaii supports it,
- Then the F# layer can become trivial and drop most of the reflection logic.

---

## 8. Summary of the Lessons

### The essential lessons learned:

1. **Full/Delta type pairs are a universal API pattern**, but OpenAPI cannot encode their relationship.
2. **F# cannot encode the relationship in the type system either**.
3. **Therefore the invariant must be checked or enforced at another level**:
   - Ideally in the schema (e.g. via vendor extensions),
   - Otherwise in generated code,
   - Or, minimally, once per type pair at runtime via reflection.
4. **Doing the check per value is wasteful**; caching solves this cleanly.
5. **The `DeltaShape` abstraction is the correct architectural layer inside F#**.
6. **Unit tests or DEBUG-mode checks maintain safety without runtime overhead**.
7. **Future OpenAPI extensions would allow eliminating the entire problem**.

---

## 9. Recommendations for Future Work

If the ecosystem allows:

1. **Define a vendor extension**, e.g.
   ```
   x-baseType: AccountFull
   ```
2. **Extend Hawaii to understand the extension**
   - Validate schemas,
   - Auto-generate Delta types,
   - Auto-generate normalisers.

3. **Remove most of the reflection-based machinery from the domain layer**
   The F# code would then simply use generated helpers.

Until that time, the `DeltaShape` cached reflection approach remains the most robust, efficient, and maintainable solution.

---


## 10. References and Further Reading

The following resources illustrate the broader discussion around PATCH semantics, partial updates, and vendor extensions in OpenAPI:

- **PATCH and partial updates with allOf composition**
  Community discussion and example pattern using a shared `PersonProperties` schema and separate `Person` / `PersonUpdate` types, highlighting the limitations for nested objects and nullable properties.
  https://stackoverflow.com/questions/62358406/how-to-update-resources-with-patch-method-using-openapi-correctly

- **Documenting JSON Merge Patch endpoints in OpenAPI**
  Discussion of how to describe `application/merge-patch+json` in OpenAPI and the lack of a direct linkage between the patch document and the base schema.
  https://stackoverflow.com/questions/72491333/how-to-document-json-merge-patch-endpoints-in-openapi

- **OData / OpenAPI alignment and `x-baseType`**
  OASIS OData TC work item proposing an `x-baseType` extension keyword in addition to `allOf` for expressing inheritance information not representable in vanilla OpenAPI.
  OData/OpenAPI mapping: https://docs.oasis-open.org/odata/odata-openapi/v1.0/cn01/odata-openapi-v1.0-cn01.html
  TC discussion referencing `x-baseType`: https://groups.oasis-open.org/communities/community-home/digestviewer/viewthread?MessageKey=0c8c2d7e-216c-473f-8ab9-202df19b1426

- **General documentation on OpenAPI vendor/specification extensions**
  Overview of `x-*` extensions and their typical use to encode tool-specific or higher-level semantics:
    - Swagger / OpenAPI official docs: https://swagger.io/docs/specification/v3_0/openapi-extensions/
    - Redocly extensions: https://redocly.com/docs-legacy/api-reference-docs/spec-extensions
    - AutoRest-specific extensions: https://azure.github.io/autorest/extensions/
    - ReadMe `x-readme` extensions: https://docs.readme.com/main/docs/openapi-extensions
    - Zuplo article on using extension data in code: https://zuplo.com/docs/articles/use-openapi-extension-data
    - Google Cloud Endpoints `x-google-*` extensions: https://cloud.google.com/endpoints/docs/openapi/openapi-extensions

End of document.
