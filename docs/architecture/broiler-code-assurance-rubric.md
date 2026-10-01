# Choosing the values on a `// Broiler-AI:` line

- **Status:** Adopted for the platform rollout
- **Owner:** Broiler Code, with the platform assurance owner consulted
- **Recorded:** 2026-09-30

`Broiler.VM` defined the vocabulary of an assessment, and the parser holds every
value to it, but no document ever said what the values *mean*. Nothing checks
that an assessment is correct, so two assessors reading the same method could
write `Security=Medium` and `Security=High` and both pass every rule. With every
component annotated, that inconsistency would make the totals in each
`CODE-ASSURANCE.md` meaningless. This page is the missing definition. Every
assessment written for the rollout follows it, and so should every new one.

An assessment is what a machine read. It is never a review. Nothing on the
`// Broiler-AI:` line or the `// Broiler-Falsified-If:` line may say or imply
that a person has read, checked, approved or signed off the code.

## The block

```csharp
/// <summary>…the declaration's own documentation stays where it is…</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=TBF
// Broiler-Falsified-If: a length prefix larger than the remaining input is read past the end of the buffer
// Broiler-Human:        PENDING
[SomeAttribute]
public int ReadLengthPrefixed(ReadOnlySpan<byte> input) …
```

The block sits between the doc comment and the declaration's first token,
including any attributes. `Fingerprint=TBF` is written by the assessor, and the
generator fills in the value. The human line is always `PENDING` when an
assessment is written, and only a person changes it.

## Origin: where the declaration's text came from

| Value | Use when |
|---|---|
| `AI` | The declaration was drafted with AI assistance in a Broiler repository. This is the default for platform code. The components say so themselves: "substantially AI-assisted". |
| `Original` | There is evidence that a person wrote it by hand, such as the component's history or notes. Do not guess. Without evidence, the answer is `AI`. |
| `Specification` | The body is a step-for-step transcription of a normative algorithm, such as ECMA-262, WHATWG, a Unicode annex, an RFC or the PDF specification. The steps must be visible in the code or its comments. Implementing a feature that a specification describes is **not** enough. |
| `Ported` | It was translated or adapted from another project's code and is still recognisably that code. Examples are HTML Renderer lineage in HTML, CSS, Graphics and Layout, and the Yantra JS import in Broiler.JS. |
| `Derived` | It was generated mechanically from data, such as `*.g.cs` tables and Unicode or CLDR tables. |
| `ThirdParty` | It was vendored unmodified from another project. |

For a component whose notices name an upstream (`THIRD_PARTY_NOTICES.md`,
`LICENSES/`, README "Origins"), check whether the file existed in the import
commit and whether the declaration is still recognisably upstream code. Use
`git log --diff-filter=A --follow -- <file>` for the first question and
`git log --follow -p -- <file>` for the second. A declaration added or rewritten
after the import is `AI`.

## IP: how well established the right to ship this text is

Values run from the weakest claim to the strongest: `None` < `Low` < `Medium` <
`High` < `Unknown`. `Unknown` is not a good result. It means the provenance
could not be established.

| Value | Use when |
|---|---|
| `None` | The declaration has no expressive content that could carry a copyright question. Examples are a plain data carrier, an enum of spec-defined names, or a one-line forwarder. |
| `Low` | It is original platform code, including original implementations of standard algorithms. This is the normal value. |
| `Medium` | It is `Ported` or `Derived` from third-party code or data whose licence permits this use and whose notice the component ships. Examples are HTML Renderer (BSD-3-Clause), Yantra JS (Apache-2.0) and Unicode data. It also covers code that closely follows one well-known external implementation. |
| `High` | The code is third-party-derived, but its notice is **missing** from the component that ships it. It also covers a licence whose compatibility is doubtful. Say which in the falsification line if Security also requires one. Otherwise report it to the rollout lead. |
| `Unknown` | You found signs of copying, such as foreign naming, comments or idioms that don't fit the codebase, and could not trace them. |

## Security: what a defect here could do to a user

Ask what an attacker controls when this code runs: page content, a document
file, a font, an image, a network response, a cookie, a URL or script. Then ask
what goes wrong if the code is wrong.

| Value | Use when |
|---|---|
| `None` | No input reaches it and it has no effect beyond its own value. Examples are constants, pure value types, and names and descriptions. |
| `Low` | It works on data that is trusted or already validated, and a defect produces a wrong result but no exposure. Examples are internal bookkeeping, formatting and diagnostics text. |
| `Medium` | Attacker-influenced data reaches it after parsing, so a defect could corrupt state, mis-render or throw. Examples are layout and style computation over page content, DOM mutation, event dispatch, UI input handling and caching. This is the common value for engine code. |
| `High` | It **parses or decodes untrusted input**: HTML, CSS, JS source, fonts, images, PDF or other documents, archives, network protocols, cookies, URLs or regular expressions. The same applies if it **enforces a boundary** (origin, permission, sandbox, CSP, credential or cookie scope), touches the **file system, processes or native interop** (P/Invoke, COM), or runs **concurrency** that guards shared state. |
| `Critical` | It is **memory-unsafe** (`unsafe`, pointers, `Marshal`, `stackalloc` or native buffers sized from input), **generates or executes code** (JIT, executable memory), is the **core of an interpreter, verifier or validator** that runs untrusted script or bytecode, or is the **bridge that exposes host objects or capabilities to page script**. |

`High` and `Critical` require a `// Broiler-Falsified-If:` line, and `None`,
`Low` and `Medium` carry none. Do not downgrade a unit to avoid writing one. If
a unit meets any `High` condition, it is `High`.

## Resources: how much work or memory a hostile or very large input can make it spend

This is a score from 0 to 10, taken over the declaration's own behaviour and
what it directly drives. It is not a measure of how often the code is called.

| Score | Meaning |
|---|---|
| 0 | Constant work and no allocation. |
| 1–2 | Small, bounded allocation, or work linear in an input that is small by construction. |
| 3–4 | Work or allocation linear in an input's size, such as a string, a node list or a stream. |
| 5–6 | Recursion that follows input nesting, caches that grow with input, large buffers, or quadratic worst cases. |
| 7–8 | Growth driven by untrusted length or count fields, backtracking, whole-document layout or paint, image, font or media decode, or anything that is bounded only by a budget kept elsewhere. |
| 9–10 | The unit exhausts CPU or memory by design unless an external limit stops it. Examples are a script execution loop and decompression. |

## Keeping values consistent across related units

Most disagreements between two assessors come from relationships between
units, not from the units themselves. The first pilot audits found every one of
the following. These rules settle them.

- **A type carries at least the worst of what it declares.** Its `Security`
  is at least the highest `Security` of its members, and its `Resources` is at
  least their highest `Resources`. Its criterion names the worst failure among
  them. Broiler.VM already does this for most of its type units.
- **Cost flows to callers.** `Resources` is what one call can cost, so a unit
  scores at least what the units it calls can cost with inputs it passes
  through. The exception is when that cost is bounded by a budget this unit
  enforces. A one-line `GetSite(host)` over a quadratic suffix walk scores what
  the walk scores.
- **A forwarder takes the `Security` of what it forwards to.** A one-line
  method that hands attacker-influenced data to a parser, or a boundary check to
  another method, is exactly as risky as the call it makes.
- **A computed table takes the `Security` of the decision it configures.**
  Examples are a character set a validator accepts and the list of forbidden
  headers, built as an array or a set. Something that is only displayed or
  logged is `None` or `Low`. A named value (a `const`, an enum, a `static
  readonly` `Guid` or handle stated by literals) is assessed the same way only
  in a component that reviews its named values; in one that watches them it
  is not assessed at all (see below), whatever decision it configures, because
  the decision is assessed where the code that enforces it is.
- **An interface or abstract member is assessed by its contract.** Rate it by
  what every implementation must get right. A transport interface that carries
  untrusted responses is `High`, even though it has no body, and its criterion
  is stated against the contract.
- **Equality that is a boundary is a boundary.** A record or key type whose
  equality or hash decides isolation is `High`, including compiler-generated
  equality. Examples are a cookie partition key, a site key and a cache key per
  origin.
- **Siblings of the same shape get the same values.** Before writing a value
  that differs from a same-shaped sibling in the same file, check that the code
  really differs.

## When the code already fails its criterion

Writing a criterion means asking how the unit could be wrong, and sometimes the
answer is that it already is. For example, a lazy cache mutated without a lock
on a type documented as thread-safe, or a response body decompressed with no
size cap. Keep the criterion as it is: it describes the defect precisely. Then
report the unit, with the evidence, as a **suspected defect** to whoever runs
the assessment, so that it reaches an issue tracker rather than living only in
a comment. An assessment never fixes code.

## Spec: optional

Cite a record only when the code plainly implements it. Put a component ADR as
`ADR-nnnn` or `ADR-nnnn sN`, and only when it exists in the component's
`docs/adr/`. Put an external section as `<document> s<section>`, for example
`ECMA-262 s27.2.1.3`, `RFC-6265bis s5.4` or `CSS-GRID-2 s7.2`. Omit the field
rather than guess.

## The falsification criterion

The criterion is required for `High` and `Critical` and is not written for
`None`, `Low` or `Medium`. Below `High`, the observation that would prove a unit
wrong is rarely more than its own name restated (`EGL_NONE is not 0x3038`), and a
line that restates is noise beside the ones that do not. It is **one line of
prose** that names one observation that would prove the unit wrong. It is a test
that someone could run or look for, not a description of risk.

- Good: `a length prefix larger than the remaining input is read past the end of the buffer`
- Good: `a cookie set by a subdomain is returned to its parent domain without a Domain attribute`
- Bad: `this is security sensitive`. It is not an observation.
- Bad: `Limit=64 is exceeded`. A `Key=Value` shape is refused.
- Bad: `reviewed and found safe`. Review claims are refused. Words such as reviewed, approved, verified by, signed off and checked by are forbidden on this line.

Write it in the component's own vocabulary. Several components have guard tests
that scan raw source text, comments included, for banned words: engine names in
the HtmlBridge neutrality ratchet, `Broiler.Graphics` in Media, and conformance
claims in DOM. The component's tests after annotation are the arbiter.

## Named values are watched, not assessed

A component whose `assurance.config.json` sets `"namedValues": "watched"` does
not assess its named values: every `const` field, every enum declaration, and
every `static readonly` `Guid`, `IntPtr`, `UIntPtr`, `nint` or `nuint` whose
initializer is literals alone (`new Guid("...")`, `IntPtr.Zero`, `(IntPtr)(-1)`).
The scanner exempts them as `NamedValue`, and they carry no block.

A value transcribed from somewhere else carries no decision. `public const int
EGL_NONE = 0x3038;` copied from the EGL headers is right if it matches the
headers and wrong if it does not, and the block above it could say nothing
else: an IP risk of a fact, a security value borrowed from whatever calls it,
and a criterion that restates the line. Several hundred such blocks bury the
few in the same component that say something. What the scheme still needs from
a named value is to notice when it changes, and that it keeps: each one is a
unit with a fingerprint in `assurance.manifest.json`, exactly as a storage field
and an enum member are, so an edited value moves a fingerprint the check
compares and appears in the diff of the generated record.

Nothing else is a named value. A `static readonly` array, set, `Regex` or
dictionary, a value built by a call, and any other type are assessed as before;
so are P/Invoke and `LibraryImport` declarations, COM interfaces and their
members, delegates for native procedures and struct layouts, where widths and
layouts have been found wrong. A component that does not set the option assesses
its named values like any other declaration, as Broiler.VM does.

When a component switches to watching its named values, `broiler-review
assurance prune` removes the blocks above them (and every criterion below
`High`), leaving any block whose human line names a reviewer or reads `STALE`
for a person to decide; `generate` then rewrites the headers and the report.

## Exempt units

The scanner's predicate decides which declarations need a block at all:
trivial accessors, parameter-assigning constructors, one-line forwarders,
storage fields, enum members, named values where a component watches them, and
similar. An assessor never annotates an exempt unit. `EXEMPT=<reason>` exists
for what the predicate cannot see, and a rollout does not use it.
