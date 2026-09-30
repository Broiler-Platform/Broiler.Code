# The assurance commands of `broiler-review`

- **Status:** `list`, `insert`, `generate`, `check` and `status` delivered
- **Owner:** Broiler Code
- **Recorded:** 2026-09-30

[Reviewing one declaration at a time](broiler-code-assurance.md) describes the
per-declaration annotation that `Broiler.VM` carries, and the editor pane that
records a reviewer's name on it. This record describes the command-line half,
which brings the same annotation to the other component repositories:

```text
broiler-review assurance list     --root <dir> [--files <list>] [--strict] [--json <out>|-] [--all-units]
broiler-review assurance insert   --root <dir> --assessments <file.json> [--dry-run] [--json <out>|-]
broiler-review assurance generate --root <dir> [--dry-run] [--adopt]
broiler-review assurance check    --root <dir> [--config <file>] [--release] [--sources-only] [--json <out>|-]
                                  [--annotation-prefix <path>] [--annotation-limit <n>]
broiler-review assurance status   --root <dir> [--config <file>]
```

`broiler-review` stands for the built tool. No project installs a command of that
name; a workflow runs it as
`dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance ...`,
and a component states that form in `regenerateCommand` (below) so that the
commands its messages and generated prose name are ones a reader can paste.

Exit codes: `0` done (for `check`, nothing wrong); `1` something was refused,
`check` found a violation, `list` could not read a covered file (or, with
`--strict`, was named a path that is not one), or a `--json` report could not be
written after the command's own writes; `2` a usage or configuration error,
reported before anything is written. Output, JSON included, is UTF-8 whatever the
console's code page.

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
| `component` | string (one line) | root directory name | Name used in generated prose. |
| `mode` | `"owned"` \| `"external"` | `"owned"` | `external`: another tool writes the annotations (Broiler.VM's own tests). This tool only reads; `insert` and `generate` refuse. |
| `projects` | string[] | **required** | Product `.csproj` files, root-relative with `/`. |
| `exclude` | (glob \| `{glob, reason}`)[] | `[]` | Files left out of coverage. `CODE-ASSURANCE.md` lists each one, with its reason, as not covered. |
| `excludeBuildOutputAtAnyDepth` | bool | `true` | Also leave out `bin` and `obj` directories below a project's root (their files are listed as not covered). `false` is the owning component's and the SDK's rule: only the project's own `bin` and `obj`. |
| `exemptionPredicate` | `"strict"` \| `"owning-component"` | `"strict"` | Which exemption predicate decides that a unit needs no review (see "What a unit is"). |
| `spdx` | `{copyright: string[], license: string}` | none; **required** by `generate`, `check` and `status` | SPDX lines of the generated header: one or more `SPDX-FileCopyrightText` values (without the prefix) and one licence expression, e.g. `Apache-2.0 AND BSD-3-Clause`. |
| `spdxOverrides` | `{glob, copyright[], license}`[] | `[]` | SPDX lines for particular files, such as third-party-derived code. The first matching glob wins. |
| `artefacts` | `{report, humanReview, manifest}` | `CODE-ASSURANCE.md`, `HUMAN_REVIEW.md`, `assurance.manifest.json` | Where the component-level artefacts go, so that a hand-written `HUMAN_REVIEW.md` can stay where it is. A path inside a `.git` directory is refused here; one through a link or into a nested checkout is refused when the tool runs. |
| `preprocessorSymbols` | string[] | the owning component's net10.0 set | Symbols the scanner parses under. A declaration under an undefined symbol is disabled text and no unit of its own; its text is still fingerprinted (see "What a unit is"). |
| `forbidDirectives` | bool | `false` | Whether any preprocessor directive in a covered file is a violation (rule J6). |
| `forgeryVocabulary` | `"narrow"` \| `"strict"` | `"narrow"` | Which words mark a comment as a forged summary (see J5). `strict` adds the owning component's full list. |
| `closedToEscapeHatch` | string[] | `[]` | Assemblies where `EXEMPT=` may not be written. Each must be the assembly of a configured project; a name none builds is refused. |
| `adrDirectory` | string | `docs/adr` | Where `Spec=ADR-nnnn` citations are resolved. |
| `regenerateCommand` | string | `broiler-review assurance generate` | The command generated prose and messages tell a reader to run. The `check`, `list` and `insert` commands named beside it are the same text with the word `generate` replaced, when it occurs once. Set it to the `dotnet run --project ... -- assurance generate --root <component>` form the component's workflow uses. |
| `manifestComment` | string[] | the tool's own | The manifest's `$comment` lines, verbatim; an empty string is a blank line. For a component whose manifest prose is fixed by its own tests. The review-claim rule still reads it. |

Globs are matched against root-relative paths, case-sensitively: `*` and `?`
stay within a segment, `**` spans segments, and a glob with no `/` matches the
file name at any depth (`*.g.cs`).

## Which files are covered

Every `*.cs` under each configured project's directory, ordered by root-relative
path (ordinal), and every `*.cs` a configured project compiles in from outside
its directory through a `<Compile Include>` it states literally: in the project
file, in a `Directory.Build.props` or `Directory.Build.targets` between the
project and the root, or in a file one of those imports by a literal path. Such a
file is credited to the including project's assembly, unless another project's
walk covers it. A wildcard include is expanded; `$(MSBuildThisFileDirectory)` and
`$(MSBuildProjectDirectory)` are resolved; a `Condition` is ignored and a
`Remove` is not applied, because the bias is to cover what may be compiled.

Not entered, and **each listed as not covered with its reason**, in
`CODE-ASSURANCE.md`, `status` and `list`, so that the record never claims a file
it left out:

- `bin` and `obj` directories below a project's root, with `excludeBuildOutputAtAnyDepth`
  (the default). Some components track stale `obj/**/*.AssemblyAttributes.cs`
  files. Each file under one is listed. A project's own `bin` and `obj` are its
  build output and are left out without a word, as the SDK leaves them out.
- A directory holding a `.git` entry: a nested checkout of another component,
  or an untracked stale copy of one. It is listed as a directory (`path/`). A
  configured project inside one is refused.
- A junction or symbolic link, directory or file. It is not followed, because a
  link can lead out of the component and a write through it lands there; it is
  listed. A configured project reached through one is refused.
- A file a `<Compile Include>` names outside the component root, which the tool
  neither reads nor writes, and an include whose value refers to a property the
  tool cannot evaluate (listed by its value).
- Files matching an `exclude` glob.

Another configured project's directory is walked for that project, so no file is
covered twice. The assembly name is read from the project file's unconditional
`AssemblyName`, or is its file name; a conditional `AssemblyName`, one naming a
property other than `$(MSBuildProjectName)`, and one set in a
`Directory.Build.props` or `.targets` are refused rather than guessed.

`--files <list>` restricts a `list` run to the paths a file names, one per line;
blank lines and lines starting with `#` are ignored. A path may be root-relative
or absolute under `--root`, with either slash, a `./` or doubled slashes; on
Windows it may differ from the file in case. A path that names no covered file
is reported with why (it does not exist, it is excluded and why, it differs from
a covered file in case only, or it is outside the root) as a note and in
`unknownFiles`; with `--strict` it makes the run exit `1`.

## What a unit is

Units, names and fingerprints are the owning component's, with these additions,
each of which changes nothing on that component's tree (asserted by the test
over a Broiler.VM checkout):

- **Names are unique within a file.** Three shapes gave two units one name: a
  partial type declared twice in one file, overloaded indexers (both `this[]`),
  and members of `Foo` and `Foo<T>` side by side. A name that would repeat first
  takes the detail the plain name leaves out (`Foo<T>.Bar()`, `this[int]`), and
  a name that still repeats takes `#2`, `#3` in document order after its first
  occurrence (`N.DomBridge#2`). A name that did not repeat keeps the owning
  component's form. A name never spans lines: whitespace around a line break
  inside a parameter type becomes one space.
- **Line endings move nothing.** A line break inside a token (a verbatim or raw
  string) is hashed as LF, so a CRLF working tree and an LF checkout of the same
  commit give the same unit, file and manifest fingerprints, and a generate on
  either platform leaves an approval as it is.
- **Directives and disabled text are in the fingerprint.** A directive's tokens,
  and the tokens of the text a directive disables, are hashed with the unit they
  sit in (and all of them with the file), so the `#else` branch of an approved
  method cannot be rewritten under the approval. Tokenized, so rewrapping it
  moves nothing. A directive or disabled text above a unit is not that unit's; a
  whole declaration under an undefined symbol is still no unit, and only the
  file fingerprint watches it.
- **Top-level statements are one unit**, `<top-level statements>`, relevant, from
  the first statement to the last, local functions included.
- **The strict exemption predicate** (the default) narrows four of the eight
  cases where they would exempt code that runs: a field or auto-property is
  exempt only when its initializer, if any, is inert (a literal, a name,
  `default`, `nameof`, `typeof`, a parameterless `new()`, or arithmetic,
  conditions, casts and arrays over those); a `throw new` expression body only
  when the exception's arguments are inert; a property initializer that runs
  code makes a record member the source's rather than the compiler's; and a
  member of a type named `AssemblyMarker` gets no exemption for where it lives.
  It never exempts a unit the owning component's predicate does not.
  `"exemptionPredicate": "owning-component"` is that predicate exactly.

## `list`

By default, the relevant units that carry no block the owning component would
attach, which is the work left to do. `--all-units` lists every unit. Each unit
has `file`, `assembly`, `line` and `column` (1-based, of the declaration's first
token, attributes included), `indent`, `unit` (the qualified name the manifest
uses, unique within the file), `displayName`, `kind`, `fingerprint`,
`extent {startLine, endLine}`, `exempt`, `exemption`, `state`, `insertable`,
`reason` and, when it helps, `detail`. Per-file and total counts are included,
and `totals.unknown` with `unknownFiles[{file, reason}]` for a `--files` run.
`excluded` lists what is not covered; with `--files`, only the named paths.

`reason` is `none` when a block can be inserted, otherwise one of `exempt`,
`annotated`, `not-own-line`, `malformed-block` (the attached AI line does not
parse), `half-block` (a human or criterion line with no AI line),
`below-declaration` (an assurance comment inside the declaration's header), or
`line-model-mismatch` (the file breaks lines on U+0085, U+2028 or U+2029).

A covered file that cannot be read is listed with its problem and makes the run
exit `1`: a run that did not read everything has not listed everything.

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
in the owning component's padded format. `unit` is the name `list` printed, which
identifies one unit of the file; `fingerprint` is the value `list` printed for the
version assessed, and an entry whose fingerprint the unit no longer computes is
refused as stale. `file` is read as a `--files` path is. `resources` is any
integral JSON number (`2` or `2.0`).

An entry is refused when: a value is outside its vocabulary or `resources` is
not an integer 0 to 10; `security` is `High` or `Critical` and there is no
`falsifiedIf`; the criterion is not one line of prose, states one of this
format's fields (`Security=Low`, `Fingerprint=...`; any other `Key=Value`, such
as `SameSite=None`, is prose) or claims a review; the `exempt` reason or the
`spec` claims a review; the unit is unknown, exempt or already annotated; the
fingerprint is stale; the input names the unit twice (both entries are refused);
the unit is not `insertable`; or it is an exemption in an assembly closed to the
escape hatch. A review claim is one of `verified`, `approve` and its forms,
`reviewer`, `reviewed by`, `human review` and its forms, `eligible for release`,
`signed off`, `sign-off`, `certified`, `attested`, `LGTM`, `looks good to me` and
`checked by`, as whole words in any case (`unverified` is not one).

The block goes directly above the declaration's first line, below its doc
comment and above its attributes, at its indentation. Each inserted line takes
the line ending of the line above it, blocks are applied bottom-up, and a UTF-8
byte-order mark is kept. A file that is not valid UTF-8 is not written. After
inserting, the file is scanned again; unless every unit and the file fingerprint
are unchanged, every new block is attached to its unit, and deleting the new
lines gives back the original text exactly, nothing is written to that file.

A `--json` target that cannot be written is refused before anything is read or
written.

## `generate`

One pure function computes what every generated artefact should contain; the
command writes the ones that differ, and `check` compares them. Per covered
file, in path order:

1. **Refresh every attached block.** The AI line's `Fingerprint` becomes the
   unit's current value, whatever it held; every other field is carried through
   in source order. The human line follows the owning component's four
   transitions, with the binding narrowed: `PENDING` stays `PENDING`; a reviewer
   with no fingerprint or `TBF` gets the current one **only when the AI line
   records the current one** — the version the last generation wrote down and
   the one the reviewer can have been reading — and otherwise becomes
   `STALE; Previous=<reviewer>@<recorded fingerprint>`, or is refused when the AI
   line records none yet; their own `IP=`, `Security=` and `Resources=` are kept;
   a matching fingerprint is kept (moved last); a different one becomes
   `STALE; Previous=<reviewer>@<fingerprint>`; and a `STALE` line is left
   exactly as it is. The three lines are re-rendered at the AI line's indent,
   labels padded to 24 columns. The criterion is carried through, never
   written.
2. **Refuse an invented approval.** A human line that is not one of the four
   defined shapes (`PENDING; Fingerprint=TBF`, a bare `STALE`, `EB;`, an empty
   body), or a rewrite that would name someone the line did not, is refused
   with the owning component's message. A reviewer is an **alias**: it opens
   with a letter, holds letters, digits, `.`, `_`, `-`, `'` and single spaces, is
   at most 64 characters long, and has no placeholder word in it (`PENDING`,
   `STALE`, `TBF`, `TODO`, `NONE`, `NOT`, `REVIEWED`, `LGTM` and the like, in any
   case). `NOT REVIEWED`, `Pending` and `PENDING` followed by a zero-width space
   are therefore refused, not sealed into approvals; the editor holds a name to
   the same rule.
3. **Rescan and count.** The header is computed from the refreshed text, so it
   states the states this run leaves behind, which is what makes one run a
   fixed point; a file whose written text would not carry what the header
   counts is refused.
4. **Replace the header.** When the file's first line (after any byte-order
   mark) opens with `// SPDX-`, the leading `//` run through
   `// GENERATED - DO NOT EDIT MANUALLY` and one blank line is removed, and then
   every further leading run that is a copy of the generated block (every line
   one the generator writes, through a marker of its own). A leading comment
   that merely uses the summary's words is left where it is, so a second run
   gives the same bytes as the first. The new header is the file's SPDX lines
   (one or more copyright lines and a licence line, from `spdx` or the first
   matching `spdxOverrides`), the banner, the nine rows, and the marker,
   followed by one blank line, at line 0 of every covered file, including files
   that declare no unit.

The report, the manifest and the human-review record are then derived from every
file's units as the run leaves them. The report states what the human lines
record ("No code unit in this component carries a decision on its human line
yet"), and names a hand-written review record at an artefact's default path
(`HUMAN_REVIEW.md` while `artefacts.humanReview` points elsewhere) as a separate
record it neither reads nor summarizes.

What is refused rather than done, because the owning component's generator would
delete or overwrite something it cannot prove it wrote:

- An SPDX run with no marker, unless every line of it is a line the new header
  carries anyway (that run is replaced). Otherwise it is a licence notice:
  state exactly its lines in `spdx` or an `spdxOverrides` entry, and the header
  replaces it.
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

Every refusal is reported in the same run, and while any stands nothing is
written. A write that fails once writing has begun (the file changed on disk
since it was read, or could not be written) is reported and does not undo the
writes before it; sources are written before the component artefacts, so `check`
then reports the record as stale, and the next run writes the rest.

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
| J2 | A block that does not parse, or an assurance comment attached to nothing (found as comment trivia, so a marker in a string is not one); a value outside its vocabulary; a criterion that is empty, states one of this format's fields or claims a review; an `EXEMPT=` reason or a `Spec=` that claims a review; `Spec=ADR-nnnn` naming no record in `adrDirectory`. |
| J3 | Read from disk: an AI fingerprint that is `TBF` or not the current one; a human fingerprint that is neither current nor the preserved `Previous`. |
| J4 | A name on a human line in a generated artefact that no source human line carries; a line the generator refused to rewrite or bind. |
| J5 | A generated artefact that is not what `generate` would write, naming the first differing line; more than one banner in a file; a summary line below the header; a header `generate` refused to replace. |
| J6 | A preprocessor directive, only with `forbidDirectives`. |
| J7 | The manifest against the tree: missing, extra, stale and duplicate unit and file entries, and anything at its top beside `$comment`, `files` and `units`. |
| J9 | Generated text stating a review word the annotations do not support, by count or by a negation in the same clause. A check on the tool's own output, and with `--sources-only` on the manifest as it is on disk. |
| J10 | A unit assessed High or Critical with no criterion line. |
| J11 | With `--release`: every relevant unit left in a state that blocks a release. |
| IO | A covered file or artefact that could not be read. |

Where a violation has a remedy, a `Run:` line follows the message: J1 names the
`list` and `insert` commands, J3 and J7 the `generate` command (J5's message
carries it already). A J7 violation about a unit or file of the tree is anchored
at that unit's line or that file, so a pull request shows it beside the change;
only an entry the tree lacks and the manifest's own shape are anchored at the
manifest. An artefact that does not exist is reported as the owning component
reports it (line 1, compared with an empty file), followed by a line saying it
does not exist.

A summary line is found in every comment the parser sees — `//`, `///`,
`/* */` and disabled text — and compared after its delimiters are removed, its
whitespace collapsed and its case folded. The narrow vocabulary (the default)
is the banner or the marker anywhere in a comment, and the nine row labels at
the start of one followed by a colon (a space may stand for the hyphen in
`Human-reviewed`). The strict one adds the owning component's list, which also
matches `reviewer`, `approved` and other words anywhere in a comment, and so
matches ordinary comments in any code that talks about reviews.

`--sources-only` compares only the covered files and the manifest's `files` and
`units` arrays with what `generate` would write, not the report, the record or
the manifest's `$comment`. It still runs the review-claim rule over the source
headers and over the manifest as it is on disk, and J7 still holds the manifest's
top-level shape. It is for Broiler.VM, whose own tests own that prose.

`--annotation-prefix <path>` puts a path before every `file=`, for a workflow
that runs from a directory above the component (`--annotation-prefix Broiler.Net`).
`--annotation-limit <n>` writes at most `n` workflow commands per rule and then a
line counting the rest; a runner shows a handful of annotations per step, and a
component that has just adopted the scheme has thousands. The summary line and
the JSON report count every violation.

`--json <out>|-` writes `{schema, component, release, sourcesOnly,
violationCount, rules{J1: n, ...}, violations[{rule, file, line, message,
remedy}]}`; with `-`, the workflow commands move to standard error.

## `status`

A few lines for a person: files covered and not covered, units, relevant and
exempt; how many are annotated and human-reviewed; how many human lines read
`PENDING` and `STALE`; the count per state and per security value; and whether
`generate` would write anything.

## Against Broiler.VM

With an external configuration naming its ten projects, VM's SPDX lines,
`"excludeBuildOutputAtAnyDepth": false`, `"exemptionPredicate": "owning-component"`,
its directive ban, the strict vocabulary and `Broiler.VM.Binary` closed to the
escape hatch, `check --sources-only` over a Broiler.VM checkout reports nothing:
every one of its 235 headers and blocks is byte-identical to what this tool
writes, and the manifest's arrays (10,226 units) are too. A test asserts that
whenever a checkout is beside this repository.

`generate` over a copy of VM, with `manifestComment` holding VM's `$comment`,
changes no source file and not the manifest; the two Markdown documents differ
only in prose: VM's own (its lanes, rule and exclusion numbers, commands, mark
legend and one-person paragraph), this tool's opening sentence, and the
"Files not covered" row and section VM's report does not have. Every count,
state table, distribution and unit list in them is identical. The same holds
after stripping every header, resetting every AI fingerprint to `TBF` and
collapsing every block's padding: one run gives back VM's bytes.

## Where this differs from the owning component

- A header, a licence notice or a hand-written record it cannot prove it wrote
  is refused rather than deleted or overwritten, and a leading comment under the
  header that uses the summary's words is left for the check to report rather
  than deleted (see `generate`).
- A byte-order mark and each file's line endings are kept; VM writes UTF-8
  without a mark.
- A line break inside a token is fingerprinted as LF, a name never spans lines,
  directives and disabled text are fingerprinted, names are unique within a
  file, and top-level statements are a unit (see "What a unit is"). None of
  these changes a VM name or fingerprint.
- The strict exemption predicate is the default; VM's is `owning-component`.
- `bin` and `obj` are left out at any depth by default, and whatever the walk
  does not enter (build output below a project's root, nested checkouts, links)
  is listed as not covered; VM leaves out only a project's own `bin` and `obj`,
  follows links and has no nested checkout. Files a project compiles in through a
  literal `<Compile Include>` are covered; VM's covered set is its directory walk.
- A reviewer is an alias in shape, not any text without an `=`, and a bare alias
  is bound only to the version the AI line records; VM fills whatever the code is
  when the generator runs.
- A criterion's review words are matched as whole words, and only this format's
  own fields count as a field on it; VM matches substrings and any `Key=Value`.
  An `EXEMPT=` reason and a `Spec=` may not claim a review either.
- Orphan assurance comments and forged summary lines are found as comment
  trivia, not by scanning lines, so a marker inside a string is not reported and
  one in a block comment, a documentation comment or disabled text is; a forged
  summary is matched after its whitespace and case are normalized.
- A `Resources` score is parsed with the invariant culture, not the current one.
- An absent or unreadable manifest is reported once, not once per unit and file;
  its line is 1 and a second line says the file does not exist.
- J7 messages are ordered by (file, name) ordinally, not with the culture-sensitive
  default comparer, so their order does not depend on the machine's locale; a J7
  about a unit or file of the tree is anchored there, not at the manifest.
- J1, J3 and J7 carry a `Run:` remedy beside VM's message.
- A difference in line endings alone is reported as such, not as "differs in
  length only".
- Refusals are collected and reported together; VM throws on the first, from a
  static initializer.
- The narrow forgery vocabulary is the default; VM's list is `strict`.
- The prose of the report, the record and the manifest comment names this
  component's commands and drops VM's lanes, ledgers and exclusion records, and
  the report's opening sentence speaks of the human lines, not of the component.

## Where the code lives

| Concern | Assembly |
| --- | --- |
| Block grammar, alias rule, human-line transitions, header strip and insert, summary, manifest, report, human-review record, review-claim rule, generator plan, checks, configuration, classification, insertion | `Broiler.Code.Review` (`Assurance/`), BCL only, text in and text out |
| Units, names, fingerprints, both exemption predicates, trivia, comments, directives (`CSharpAssuranceScanner`, `CSharpAssuranceFileScanner`) | `Broiler.Code.Language.CSharp.Assurance`, Roslyn from nuget.org only |
| Discovery (walk, `<Compile Include>`, links, checkouts, assembly names), path normalization, file I/O, byte-order marks, line endings of artefacts, JSON, commands | `Broiler.Code.Review.Cli` (`Assurance/`) |

The CLI never references `Broiler.Code.Language.CSharp.Roslyn`, whose UI package
closure comes from an authenticated feed that the review workflow does not have.
