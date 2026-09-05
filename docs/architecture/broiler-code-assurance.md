# Reviewing one declaration at a time

- **Status:** Delivered on Windows and Linux
- **Owner:** Broiler Code, with the platform assurance owner consulted
- **Recorded:** 2026-09-05

The [Human Review workspace](broiler-code-review.md) answers *has a person read
this file?* Some components ask a narrower question — *has a person put their
name to this declaration?* — and answer it in the source itself, one code unit
at a time.

`Broiler.VM` is the component that does. Every relevant declaration in it carries
two comment lines: what a machine assessed, and what a human has said.

```csharp
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4A3BFD
// Broiler-Human:        PENDING
public bool IsWellFormed => …
```

All 1,697 of them say `PENDING`, and that is the point: the absence of review,
measured precisely. Moving one off `PENDING` meant opening the file and typing
into a comment at a column the component's parser insists on. This is the pane
that does it instead.

## What it is

A third section in the existing review pane, above the file's own record:

```text
┌──────────────────────┬──────────────────────────────────┬──────────────────────────────┐
│ Solution Explorer    │ VmArtifactDescriptor.cs          │ Assurance   IsWellFormed     │
│                      │                                  │  State      needs human rev… │
│ ▼ Broiler.VM         │   // Broiler-AI:  Origin=AI; …   │  Origin     AI               │
│   VmArtifactDesc…    │   // Broiler-Human:   PENDING    │  IP risk    Low              │
│     needs review     │   public bool IsWellFormed =>    │  Security   Low              │
│                      │  ▏    !ProfileId.IsEmpty &&      │  Fingerprint 4A3BFD          │
│                      │       FormatVersion >= 1 &&      │  Human line PENDING          │
│                      │                                  │                              │
│                      │                                  │ Human Review  0 of 4 reviewed│
│                      │                                  │ Units                        │
│                      │                                  │  VmArtifactDescriptor        │
│                      │                                  │  IsWellFormed                │
└──────────────────────┴──────────────────────────────────┴──────────────────────────────┘
```

The caret chooses the unit. Moving it into a class, a method, a property or an
operator fills the section with what that declaration's annotation records;
activating a row in **Units** moves the caret the other way, so the pane is a way
through a file rather than a readout of wherever somebody already was.
**Review ▸ Sign Unit as Reviewed** writes the reviewer's name onto the
declaration. **Withdraw Unit Signature** puts it back to `PENDING`.

## The decisions

### The editor writes a name and never a fingerprint

This is the load-bearing decision, and it is a restriction rather than a feature.

The human line's `Fingerprint=` field is what binds an approval to a version of
the code: the unit's fingerprint today equal to the one a reviewer approved is
the entire definition of `VERIFIED`, and different is the entire definition of
`STALE`. The owning component's generator writes that field. Its own state
machine records the rule the whole scheme rests on — only a human may create an
approval, and no automated step may produce `VERIFIED` or a reviewer identifier
from a source that does not already carry one.

So this editor writes the bare name. That is exactly the shape the generator
expects from a person; it fills the fingerprint in on its next run, over the tree
that was actually committed. The consequence worth stating plainly is that
**signing a unit here cannot make it reviewed** — it moves it to
`HUMAN_APPROVED_PENDING_FINGERPRINT`, which still blocks a release — and that is
the correct outcome, not a missing step. An editor that wrote the fingerprint
too could turn one keystroke into a completed approval of code nobody read.

It is the same rule as `FileReview.WithDecision` hashing its own content, and the
same rule as there being no `broiler-review mark` command: the one way to make
this record worthless is to let something approve code without a human reading
it, so the tool is built unable to.

### The generated file header is rewritten only when this build can reproduce it

Every annotated file opens with a generated block that counts its units:

```text
// Relevant units:   4
// Annotated:        4/4
// Exempt:           9
// Human-reviewed:   0/4
…
// GENERATED - DO NOT EDIT MANUALLY
```

Recording a review can move two of those numbers, so leaving the block alone
makes a file contradict its own annotations — in the half of the file nobody
reads expecting it to be out of date. But writing a block from counts that
differ from the owning component's would be worse: the result looks generated
and is wrong.

So the pane rewrites the header only after reproducing, byte for byte, the
header the file already carries — from its own reading of that same file. That
is the only evidence available that this build counts the way the component that
owns the format counts, and it is evidence about the file being changed rather
than about files in general.

It fails closed, and the failure mode is the useful one. Counting units needs the
exemption predicate and a fingerprint per unit; a host that composes no C# parser
has neither, produces no summary, cannot reproduce the header, and therefore
never touches it — while still recording the reviewer's line, which is the part
a human owns. The same guard covers a file whose header was hand-edited, or one
this build's predicate reads differently: the signature lands, the header is left
alone, and nothing silently claims a number nobody computed.

### Two levels of confidence, and the pane says which

Reading the annotation blocks needs no parser: a comment is a comment, and the
blocks are found by their markers. That level shows what every annotated
declaration records and writes a reviewer's decision onto it — the whole of what
a human writes — on any host, in any language the format is ever applied to.

Finding *every* unit, deciding which are exempt, and computing a fingerprint
needs a real C# parser. `IAssuranceUnitScanner` is that seam and
`CSharpAssuranceScanner` is its one implementation, in
`Broiler.Code.Language.CSharp.Roslyn` where Roslyn already lives. The desktop
heads compose it; `Broiler.Code.Core` does not reference it, which is the
constraint Phase 0's payload probes produced and which
`CodeEditorArchitectureTests` still asserts.

Where the second level is missing, the difference is reported rather than
guessed. A unit whose approval names a fingerprint this build cannot compute
reports `fingerprint unknown`, never `STALE`: telling a reviewer their
colleague's approval has lapsed, on no evidence, is how a warning gets trained
out of people. The review model already draws the same distinction between
`Stale` and `Unknown`, for the same reason.

### A signature is an ordinary edit in the buffer

The rewrite is submitted to the open document, not written to storage. It is
undoable, it shows up in the editor before it is committed, and saving stays
where it already was. Only the one line changes: every other line keeps its own
text and its own line ending, so a file that mixes CRLF and LF comes back mixed
the same way. A review that reformatted a file would invalidate every other
reviewer's content hash on the way past and bury the one line that matters.

### Signing is allowed on a document with unsaved changes

The four file-level decisions are not, and the difference is deliberate rather
than an inconsistency. A file review records the hash of the content a person
read, so unsaved text — content nobody else can fetch — would make it
unverifiable by CI, by a second reviewer, or by the same reviewer tomorrow. A
signature records a name and no claim about content at all; the claim is the
generator's, over the committed tree. There is nothing here for unsaved text to
make unverifiable, and a reviewer working down a file would otherwise have to
save between every declaration.

### A name that the format cannot carry is refused

There is no roster of permitted reviewers anywhere in the format, and nothing
here refuses anybody on the grounds of who they are. What is refused is a name
that would not survive the round trip: the human line is split on `;`, and
staleness is later recorded as `Previous=name@fingerprint`, so a name carrying
either delimiter would come back as a different name — or as a body nothing
recognizes. `PENDING` and `STALE` are refused for the same reason.

## What this closes in the review workspace

Two of the limitations [that record](broiler-code-review.md) lists are no longer
true:

- **Note kinds.** The pane now carries a picker, so `Concern`, `Todo` and
  `Observation` are reachable. They were in the record format, read back and
  rendered, and unreachable because there was nowhere to choose.
- **Symbol anchors.** A note written while the caret is inside a declaration
  records that declaration's qualified name. The field has been in the format
  from the start and nothing had filled it in, because nothing in the shell knew
  what a declaration was. It is still display and search only — never the thing
  that decides where a note goes, which is still the anchored text.

## Ownership

| Concern | Owner | Boundary |
| --- | --- | --- |
| Annotation grammar, the state machine, the header's arithmetic, the rewrite | `Broiler.Code.Review` | References `Broiler.Code.Workspaces` and nothing else. No UI, no parser, no platform. |
| Units, the exemption predicate, fingerprints | `Broiler.Code.Language.CSharp.Roslyn` | The one place a C# parser is needed, behind `IAssuranceUnitScanner` |
| The caret, the pane sections, the commands, the buffer edit | `Broiler.Code.Core` | The seam that knows about both a workspace and a screen |
| Composing a scanner | `Broiler.Code.Windows`, `.Linux` | Optional, so a head that composes none still gets the annotation-text reading |

## How agreement with the owning component is asserted

`CSharpAssuranceScanner` is a second implementation of an algorithm defined in
another repository's test assembly, which cannot be referenced from here. So
agreement is asserted against evidence that component already published: a file
from it is kept verbatim as a fixture, and the tests assert the four fingerprints
its generator wrote into that file, the nine more its `assurance.manifest.json`
records, the file's own fingerprint, the thirteen-unit split its header states,
and the exemption case each of the nine falls under.

A failure there does not mean a number moved. It means this editor and that
component have stopped agreeing about what a unit is or what a fingerprint
covers — and the editor is the one that is wrong, which is why every expected
value is written out as a literal rather than computed.

## What this does not do

- **It does not seal an approval.** By design; see the first decision above. A
  unit signed here is reviewed once that component's generator has run.
- **It does not write the reviewer's own risk assessment.** The format lets a
  human record `IP=`, `Security=` or `Resources=` beside their name, disagreeing
  with the machine. Such an assessment is parsed, shown, and preserved when the
  same reviewer signs again — but there is no way to enter one.
- **It does not report what moved under a stale approval.** The token stream a
  fingerprint is taken over is available and is not shown, so a reviewer whose
  approval lapsed is told that it did and not why.
- **It does not touch the component-level artefacts.** `CODE-ASSURANCE.md`, the
  manifest and the human-review summary are the generator's; only the per-file
  header is recounted here.
- **It does not create an annotation.** A relevant declaration carrying none is
  reported as such and cannot be signed. Writing the machine's assessment line is
  an assessment, not a review.
