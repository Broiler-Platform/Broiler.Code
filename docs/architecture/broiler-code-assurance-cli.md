# The assurance commands of `broiler-review`

- **Status:** `list`, `insert`, `generate`, `check` and `status` delivered
- **Owner:** Broiler Code
- **Recorded:** 2026-09-30

[Reviewing one declaration at a time](broiler-code-assurance.md) describes the
per-declaration annotation that `Broiler.VM` carries, and the editor pane that
records a reviewer's name on it. This record describes the command-line half,
which brings the same annotation to the other component repositories:

```text
broiler-review assurance list     --root <dir> [--files <list>] [--json <out>|-] [--all-units]
broiler-review assurance insert   --root <dir> --assessments <file.json> [--dry-run] [--json <out>|-]
broiler-review assurance generate --root <dir> [--dry-run] [--adopt]
broiler-review assurance check    --root <dir> [--config <file>] [--release] [--sources-only] [--json <out>|-]
broiler-review assurance status   --root <dir> [--config <file>]
```

Exit codes: `0` done (for `check`, nothing wrong), `1` something was refused or
`check` found a violation, `2` a usage or configuration error, reported before
anything is written.

A component adopts the scheme in four steps: commit `assurance.config.json`;
`list` the units that need a block; `insert` machine assessments for them; and
`generate`, which fills every `Fingerprint=TBF`, writes a header into every
covered file and writes the three component-level artefacts. From then on
`check` in CI holds the tree to what `generate` would write, and a reviewer's
only edit is the `// Broiler-Human:` line.

## Opting in: `assurance.config.json`

A component opts in by committing `assurance.config.json` at its root. `list`
works without it and guesses the product projects, and says so. `insert` and
`generate` refuse to run without it, so pointing the tool at a component cannot
change that component's sources. `check` and `status` read it from the root or
from `--config`, which is how a component that has not opted in (Broiler.VM,
whose own tests generate its record) can be checked without adding a file to
it. An unknown property is an error, not ignored, because a misspelled
`exclude` would otherwise cover the files it was written to leave out. Comments
and trailing commas are allowed.

| Property | Type | Default | Meaning |
| --- | --- | --- | --- |
| `schema` | `1` | `1` | Format version. |
| `component` | string | root directory name | Name used in generated prose. |
| `mode` | `"owned"` \| `"external"` | `"owned"` | `external`: another tool writes the annotations (Broiler.VM's own tests). This tool only reads; `insert` and `generate` refuse. |
| `projects` | string[] | **required** | Product `.csproj` files, root-relative with `/`. |
| `exclude` | (glob \| `{glob, reason}`)[] | `[]` | Files left out of coverage. `CODE-ASSURANCE.md` lists each one, with its reason, as not covered. |
| `spdx` | `{copyright: string[], license: string}` | none; **required** by `generate`, `check` and `status` | SPDX lines of the generated header: one or more `SPDX-FileCopyrightText` values (without the prefix) and one licence expression, e.g. `Apache-2.0 AND BSD-3-Clause`. |
| `spdxOverrides` | `{glob, copyright[], license}`[] | `[]` | SPDX lines for particular files, such as third-party-derived code. The first matching glob wins. |
| `artefacts` | `{report, humanReview, manifest}` | `CODE-ASSURANCE.md`, `HUMAN_REVIEW.md`, `assurance.manifest.json` | Where the component-level artefacts go, so that a hand-written `HUMAN_REVIEW.md` can stay where it is. |
| `preprocessorSymbols` | string[] | the owning component's net10.0 set | Symbols the scanner parses under. Code under an undefined symbol is disabled text, in no unit and no fingerprint. |
| `forbidDirectives` | bool | `false` | Whether any preprocessor directive in a covered file is a violation (rule J6). |
| `forgeryVocabulary` | `"narrow"` \| `"strict"` | `"narrow"` | Which words mark a comment as a forged summary (see J5). `strict` is the owning component's full list. |
| `closedToEscapeHatch` | string[] | `[]` | Assemblies where `EXEMPT=` may not be written. |
| `adrDirectory` | string | `docs/adr` | Where `Spec=ADR-nnnn` citations are resolved. |
| `regenerateCommand` | string | `broiler-review assurance generate` | The command generated prose tells a reader to run. The check command named beside it is the same text with the word `generate` replaced by `check`, when it occurs once. |
| `manifestComment` | string[] | the tool's own | The manifest's `$comment` lines, verbatim; an empty string is a blank line. For a component whose manifest prose is fixed by its own tests. The review-claim rule still reads it. |

Globs are matched against root-relative paths, case-sensitively: `*` and `?`
stay within a segment, `**` spans segments, and a glob with no `/` matches the
file name at any depth (`*.g.cs`).

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

`--files <list>` restricts a `list` run to the paths the list names, one per
line.

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

## `generate`

One pure function computes what every generated artefact should contain; the
command writes the ones that differ, and `check` compares them. Per covered
file, in path order:

1. **Refresh every attached block.** The AI line's `Fingerprint` becomes the
   unit's current value, whatever it held; every other field is carried through
   in source order. The human line follows the owning component's four
   transitions: `PENDING` stays `PENDING`; a reviewer with no fingerprint or
   `TBF` gets the current one, their own `IP=`, `Security=` and `Resources=`
   kept; a matching fingerprint is kept (moved last); a different one becomes
   `STALE; Previous=<reviewer>@<fingerprint>`; and a `STALE` line is left
   exactly as it is. The three lines are re-rendered at the AI line's indent,
   labels padded to 24 columns. The criterion is carried through, never
   written.
2. **Refuse an invented approval.** A human line that is not one of the four
   defined shapes (`PENDING; Fingerprint=TBF`, a bare `STALE`, `EB;`, an empty
   body), or a rewrite that would name someone the line did not, is refused
   with the owning component's message.
3. **Rescan and count.** The header is computed from the refreshed text, so it
   states the states this run leaves behind, which is what makes one run a
   fixed point.
4. **Replace the header.** When the file's first line (after any byte-order
   mark) opens with `// SPDX-`, the leading `//` run through
   `// GENERATED - DO NOT EDIT MANUALLY` and one blank line is removed, and then
   every further leading comment run that reads like a summary block. The new
   header is the file's SPDX lines (one or more copyright lines and a licence
   line, from `spdx` or the first matching `spdxOverrides`), the banner, the
   nine rows, and the marker, followed by one blank line, at line 0 of every
   covered file, including files that declare no unit.

The report, the manifest and the human-review record are then derived from every
file's units as the run leaves them.

What is refused rather than done, because the owning component's generator would
delete or overwrite something it cannot prove it wrote:

- An SPDX run with no marker, unless every line of it is a line the new header
  carries anyway (that run is replaced). Otherwise it is a licence notice.
- A line between a header's SPDX lines and its marker that is neither a header
  line nor summary vocabulary.
- A copyright line in the old header that the configuration no longer states
  for the file.
- A report, record or manifest that exists and carries no
  `GENERATED - DO NOT EDIT MANUALLY` line, unless `--adopt` is given. Fourteen
  components carry a hand-written `HUMAN_REVIEW.md`; point `artefacts.humanReview`
  elsewhere to keep one.
- A file that is not valid UTF-8, or that breaks lines on U+0085, U+2028 or
  U+2029.

Any refusal stops the whole run and nothing is written.

A comment at the top of a file that is not an SPDX line (a licence notice in
prose, `#nullable enable`, a `global using`) stays where it is, below the new
header and its blank line.

Writing keeps each file's byte-order mark and line endings. The header's lines
take the endings of the header they replace, or the ending of the line they land
above. The component artefacts are rendered with LF and written back in the
ending the file already has, and are compared in LF, so a checkout that converts
them to CRLF is not reported as stale. Every write is refused if the file changed
on disk since it was read.

## `check`

Computes the same plan in memory and reports, as GitHub workflow commands
(`::error file=<path>,line=<n>::<rule> <message>`, newlines escaped as `%0A`),
with the owning component's messages:

| Rule | What |
| --- | --- |
| J1 | A relevant unit with no parsed block. `EXEMPT=` in an assembly closed to it. |
| J2 | A block that does not parse, or an assurance comment attached to nothing (found as comment trivia, so a marker in a string is not one); a value outside its vocabulary; a criterion that is empty, states a field or claims a review; `Spec=ADR-nnnn` naming no record in `adrDirectory`. |
| J3 | Read from disk: an AI fingerprint that is `TBF` or not the current one; a human fingerprint that is neither current nor the preserved `Previous`. |
| J4 | A name on a human line in a generated artefact that no source human line carries; a line the generator refused to rewrite. |
| J5 | A generated artefact that is not what `generate` would write, naming the first differing line; more than one banner in a file; a summary line below the header; a header `generate` refused to replace. |
| J6 | A preprocessor directive, only with `forbidDirectives`. |
| J7 | The manifest against the tree: missing, extra, stale and duplicate unit and file entries. |
| J9 | Generated text stating a review word the annotations do not support, by count or by a negation in the same clause. A check on the tool's own output. |
| J10 | A unit assessed High or Critical with no criterion line. |
| J11 | With `--release`: every relevant unit left in a state that blocks a release. |
| IO | A covered file or artefact that could not be read. |

The narrow forgery vocabulary (the default) is the banner, the marker and the
nine row labels at the start of a comment, in any case. The strict one is the
owning component's list, which also matches `reviewer`, `approved` and other
words anywhere in a comment, and so matches ordinary comments in any code that
talks about reviews.

`--sources-only` compares only the covered files and the manifest's `files` and
`units` arrays with what `generate` would write, not the report, the record or
the manifest's `$comment`, and runs the review-claim rule over the source headers
and the manifest only. It is for Broiler.VM, whose own tests own that prose.

`--json <out>|-` writes `{schema, component, release, sourcesOnly,
violationCount, rules{J1: n, ...}, violations[{rule, file, line, message}]}`;
with `-`, the workflow commands move to standard error.

## `status`

A few lines for a person: files, units, relevant and exempt; how many are
annotated and human-reviewed; how many human lines read `PENDING` and `STALE`;
the count per state and per security value; and whether `generate` would write
anything.

## Against Broiler.VM

With an external configuration naming its ten projects, VM's SPDX lines, its
directive ban, the strict vocabulary and `Broiler.VM.Binary` closed to the
escape hatch, `check --sources-only` over a Broiler.VM checkout reports nothing:
every one of its 235 headers and blocks is byte-identical to what this tool
writes, and the manifest's arrays (10,226 units) are too. A test asserts that
whenever a checkout is beside this repository.

`generate` over a copy of VM, with `manifestComment` holding VM's `$comment`,
changes no source file and not the manifest; the two Markdown documents differ
only in the prose that is VM's own (its lanes, rule and exclusion numbers, mark
legend and one-person paragraph). Every table, list and figure in them is
identical. The same holds after stripping every header, resetting every AI
fingerprint to `TBF` and collapsing every block's padding: one run gives back
VM's bytes.

## Where this differs from the owning component

- A header, a licence notice or a hand-written record it cannot prove it wrote
  is refused rather than deleted or overwritten (see `generate`).
- A byte-order mark and each file's line endings are kept; VM writes UTF-8
  without a mark.
- Orphan assurance comments are found as comment trivia, not by scanning lines,
  so a marker inside a string, a block comment or disabled code is not reported.
- A `Resources` score is parsed with the invariant culture, not the current one.
- An absent or unreadable manifest is reported once, not once per unit and file.
- A difference in line endings alone is reported as such, not as "differs in
  length only".
- Refusals are collected per file and reported together; VM throws on the
  first, from a static initializer.
- The narrow forgery vocabulary is the default; VM's list is `strict`.
- The prose of the report, the record and the manifest comment names this
  component's commands and drops VM's lanes, ledgers and exclusion records.

## Where the code lives

| Concern | Assembly |
| --- | --- |
| Block grammar, human-line transitions, header strip and insert, summary, manifest, report, human-review record, review-claim rule, generator plan, checks, configuration, classification, insertion | `Broiler.Code.Review` (`Assurance/`), BCL only, text in and text out |
| Units, fingerprints, trivia, directives (`CSharpAssuranceScanner`, `CSharpAssuranceFileScanner`) | `Broiler.Code.Language.CSharp.Assurance`, Roslyn from nuget.org only |
| Discovery, file I/O, byte-order marks, line endings of artefacts, JSON, commands | `Broiler.Code.Review.Cli` (`Assurance/`) |

The CLI never references `Broiler.Code.Language.CSharp.Roslyn`, whose UI package
closure comes from an authenticated feed that the review workflow does not have.
