# The assurance commands of `broiler-review`

- **Status:** `list` and `insert` delivered; `generate` and `check` not yet
- **Owner:** Broiler Code
- **Recorded:** 2026-09-30

[Reviewing one declaration at a time](broiler-code-assurance.md) describes the
per-declaration annotation that `Broiler.VM` carries, and the editor pane that
records a reviewer's name on it. This record describes the command-line half,
which brings the same annotation to the other component repositories:

```text
broiler-review assurance list   --root <dir> [--files <list>] [--json <out>|-] [--all-units]
broiler-review assurance insert --root <dir> --assessments <file.json> [--dry-run] [--json <out>|-]
```

Exit codes: `0` done, `1` something was refused, `2` a usage or configuration
error, reported before anything is written.

## Opting in: `assurance.config.json`

A component opts in by committing `assurance.config.json` at its root. `list`
works without it and guesses the product projects, and says so. `insert` refuses
to run without it, so pointing the tool at a component cannot change that
component's sources. An unknown property is an error, not ignored, because a
misspelled `exclude` would otherwise cover the files it was written to leave out.
Comments and trailing commas are allowed.

| Property | Type | Default | Meaning |
| --- | --- | --- | --- |
| `schema` | `1` | `1` | Format version. |
| `component` | string | root directory name | Name used in generated prose. |
| `mode` | `"owned"` \| `"external"` | `"owned"` | `external`: another tool writes the annotations (Broiler.VM's own tests). This tool only reads; every write command refuses. |
| `projects` | string[] | **required** | Product `.csproj` files, root-relative with `/`. |
| `exclude` | (glob \| `{glob, reason}`)[] | `[]` | Files left out of coverage. The reason is printed beside each excluded file. |
| `spdx` | `{copyright: string[], license: string}` | none | SPDX lines of the generated header: one or more `SPDX-FileCopyrightText` values (without the prefix) and one licence expression, e.g. `Apache-2.0 AND BSD-3-Clause`. |
| `spdxOverrides` | `{glob, copyright[], license}`[] | `[]` | SPDX lines for particular files, such as third-party-derived code. The first matching glob wins. |
| `artefacts` | `{report, humanReview, manifest}` | `CODE-ASSURANCE.md`, `HUMAN_REVIEW.md`, `assurance.manifest.json` | Where the component-level artefacts go, so that a hand-written `HUMAN_REVIEW.md` can stay where it is. |
| `preprocessorSymbols` | string[] | the owning component's net10.0 set | Symbols the scanner parses under. Code under an undefined symbol is disabled text, in no unit and no fingerprint. |
| `forbidDirectives` | bool | `false` | Whether any preprocessor directive in a covered file is a violation (the owning component's rule J6). |
| `forgeryVocabulary` | `"narrow"` \| `"strict"` | `"narrow"` | Which words mark a comment below the header as a forged summary. `strict` is the owning component's full list. |
| `closedToEscapeHatch` | string[] | `[]` | Assemblies where `EXEMPT=` may not be written. |
| `adrDirectory` | string | `docs/adr` | Where `Spec=ADR-nnnn` citations are resolved. |
| `regenerateCommand` | string | the tool's own | The command generated prose tells a reader to run. |

Globs are matched against root-relative paths, case-sensitively: `*` and `?`
stay within a segment, `**` spans segments, and a glob with no `/` matches the
file name at any depth (`*.g.cs`).

`spdx`, `spdxOverrides`, `artefacts`, `forbidDirectives`, `forgeryVocabulary`,
`adrDirectory` and `regenerateCommand` are read and validated now and used by
`generate` and `check`, which are not delivered yet.

## Which files are covered

Every `*.cs` under each configured project's directory, ordered by root-relative
path (ordinal). Left out:

- `bin` and `obj` directories at any depth. Some components track stale
  `obj/**/*.AssemblyAttributes.cs` files.
- Any directory holding a `.git` entry: a nested checkout of another component,
  or an untracked stale copy of one. A configured project inside one is refused.
- Another configured project's directory, which that project covers, so no file
  is covered twice.
- Files matching an `exclude` glob.

`--files <list>` restricts a run to the paths the list names, one per line.

## `list`

By default, the relevant units that carry no block the owning component would
attach, which is the work left to do. `--all-units` lists every unit. Each unit
has `file`, `assembly`, `line` and `column` (1-based, of the declaration's first
token, attributes included), `indent`, `unit` (the qualified name the owning
component's manifest uses), `displayName`, `kind`, `fingerprint`,
`extent {startLine, endLine}`, `exempt`, `exemption`, `state`, `insertable`,
`reason` and, when it helps, `detail`. Per-file and total counts are included.

`reason` is `none` when a block can be inserted, otherwise one of `exempt`,
`annotated`, `not-own-line`, `malformed-block` (the attached AI line does not
parse), `half-block` (a human or criterion line with no AI line),
`below-declaration` (an assurance comment inside the declaration's header), or
`line-model-mismatch` (the file breaks lines on U+0085, U+2028 or U+2029).

## `insert`

```json
{ "schema": 1, "assessments": [
  { "file": "src/A/X.cs", "unit": "N.C.Run()", "fingerprint": "4A3BFD",
    "origin": "AI", "spec": "ADR-0007 s6", "ip": "Low", "security": "High", "resources": 2,
    "falsifiedIf": "a negative value reaches the running total" },
  { "file": "src/A/X.cs", "unit": "N.C.Shim()", "exempt": "generated interop shim" }
] }
```

There is no field for the human line, and an entry carrying any undefined field
is refused. Every block records `Fingerprint=TBF` and `// Broiler-Human: PENDING`
in the owning component's padded format. `fingerprint` is the value `list`
printed for the version assessed; names are not unique, and it also says which
of two same-named units is meant.

An entry is refused when: a value is outside its vocabulary or `resources` is
not 0 to 10; `security` is `High` or `Critical` and there is no `falsifiedIf`;
the criterion is not one line of prose, states a `Key=Value` field or uses a
review word; the unit is unknown, ambiguous, exempt or already annotated; the
fingerprint is stale; the input names the unit twice (both entries are refused);
the unit is not `insertable`; or it is an exemption in an assembly closed to the
escape hatch.

The block goes directly above the declaration's first line, below its doc
comment and above its attributes, at its indentation. Each inserted line takes
the line ending of the line above it, blocks are applied bottom-up, and a UTF-8
byte-order mark is kept. A file that is not valid UTF-8 is not written. After
inserting, the file is scanned again; unless every unit and the file fingerprint
are unchanged, every new block is attached to its unit, and deleting the new
lines gives back the original text exactly, nothing is written to that file.

## Where the code lives

| Concern | Assembly |
| --- | --- |
| Block grammar (strict and lenient), vocabulary rules, configuration, classification, insertion | `Broiler.Code.Review` (`Assurance/`), BCL only |
| Units, fingerprints, trivia, directives (`CSharpAssuranceScanner`, `CSharpAssuranceFileScanner`) | `Broiler.Code.Language.CSharp.Assurance`, Roslyn from nuget.org only |
| Discovery, file I/O, JSON, commands | `Broiler.Code.Review.Cli` (`Assurance/`) |

The CLI never references `Broiler.Code.Language.CSharp.Roslyn`, whose UI package
closure comes from an authenticated feed that the review workflow does not have.

Against a scratch copy of Broiler.VM with its ten covered projects configured,
`list --all-units` reproduces `assurance.manifest.json` exactly: 235 files and
10,226 units, with the same names, exemptions and fingerprints. Stripping VM's
5,773 relevant-unit blocks and inserting them again from JSON gives back the
committed tree, apart from `Fingerprint=TBF` and two blocks VM had placed inside
a doc comment.
