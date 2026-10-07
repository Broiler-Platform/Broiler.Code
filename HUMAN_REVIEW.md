# Human Review: Broiler.Code

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance generate --root Broiler.Code`, which rewrites this file,
`CODE-ASSURANCE.md`, `assurance.manifest.json` and every generated source header from the
product tree.

> **Status: PENDING.** Human-reviewed: 0 of 1472 relevant units. `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Code --release`
> fails while any relevant unit is without a decision bound to its current fingerprint.

## 1. How To Use This File

Read it; do not edit it. A decision about a code unit is the `// Broiler-Human:` line on that
unit's declaration, and every table below is read out of those lines. There is nothing here
to fill in and nothing here to leave blank.

## 2. How A Review Is Recorded

In one place: the `// Broiler-Human:` line of the assurance annotation that sits on the
declaration being read. Nothing in this file is edited by hand, no second document carries a
per-item checklist, and no list of permitted aliases exists to be added to.

```csharp
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=4A3BFD
// Broiler-Falsified-If: a negative value reaches the running total
// Broiler-Human:        PENDING
```

The last line has four shapes. A human writes three of them; the generator writes the fourth
and may never invent an alias, which the check asserts in both directions.

| Line | Meaning |
|---|---|
| `PENDING` | Nobody has recorded a decision for this unit. The generator leaves it exactly as it stands. |
| `<alias>` | A human states their own alias and leaves the machine field to the generator, which fills it with the declaration's fingerprint at the next run. |
| `<alias>; Fingerprint=<six hex>` | A decision bound to one exact version of one declaration. |
| `STALE; Previous=<alias>@<fingerprint>` | Written by the generator when the code moved after a decision. Only a human clears it, by stating their alias again. |

A human may state their own `IP=`, `Security=` and `Resources=` assessment beside their alias,
which is how a reader disagrees with the machine assessment on the line above: an assessment is
a comment and moves no fingerprint, so there is nowhere else to say it.

**No branch, commit or tag is recorded in this file.** Each decision names the fingerprint of
the declaration it was made against, and the state machine compares that value with the
declaration as it now stands. A commit says a tree moved; a fingerprint says whether this unit
did, which is the narrower and the more useful of the two.

## 3. Summary

| Metric | Value |
|---|---:|
| Files scanned | 102 |
| Code units | 2119 |
| Relevant | 1472 |
| Exempt | 647 |
| Assessed | 1472 of 1472 (100%) |
| Human reviewed | 0 of 1472 (0%) |
| Unverified | 1472 |
| Aliases naming a decision | 0 |

## 4. Review States

One row per state of the machine that reads the two lines. The states are computed from the
annotations and the current fingerprints; nothing stores them.

| State | Units |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 1472 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 647 |

## 5. Aliases In The Tree

No alias appears on a human line anywhere in the product tree. Nobody has recorded a
decision about any unit of this component.

## 6. Coverage By File

One row per covered file, carrying that file's generated header. `Unverified` counts the
relevant units in a state that blocks a release.

| File | Units | Relevant | Exempt | Unverified | IP risk | Security risk | Criteria |
|---|---:|---:|---:|---:|---|---|---:|
| `src/Broiler.Code.Core/CodeAnalysisController.cs` | 26 | 11 | 15 | 11 | Low | High | 11/7 |
| `src/Broiler.Code.Core/Diagnostics/DiagnosticMerge.cs` | 17 | 9 | 8 | 9 | Low | Medium | 8/0 |
| `src/Broiler.Code.Core/Diagnostics/ProblemsModel.cs` | 21 | 17 | 4 | 17 | Low | Medium | 14/0 |
| `src/Broiler.Code.Core/Hosting/DesktopInputRouter.cs` | 18 | 14 | 4 | 14 | Low | High | 14/2 |
| `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` | 20 | 13 | 7 | 13 | Low | High | 13/7 |
| `src/Broiler.Code.Core/Hosting/HostServiceReport.cs` | 16 | 13 | 3 | 13 | Low | Low | 6/0 |
| `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` | 10 | 6 | 4 | 6 | Low | High | 6/6 |
| `src/Broiler.Code.Core/Review/AssuranceController.cs` | 43 | 25 | 18 | 25 | Low | High | 22/12 |
| `src/Broiler.Code.Core/Review/GitCommand.cs` | 5 | 5 | 0 | 5 | Low | High | 5/5 |
| `src/Broiler.Code.Core/Review/GitRevisionProvider.cs` | 5 | 4 | 1 | 4 | Low | High | 4/2 |
| `src/Broiler.Code.Core/Review/ReviewController.cs` | 44 | 25 | 19 | 25 | Low | High | 21/12 |
| `src/Broiler.Code.Core/Review/ReviewPaneSource.cs` | 42 | 32 | 10 | 32 | Low | Medium | 25/0 |
| `src/Broiler.Code.Core/Shell/CodeCommands.cs` | 46 | 33 | 13 | 33 | Low | Medium | 11/0 |
| `src/Broiler.Code.Core/Shell/CodeShell.cs` | 131 | 79 | 52 | 79 | Low | High | 69/36 |
| `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` | 31 | 18 | 13 | 18 | Low | High | 11/7 |
| `src/Broiler.Code.Core/Shell/FileDialogs.cs` | 14 | 7 | 7 | 7 | Low | High | 4/4 |
| `src/Broiler.Code.Core/Shell/ProblemsTreeSource.cs` | 16 | 11 | 5 | 11 | Low | Medium | 7/0 |
| `src/Broiler.Code.Core/Shell/SolutionExplorerSource.cs` | 27 | 20 | 7 | 20 | Low | Medium | 12/0 |
| `src/Broiler.Code.Core/Shell/WorkspaceBootstrap.cs` | 7 | 7 | 0 | 7 | Low | High | 7/4 |
| `src/Broiler.Code.Core/SourceBufferDocument.cs` | 31 | 18 | 13 | 18 | Low | Medium | 6/0 |
| `src/Broiler.Code.Core/Templates/CodeTemplateService.cs` | 16 | 13 | 3 | 13 | Low | High | 9/3 |
| `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` | 15 | 12 | 3 | 12 | Low | High | 12/9 |
| `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` | 75 | 62 | 13 | 62 | Low | High | 61/50 |
| `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` | 14 | 10 | 4 | 10 | Low | High | 9/9 |
| `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` | 13 | 10 | 3 | 10 | Low | High | 9/4 |
| `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` | 16 | 11 | 5 | 11 | Low | Critical | 11/7 |
| `src/Broiler.Code.Language.CSharp.Roslyn/EvaluatedProjectGraph.cs` | 32 | 17 | 15 | 17 | Low | High | 15/2 |
| `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` | 15 | 15 | 0 | 15 | Low | High | 15/9 |
| `src/Broiler.Code.Language.CSharp.Syntax/KeywordTable.cs` | 7 | 7 | 0 | 7 | None | Medium | 7/0 |
| `src/Broiler.Code.Language.CSharp.Syntax/LineState.cs` | 8 | 3 | 5 | 3 | None | Medium | 2/0 |
| `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` | 16 | 12 | 4 | 12 | Low | High | 11/5 |
| `src/Broiler.Code.Linux/CodeHost.cs` | 4 | 4 | 0 | 4 | Low | High | 4/2 |
| `src/Broiler.Code.Linux/CodeShellFactory.cs` | 4 | 4 | 0 | 4 | Low | High | 4/3 |
| `src/Broiler.Code.Linux/CodeWindow.cs` | 34 | 18 | 16 | 18 | Low | High | 13/6 |
| `src/Broiler.Code.Linux/LinuxCodeInput.cs` | 31 | 16 | 15 | 16 | Low | High | 13/7 |
| `src/Broiler.Code.Linux/LinuxFileDialogs.cs` | 20 | 15 | 5 | 15 | Low | High | 10/10 |
| `src/Broiler.Code.Linux/Program.cs` | 2 | 2 | 0 | 2 | Low | High | 2/2 |
| `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` | 35 | 35 | 0 | 35 | Low | High | 17/12 |
| `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` | 15 | 15 | 0 | 15 | Low | High | 13/9 |
| `src/Broiler.Code.Review.Cli/Assurance/AssuranceJson.cs` | 19 | 19 | 0 | 19 | Low | High | 14/4 |
| `src/Broiler.Code.Review.Cli/Assurance/AssurancePruneCommand.cs` | 6 | 6 | 0 | 6 | Low | High | 3/3 |
| `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` | 12 | 8 | 4 | 8 | Low | High | 8/7 |
| `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` | 14 | 8 | 6 | 8 | Low | High | 8/6 |
| `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` | 31 | 31 | 0 | 31 | Low | High | 21/21 |
| `src/Broiler.Code.Review.Cli/Program.cs` | 1 | 1 | 0 | 1 | Low | High | 1/1 |
| `src/Broiler.Code.Review.Cli/ReviewReport.cs` | 8 | 8 | 0 | 8 | Low | High | 5/3 |
| `src/Broiler.Code.Review.Cli/SourceInventory.cs` | 11 | 7 | 4 | 7 | Low | High | 7/7 |
| `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` | 34 | 26 | 8 | 26 | Low | High | 23/14 |
| `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` | 22 | 21 | 1 | 21 | Low | High | 17/8 |
| `src/Broiler.Code.Review/Assurance/AssuranceCandidates.cs` | 24 | 14 | 10 | 14 | Low | High | 7/4 |
| `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` | 15 | 15 | 0 | 15 | Low | High | 14/12 |
| `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` | 65 | 34 | 31 | 34 | Low | High | 22/20 |
| `src/Broiler.Code.Review/Assurance/AssuranceComponentReport.cs` | 5 | 5 | 0 | 5 | Low | Medium | 3/0 |
| `src/Broiler.Code.Review/Assurance/AssuranceCorpus.cs` | 16 | 8 | 8 | 8 | None | High | 2/2 |
| `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` | 37 | 24 | 13 | 24 | Low | High | 22/13 |
| `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` | 24 | 20 | 4 | 20 | Low | High | 17/14 |
| `src/Broiler.Code.Review/Assurance/AssuranceGlob.cs` | 9 | 5 | 4 | 5 | Low | High | 4/4 |
| `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` | 20 | 20 | 0 | 20 | Low | High | 20/15 |
| `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` | 11 | 11 | 0 | 11 | Low | High | 9/8 |
| `src/Broiler.Code.Review/Assurance/AssuranceHumanReviewRecord.cs` | 14 | 14 | 0 | 14 | Low | Medium | 11/0 |
| `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` | 28 | 17 | 11 | 17 | Low | High | 15/8 |
| `src/Broiler.Code.Review/Assurance/AssuranceLines.cs` | 14 | 11 | 3 | 11 | Low | High | 10/3 |
| `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` | 21 | 21 | 0 | 21 | Low | High | 15/10 |
| `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` | 13 | 11 | 2 | 11 | Low | High | 6/6 |
| `src/Broiler.Code.Review/Assurance/AssuranceReportContext.cs` | 13 | 10 | 3 | 10 | Low | Low | 7/0 |
| `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` | 9 | 9 | 0 | 9 | Low | High | 9/9 |
| `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` | 15 | 15 | 0 | 15 | Low | High | 15/13 |
| `src/Broiler.Code.Review/Assurance/AssuranceScannedFile.cs` | 7 | 6 | 1 | 6 | None | High | 2/2 |
| `src/Broiler.Code.Review/Assurance/AssuranceUnit.cs` | 19 | 7 | 12 | 7 | None | High | 6/2 |
| `src/Broiler.Code.Review/Assurance/AssuranceUnitState.cs` | 14 | 6 | 8 | 6 | Low | High | 6/4 |
| `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` | 27 | 27 | 0 | 27 | Low | High | 26/17 |
| `src/Broiler.Code.Review/FileReview.cs` | 18 | 10 | 8 | 10 | Low | High | 10/2 |
| `src/Broiler.Code.Review/IRevisionProvider.cs` | 5 | 4 | 1 | 4 | None | Medium | 4/0 |
| `src/Broiler.Code.Review/NoteAnchoring.cs` | 6 | 6 | 0 | 6 | Low | High | 6/4 |
| `src/Broiler.Code.Review/ReviewContentHash.cs` | 7 | 7 | 0 | 7 | Low | High | 7/7 |
| `src/Broiler.Code.Review/ReviewCoverage.cs` | 12 | 12 | 0 | 12 | Low | High | 11/5 |
| `src/Broiler.Code.Review/ReviewJson.cs` | 15 | 15 | 0 | 15 | Low | High | 15/9 |
| `src/Broiler.Code.Review/ReviewNote.cs` | 27 | 11 | 16 | 11 | Low | Medium | 9/0 |
| `src/Broiler.Code.Review/ReviewStateEvaluator.cs` | 3 | 3 | 0 | 3 | Low | High | 3/3 |
| `src/Broiler.Code.Review/ReviewStatus.cs` | 17 | 8 | 9 | 8 | Low | High | 8/6 |
| `src/Broiler.Code.Review/ReviewStore.cs` | 14 | 13 | 1 | 13 | Low | High | 12/10 |
| `src/Broiler.Code.Windows/CodeHost.cs` | 13 | 12 | 1 | 12 | Low | Critical | 7/5 |
| `src/Broiler.Code.Windows/CodeShellFactory.cs` | 4 | 4 | 0 | 4 | Low | High | 4/3 |
| `src/Broiler.Code.Windows/CodeWindow.cs` | 29 | 18 | 11 | 18 | Low | High | 17/4 |
| `src/Broiler.Code.Windows/Program.cs` | 2 | 2 | 0 | 2 | Low | High | 2/2 |
| `src/Broiler.Code.Windows/WindowsFileDialogs.cs` | 49 | 17 | 32 | 17 | Low | Critical | 17/17 |
| `src/Broiler.Code.Windows/WindowsTextInputService.cs` | 29 | 19 | 10 | 19 | Low | Critical | 18/16 |
| `src/Broiler.Code.Workspaces/CodeWorkspace.cs` | 55 | 37 | 18 | 37 | Low | High | 26/13 |
| `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` | 12 | 6 | 6 | 6 | Low | High | 5/5 |
| `src/Broiler.Code.Workspaces/Model/WorkspaceItem.cs` | 51 | 10 | 41 | 10 | Low | Medium | 4/0 |
| `src/Broiler.Code.Workspaces/Projects/DeclaredProjectFile.cs` | 28 | 19 | 9 | 19 | Low | High | 17/2 |
| `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` | 22 | 13 | 9 | 13 | Low | High | 11/5 |
| `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` | 13 | 12 | 1 | 12 | Low | High | 11/8 |
| `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` | 19 | 16 | 3 | 16 | Low | High | 16/10 |
| `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` | 33 | 15 | 18 | 15 | None | High | 12/7 |
| `src/Broiler.Code.Workspaces/Storage/WorkspacePath.cs` | 5 | 5 | 0 | 5 | Low | High | 5/4 |
| `src/Broiler.Code.Workspaces/Text/RopeNode.cs` | 28 | 19 | 9 | 19 | Low | Medium | 19/0 |
| `src/Broiler.Code.Workspaces/Text/SourceBuffer.cs` | 24 | 16 | 8 | 16 | Low | Medium | 10/0 |
| `src/Broiler.Code.Workspaces/Text/TextChange.cs` | 18 | 14 | 4 | 14 | Low | Low | 4/0 |
| `src/Broiler.Code.Workspaces/Text/TextSearch.cs` | 10 | 10 | 0 | 10 | Low | Medium | 4/0 |
| `src/Broiler.Code.Workspaces/Text/TextSnapshot.cs` | 25 | 20 | 5 | 20 | Low | Medium | 16/0 |
| `src/Broiler.Code.Workspaces/WorkspaceLoader.cs` | 6 | 6 | 0 | 6 | Low | High | 5/4 |

## 7. Decisions Recorded

No unit in this component carries a decision on its human line. Every one of them reads
`PENDING`.

## 8. Decisions The Code Has Outrun

No unit carries a decision that the code has since moved past.

## 9. Where A Decision Is Required First

The units at the top of the security vocabulary, with the observation that would show each
one wrong and the human line it carries. The set is read from the assessments rather than
written out, so a unit that becomes `High` joins it at the next generation.

- `Broiler.Code.Core.CodeAnalysisController` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, Spec=none cited, `23AD15`, PENDING
  - Falsified if: a classification computed for an older snapshot is applied to the editor after a newer edit has started a run
- `Broiler.Code.Core.CodeAnalysisController.CodeAnalysisController(UiCodeEditor, SourceBufferDocument, ICodeClassifier, IUiDispatcher, IAnalysisScheduler?, int)` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, Spec=none cited, `6C2E25`, PENDING
  - Falsified if: an edit raised after construction does not start a classification run because the handler is not subscribed to SnapshotChangedWithDelta
- `Broiler.Code.Core.CodeAnalysisController.Dispose()` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, Spec=none cited, `484938`, PENDING
  - Falsified if: an edit made after Dispose still starts a classification run
- `Broiler.Code.Core.CodeAnalysisController.Refresh()` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, Spec=none cited, `1E7D4D`, PENDING
  - Falsified if: Refresh hands the classifier a non-null change, so it reuses the previous result instead of classifying from scratch
- `Broiler.Code.Core.CodeAnalysisController.Start(ICodeTextSnapshot, CodeTextChange?)` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, Spec=none cited, `AD314B`, PENDING
  - Falsified if: a newer Start leaves the previous run's token uncancelled, so a superseded background classification runs to completion
- `Broiler.Code.Core.CodeAnalysisController.RunInBackground(long, CodeClassificationResult?, ICodeTextSnapshot, CodeTextChange?, CancellationToken)` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, Spec=none cited, `2F0F28`, PENDING
  - Falsified if: a completed background run calls Publish on the worker thread instead of through the dispatcher
- `Broiler.Code.Core.CodeAnalysisController.Publish(long, CodeClassificationResult)` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, Spec=none cited, `832F7D`, PENDING
  - Falsified if: a result whose generation is older than the latest Start is applied to the editor
- `Broiler.Code.Core.Hosting.DesktopInputRouter` in `src/Broiler.Code.Core/Hosting/DesktopInputRouter.cs` - Security=High, Spec=none cited, `062099`, PENDING
  - Falsified if: two events built concurrently by one router carry the same sequence number
- `Broiler.Code.Core.Hosting.DesktopInputRouter.Header(InputDeviceId)` in `src/Broiler.Code.Core/Hosting/DesktopInputRouter.cs` - Security=High, Spec=none cited, `695AF6`, PENDING
  - Falsified if: two events built concurrently by one router receive the same sequence number, or a later event a smaller one
- `Broiler.Code.Core.Hosting.EvdevInputRouter` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, Spec=none cited, `54F756`, PENDING
  - Falsified if: pointer updates from two threads interleave so that PointerPosition reports a coordinate outside the current viewport
- `Broiler.Code.Core.Hosting.EvdevInputRouter.PointerPosition` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, Spec=none cited, `58D12E`, PENDING
  - Falsified if: a read during a concurrent move returns the X of one update together with the Y of another
- `Broiler.Code.Core.Hosting.EvdevInputRouter.SetViewport(BSize)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, Spec=none cited, `1420B9`, PENDING
  - Falsified if: after the pointer has been placed, a resize to a smaller viewport leaves it beyond the new width or height minus one
- `Broiler.Code.Core.Hosting.EvdevInputRouter.SetAbsolutePointer(double, double, InputDeviceId, long)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, Spec=none cited, `AD971D`, PENDING
  - Falsified if: an absolute position outside the viewport is emitted without being clamped to it
- `Broiler.Code.Core.Hosting.EvdevInputRouter.FromMouseMove(MouseMoveEvent)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, Spec=none cited, `BD2522`, PENDING
  - Falsified if: relative motion past the viewport edge yields a position below zero or above the extent minus one
- `Broiler.Code.Core.Hosting.EvdevInputRouter.FromMouseButton(MouseButtonEvent)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, Spec=none cited, `11A5E4`, PENDING
  - Falsified if: a button event that arrives before any motion is emitted at a position other than the viewport centre
- `Broiler.Code.Core.Hosting.EvdevInputRouter.FromWheel(MouseWheelEvent)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, Spec=none cited, `43AFDC`, PENDING
  - Falsified if: a wheel event carrying Shift in its Modifiers is emitted with no modifiers
- `Broiler.Code.Core.Hosting.UiThreadDispatcher` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, Spec=none cited, `08954B`, PENDING
  - Falsified if: a callback passed to Post from a worker thread runs on that worker thread rather than on the thread that constructed the dispatcher
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.UiThreadDispatcher(Action?)` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, Spec=none cited, `089C6F`, PENDING
  - Falsified if: UiThreadId differs from the managed thread id of the thread that ran the constructor
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.PendingCount` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, Spec=none cited, `040E0B`, PENDING
  - Falsified if: a read concurrent with Post throws or returns a count the queue never held
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.CheckAccess()` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, Spec=none cited, `BE395E`, PENDING
  - Falsified if: CheckAccess returns true on a thread other than the one that constructed the dispatcher
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.Post(Action)` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, Spec=none cited, `EE0A23`, PENDING
  - Falsified if: a callback posted concurrently with another Post or with a Drain is lost and never runs
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.Drain()` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, Spec=none cited, `7BE455`, PENDING
  - Falsified if: a callback posted while Drain runs is executed within that same Drain call
- `Broiler.Code.Core.Review.AssuranceController` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `844676`, PENDING
  - Falsified if: a signature is written into the open buffer from a document model read at an earlier buffer version, so edits made since that version are reverted
- `Broiler.Code.Core.Review.AssuranceController.SetCurrentDocument(WorkspaceItemId)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `85AAF2`, PENDING
  - Falsified if: after a switch to another document, the unit picked before the editor reports a caret comes from the previous document's caret line rather than line 0
- `Broiler.Code.Core.Review.AssuranceController.SetCaretLine(int)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `A74D32`, PENDING
  - Falsified if: moving the caret to a line inside a different declaration, with the buffer version unchanged, leaves CurrentUnit on the previous declaration
- `Broiler.Code.Core.Review.AssuranceController.Refresh()` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `C349F7`, PENDING
  - Falsified if: CurrentUnit after a refresh comes from a document model read at a buffer version older than the buffer's current one
- `Broiler.Code.Core.Review.AssuranceController.OnBufferChanged(TextSnapshot, TextChange)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `B616D0`, PENDING
  - Falsified if: an edit that rewrites a human line in the watched buffer leaves CurrentUnit reporting the line as it was before the edit
- `Broiler.Code.Core.Review.AssuranceController.Approve()` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `1CA5CD`, PENDING
  - Falsified if: signing while the configured name is empty or whitespace changes the open buffer
- `Broiler.Code.Core.Review.AssuranceController.Withdraw()` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `4CEFEA`, PENDING
  - Falsified if: withdrawing changes a line of the buffer other than the human line of the unit that contains the caret
- `Broiler.Code.Core.Review.AssuranceController.Act(Func<AssuranceDocument, AssuranceUnit, string, AssuranceEditResult>)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `826F83`, PENDING
  - Falsified if: after the buffer refuses one decision, the next accepted decision writes the refused line change into the buffer as well as its own
- `Broiler.Code.Core.Review.AssuranceController.Apply(string)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `89F717`, PENDING
  - Falsified if: a buffer that moved to a new version between reading the snapshot and submitting the edit accepts the replacement instead of rejecting it as stale
- `Broiler.Code.Core.Review.AssuranceController.Narrow(string, string)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `722C27`, PENDING
  - Falsified if: applying the returned span to the current text yields a string other than the updated text, for example when the change sits inside a run of repeated characters
- `Broiler.Code.Core.Review.AssuranceController.Rebuild()` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `1DA4B2`, PENDING
  - Falsified if: the document model is kept after the open buffer's version or the item's path changed, so the next decision is computed from text the buffer no longer holds
- `Broiler.Code.Core.Review.AssuranceController.ScannerFor(WorkspaceItemId)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, Spec=none cited, `4C462F`, PENDING
  - Falsified if: a document granted from another folder is scanned with the scanner made for the workspace's own storage, so its relative path is read against the wrong root
- `Broiler.Code.Core.Review.GitCommand` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, Spec=none cited, `2D0DFB`, PENDING
  - Falsified if: a file named git in the editor process's current directory is started in place of the git found on PATH
- `Broiler.Code.Core.Review.GitCommand.Timeout` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, Spec=none cited, `3B2C97`, PENDING
  - Falsified if: a git child that never exits keeps a lookup waiting for more than two seconds
- `Broiler.Code.Core.Review.GitCommand.RunAsync(string, string, CancellationToken)` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, Spec=none cited, `3ED69D`, PENDING
  - Falsified if: a file named git in the editor process's current directory is started in place of the git found on PATH
- `Broiler.Code.Core.Review.GitIdentity` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, Spec=none cited, `83CBB4`, PENDING
  - Falsified if: a git lookup that fails, times out or prints an empty user.name yields an empty name instead of the account name
- `Broiler.Code.Core.Review.GitIdentity.ResolveReviewerAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, Spec=none cited, `7D5BB6`, PENDING
  - Falsified if: a git lookup that fails, times out or prints an empty user.name yields an empty name instead of the account name
- `Broiler.Code.Core.Review.GitRevisionProvider` in `src/Broiler.Code.Core/Review/GitRevisionProvider.cs` - Security=High, Spec=none cited, `25855C`, PENDING
  - Falsified if: git output other than a single hexadecimal object name of 7 to 64 digits is returned as the current revision
- `Broiler.Code.Core.Review.GitRevisionProvider.GetCurrentRevisionAsync(CancellationToken)` in `src/Broiler.Code.Core/Review/GitRevisionProvider.cs` - Security=High, Spec=none cited, `71534B`, PENDING
  - Falsified if: rev-parse output with a warning line before the object name is returned as the current revision
- `Broiler.Code.Core.Review.ReviewController` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `655D40`, PENDING
  - Falsified if: a decision other than clearing, taken on a document with unsaved changes, is written to the store with a hash of the unsaved buffer text
- `Broiler.Code.Core.Review.ReviewController.LoadAsync(CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `324755`, PENDING
  - Falsified if: a decision committed while LoadAsync is still reading records is missing from StateFor after the load publishes
- `Broiler.Code.Core.Review.ReviewController.Publish(Dictionary<string, FileReview>, Dictionary<string, ReviewState>, IReadOnlyList<StorageFailure>, long)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `0748C6`, PENDING
  - Falsified if: a review cleared after the load began has its old state back in StateFor once the load's snapshot is swapped in
- `Broiler.Code.Core.Review.ReviewController.ContentOfAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `B107F6`, PENDING
  - Falsified if: a document opened or closed on the UI thread while a background load evaluates records makes this lookup throw or miss the open buffer
- `Broiler.Code.Core.Review.ReviewController.SetCurrentDocument(WorkspaceItemId)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `AE7E48`, PENDING
  - Falsified if: after a document is selected, the next decision is written to the record of the previously selected file
- `Broiler.Code.Core.Review.ReviewController.ReviewFor(string)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `26B92D`, PENDING
  - Falsified if: a path with no record of its own returns the record of another path instead of an empty record for itself
- `Broiler.Code.Core.Review.ReviewController.RecordDecisionAsync(ReviewStatus, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `5B26D5`, PENDING
  - Falsified if: a decision other than clearing, taken on a document with unsaved changes, is written to the store
- `Broiler.Code.Core.Review.ReviewController.AddNoteAsync(ReviewNoteKind, string, int, int, string?, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `5AFDA7`, PENDING
  - Falsified if: a note is written with an empty or whitespace author when no name is configured
- `Broiler.Code.Core.Review.ReviewController.ResolveNoteAsync(string, string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `975B40`, PENDING
  - Falsified if: a note id that is not in the record is reported as resolved and the record is rewritten
- `Broiler.Code.Core.Review.ReviewController.RemoveNoteAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `80DA0A`, PENDING
  - Falsified if: a note id that is not in the record is reported as removed and the record is rewritten
- `Broiler.Code.Core.Review.ReviewController.CommitAsync(FileReview, string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `16B141`, PENDING
  - Falsified if: the review maps are written and Changed is raised on a thread-pool thread when a decision is recorded with a revision lookup that completes asynchronously
- `Broiler.Code.Core.Review.ReviewController.Target()` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `77FAB9`, PENDING
  - Falsified if: a target is returned for a selected item that has no open document
- `Broiler.Code.Core.Shell.CodeShell` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `AC9F26`, PENDING
  - Falsified if: a tab switch or finished background load that moves the file status picker is read back as a new decision and recorded against the file now shown
- `Broiler.Code.Core.Shell.CodeShell.ReviewStatusItems` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `42EA1B`, PENDING
  - Falsified if: an entry runs a file decision command other than the one its label names, such as the in-review entry recording a different status
- `Broiler.Code.Core.Shell.CodeShell.ReviewUnitItems` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `0F9BB4`, PENDING
  - Falsified if: the entry labelled as needing review runs ApproveUnit, or the other entry runs WithdrawUnit
- `Broiler.Code.Core.Shell.CodeShell.AttachWorkspace(CodeWorkspace, Workspaces.Recovery.RecoveryJournal?)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `F362A1`, PENDING
  - Falsified if: after a tab click the review controller still targets the tab just left, so the next recorded decision is written against that file
- `Broiler.Code.Core.Shell.CodeShell.AttachReview(CodeWorkspace)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `A6634C`, PENDING
  - Falsified if: a file in the newly attached workspace is badged with the review state recorded for the same relative path in the workspace attached before it
- `Broiler.Code.Core.Shell.CodeShell.LoadReviewAsync(ReviewController, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `3998B9`, PENDING
  - Falsified if: with no dispatcher set, the records are read on a pool thread and published into the review maps while the UI thread is reading them
- `Broiler.Code.Core.Shell.CodeShell.Reviewer` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `BE4898`, PENDING
  - Falsified if: after the name changes, the next file decision or unit signature is still written under the previous name
- `Broiler.Code.Core.Shell.CodeShell.InvokeAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `61F5D1`, PENDING
  - Falsified if: a command whose IsEnabled is false still runs its handler, for example a file decision written while no name is set
- `Broiler.Code.Core.Shell.CodeShell.NewProjectAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `764E37`, PENDING
  - Falsified if: a decision recorded in the newly created project carries the commit of the repository that was open before it
- `Broiler.Code.Core.Shell.CodeShell.OpenAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `BCBD0E`, PENDING
  - Falsified if: a source file picked outside the current root is opened through the workspace's own storage instead of the grant the dialog returned
- `Broiler.Code.Core.Shell.CodeShell.OpenFolderAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `671877`, PENDING
  - Falsified if: a folder picked while the unsaved-changes prompt is declined still replaces the open workspace
- `Broiler.Code.Core.Shell.CodeShell.ApplyRevisionProviderFor(FileGrant)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `5E4E95`, PENDING
  - Falsified if: after a second folder is granted with no factory set, a decision recorded there carries the commit of the root opened before it
- `Broiler.Code.Core.Shell.CodeShell.SaveActiveAsAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `28BA35`, PENDING
  - Falsified if: the document is written to a path other than the one inside the grant the dialog returned, or its tab is renamed when the write failed
- `Broiler.Code.Core.Shell.CodeShell.OpenSolutionAsync(FileGrant, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `216AB1`, PENDING
  - Falsified if: a solution that fails to load, or whose unsaved-changes prompt is declined, still detaches the open workspace
- `Broiler.Code.Core.Shell.CodeShell.OpenDocumentAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `0A1237`, PENDING
  - Falsified if: an item id that names no document of the attached workspace opens a tab instead of reporting that the item could not be opened
- `Broiler.Code.Core.Shell.CodeShell.RefreshCommands()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `384BC6`, PENDING
  - Falsified if: a menu entry is drawn enabled while the command it runs reports itself disabled
- `Broiler.Code.Core.Shell.CodeShell.SyncReviewState()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `84DE76`, PENDING
  - Falsified if: after a tab switch the review or assurance controller still targets the document shown before it, so the next decision is recorded against that file
- `Broiler.Code.Core.Shell.CodeShell.SyncReviewUnitInput()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `DCC33D`, PENDING
  - Falsified if: pointing the declaration picker at a different unit runs ApproveUnit or WithdrawUnit on it without anyone picking an entry
- `Broiler.Code.Core.Shell.CodeShell.OnReviewUnitSelected(object?, UiComboBoxSelectionChangedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `71DCEC`, PENDING
  - Falsified if: a selection raised while a picker is being synced runs ApproveUnit or WithdrawUnit
- `Broiler.Code.Core.Shell.CodeShell.SyncReviewStatusInput()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `0BACA6`, PENDING
  - Falsified if: matching the file picker to the record, for example on a tab switch, runs a file decision command against the file now shown
- `Broiler.Code.Core.Shell.CodeShell.OnReviewStatusSelected(object?, UiComboBoxSelectionChangedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `C02D4C`, PENDING
  - Falsified if: a selection raised while the picker is being synced to the record runs a file decision command
- `Broiler.Code.Core.Shell.CodeShell.RecordPickedStatusAsync(string)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `F1B294`, PENDING
  - Falsified if: after a refused decision the file picker keeps showing the refused status instead of the recorded one
- `Broiler.Code.Core.Shell.CodeShell.OnToolbarButtonClicked(object?, UiButtonClickEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `4D168C`, PENDING
  - Falsified if: clicking a toolbar button runs a command other than the one its CommandName names
- `Broiler.Code.Core.Shell.CodeShell.OnMenuItemInvoked(object?, UiMenuItemInvokedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `75A984`, PENDING
  - Falsified if: invoking a menu item runs a command other than the one its CommandName names, or a separator runs a command
- `Broiler.Code.Core.Shell.CodeShell.OnExplorerNodeActivated(object?, TreeNodeEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `4021EE`, PENDING
  - Falsified if: activating a folder or project row opens it as a document
- `Broiler.Code.Core.Shell.CodeShell.OnProblemActivated(object?, TreeNodeEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `05BB87`, PENDING
  - Falsified if: activating a project-level problem, or one whose document is not in the workspace, opens a document
- `Broiler.Code.Core.Shell.CodeShell.OnReviewChanged(object?, EventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `15E73A`, PENDING
  - Falsified if: after a file decision is cleared, the explorer keeps its earlier badge on that file until something else refreshes the tree
- `Broiler.Code.Core.Shell.CodeShell.OnEditorSelectionChanged(object?, CodeSelectionChangedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `84E20B`, PENDING
  - Falsified if: the line handed to the assurance controller is not the line holding the caret's focus, so a signature lands on a neighbouring declaration
- `Broiler.Code.Core.Shell.CodeShell.OnReviewNoteSubmitted(object?, UiEditSubmittedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `978A10`, PENDING
  - Falsified if: submitting the note field runs a command other than AddNote
- `Broiler.Code.Core.Shell.CodeShell.AddNoteFromInputAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `8A5CB3`, PENDING
  - Falsified if: the note is anchored to a line other than the one holding the caret when it is submitted
- `Broiler.Code.Core.Shell.CodeShell.SignUnit(bool)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `4B699E`, PENDING
  - Falsified if: a unit signature is written under a name other than the shell's current one, such as a name set on the assurance controller before the change
- `Broiler.Code.Core.Shell.CodeShell.RecordReviewAsync(ReviewStatus, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `DE1B4F`, PENDING
  - Falsified if: a decision the controller refused, such as one on a document with unsaved changes, is reported as recorded
- `Broiler.Code.Core.Shell.CodeShell.NavigateAsync(WorkspaceItemId, ProblemEntry)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `74A038`, PENDING
  - Falsified if: a problem whose line is past the end of the reopened document places the caret outside it or throws
- `Broiler.Code.Core.Shell.CodeShell.SaveActiveAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `9D092A`, PENDING
  - Falsified if: a save refused because the file changed on disk since it was opened is reported as saved
- `Broiler.Code.Core.Shell.CodeShell.SaveAllAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `2C007B`, PENDING
  - Falsified if: a document that failed to save is missing from the status line's list of unsaved files
- `Broiler.Code.Core.Shell.CodeShell.DetachWorkspace()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, Spec=none cited, `9CE6B8`, PENDING
  - Falsified if: a review load still running when the workspace is detached goes on to publish its records into a controller that the pane or explorer still reads
- `Broiler.Code.Core.Shell.DocumentCoordinator` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, Spec=none cited, `E40A07`, PENDING
  - Falsified if: a dirty document is closed and its unsaved text dropped although the prompt did not answer Discard
- `Broiler.Code.Core.Shell.DocumentCoordinator.OpenAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, Spec=none cited, `9F57C0`, PENDING
  - Falsified if: opening an item that already has a tab creates a second tab or a second buffer adapter for the same document
- `Broiler.Code.Core.Shell.DocumentCoordinator.CloseAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, Spec=none cited, `0BED20`, PENDING
  - Falsified if: a dirty document is closed and its unsaved text dropped when the prompt answered anything other than Discard, including an unset prompt or a Save that failed
- `Broiler.Code.Core.Shell.DocumentCoordinator.CloseAllAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, Spec=none cited, `B97F3F`, PENDING
  - Falsified if: documents after one whose close was cancelled or whose save failed are still closed
- `Broiler.Code.Core.Shell.DocumentCoordinator.SaveAllAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, Spec=none cited, `15A4AD`, PENDING
  - Falsified if: a document whose save failed has its recovery journal entry deleted or its tab shown as clean
- `Broiler.Code.Core.Shell.DocumentCoordinator.OnCloseRequested(object?, UiTabCloseRequestedEventArgs)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, Spec=none cited, `DC7F01`, PENDING
  - Falsified if: a close requested from the tab strip on a dirty document removes the tab without going through the dirty-close prompt
- `Broiler.Code.Core.Shell.DocumentCoordinator.OnBufferChanged(WorkspaceItemId)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, Spec=none cited, `6EDB4E`, PENDING
  - Falsified if: an edit made while the previous journal write for the same document is still in flight leaves the journal without that edit's text
- `Broiler.Code.Core.Shell.IFileDialogService` in `src/Broiler.Code.Core/Shell/FileDialogs.cs` - Security=High, Spec=none cited, `BE9D67`, PENDING
  - Falsified if: an implementation returns a grant whose Storage reaches above the chosen file's directory or above the folder the user picked
- `Broiler.Code.Core.Shell.IFileDialogService.RequestOpenAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Core/Shell/FileDialogs.cs` - Security=High, Spec=none cited, `944E8F`, PENDING
  - Falsified if: an implementation returns a grant whose Storage reaches above the directory holding the file the user picked
- `Broiler.Code.Core.Shell.IFileDialogService.RequestSaveAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Core/Shell/FileDialogs.cs` - Security=High, Spec=none cited, `DCA869`, PENDING
  - Falsified if: an implementation returns a grant whose Storage reaches above the directory of the name the user chose
- `Broiler.Code.Core.Shell.IFileDialogService.RequestFolderAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Core/Shell/FileDialogs.cs` - Security=High, Spec=none cited, `EF6151`, PENDING
  - Falsified if: an implementation returns a grant whose Storage reaches above the directory the user picked
- `Broiler.Code.Core.Shell.WorkspaceBootstrap` in `src/Broiler.Code.Core/Shell/WorkspaceBootstrap.cs` - Security=High, Spec=none cited, `460147`, PENDING
  - Falsified if: a directory entry listed under the grant that leads back to one of its ancestors makes the startup walk recurse without end
- `Broiler.Code.Core.Shell.WorkspaceBootstrap.OpenAsync(CodeShell, IWorkspaceStorage, CancellationToken)` in `src/Broiler.Code.Core/Shell/WorkspaceBootstrap.cs` - Security=High, Spec=none cited, `DDEF13`, PENDING
  - Falsified if: a solution in the grant that fails to load leaves the shell without an attached workspace instead of opening the folder as loose sources
- `Broiler.Code.Core.Shell.WorkspaceBootstrap.FindSolutionAsync(IWorkspaceStorage, CancellationToken)` in `src/Broiler.Code.Core/Shell/WorkspaceBootstrap.cs` - Security=High, Spec=none cited, `ACA135`, PENDING
  - Falsified if: a directory whose name ends in .slnx or .sln is returned as the solution to load
- `Broiler.Code.Core.Shell.WorkspaceBootstrap.AddSourcesAsync(CodeWorkspace, IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Core/Shell/WorkspaceBootstrap.cs` - Security=High, Spec=none cited, `22DE66`, PENDING
  - Falsified if: a directory entry that leads back to one of its ancestors makes the walk recurse without end
- `Broiler.Code.Core.Templates.CodeTemplateService` in `src/Broiler.Code.Core/Templates/CodeTemplateService.cs` - Security=High, Spec=none cited, `3AC707`, PENDING
  - Falsified if: WriteAsync replaces a file that already existed at a planned path when the call began instead of refusing the plan
- `Broiler.Code.Core.Templates.CodeTemplateService.WriteAsync(TemplateResult, CancellationToken)` in `src/Broiler.Code.Core/Templates/CodeTemplateService.cs` - Security=High, Spec=none cited, `B2F7A2`, PENDING
  - Falsified if: a plan whose later file is refused by storage returns a failure after its earlier files were already written
- `Broiler.Code.Core.Templates.CodeTemplateService.AddProjectReferenceAsync(string, string, CancellationToken)` in `src/Broiler.Code.Core/Templates/CodeTemplateService.cs` - Security=High, Spec=none cited, `BA3BFC`, PENDING
  - Falsified if: a project file that is not well-formed XML makes the call throw instead of returning a failed TemplateResult
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, Spec=none cited, `FCB453`, PENDING
  - Falsified if: an AI marker line in one declaration's leading trivia is reported as the AnnotationLine of a different declaration
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.Markers` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, Spec=none cited, `D17683`, PENDING
  - Falsified if: a stray criterion-marker or human-line-marker comment that opens its own line is missing from AssuranceCommentLines, so the orphan rule never reports it
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.CSharpAssuranceFileScanner(IEnumerable<string>?, AssuranceExemptionPredicate, AssuranceNamedValues)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, Spec=none cited, `3FD07D`, PENDING
  - Falsified if: a scanner built with its own preprocessor symbols or the strict predicate scans under the default symbols or the owning-component predicate instead
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.ScanFile(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, Spec=none cited, `50D96B`, PENDING
  - Falsified if: an AI marker written inside a raw string literal or #if-disabled code is reported as a unit's AnnotationLine or among AssuranceCommentLines
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.IsCommentLike(SyntaxTrivia)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, Spec=none cited, `C9EB4E`, PENDING
  - Falsified if: a forged summary line written in a block comment, a documentation comment or #if-disabled text is absent from CommentLines and so escapes the forged-summary rule
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.LinesOf(SourceText, SyntaxTrivia)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, Spec=none cited, `941D43`, PENDING
  - Falsified if: a block comment spanning three physical lines yields other than three entries, or an entry numbered with a line its text is not on
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.OpensWithMarker(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, Spec=none cited, `B604BB`, PENDING
  - Falsified if: a comment that carries a marker later in its text rather than at its start is taken for an assurance line, or one opening with the human-line marker is not
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.LineOf(SyntaxTree, TextSpan)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, Spec=none cited, `B90CE4`, PENDING
  - Falsified if: a #line directive above a declaration moves the line reported for its block or for a directive away from the physical line
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.IsBlank(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, Spec=none cited, `A65D8A`, PENDING
  - Falsified if: a marker comment preceded on its line only by U+FEFF or U+001A, which the C# parser skips as whitespace, is left out of AssuranceCommentLines and escapes the orphan and below-declaration rules
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `BB1981`, PENDING
  - Falsified if: under the owning component's predicate, a file with no directive, top-level statement, multi-line literal or repeated name yields a unit whose name, exemption answer or fingerprint differs from what the owning component's scanner reports for it
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.DefaultSymbols` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `75F133`, PENDING
  - Falsified if: a symbol a net10.0 build defines, or one of DEBUG, RELEASE and TRACE, is missing from the list, so code under it scans as disabled text that belongs to no unit
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.DefaultParseOptions` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `B30BF9`, PENDING
  - Falsified if: the parameterless scanner or FingerprintOfFile parses under preprocessor symbols other than DefaultSymbols
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.CSharpAssuranceScanner()` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `B10EBD`, PENDING
  - Falsified if: the parameterless scanner applies the strict predicate instead of the owning component's
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.CSharpAssuranceScanner(IEnumerable<string>?, AssuranceExemptionPredicate, AssuranceNamedValues)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `99A0BA`, PENDING
  - Falsified if: an empty symbol list is treated like null and parses under the default symbols instead of under none
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.DefaultPreprocessorSymbols` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `81A57E`, PENDING
  - Falsified if: the list it returns differs from the symbols the default parse options were built from
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Scan(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `6F5C93`, PENDING
  - Falsified if: a file of about 20,000 nested parentheses ends the process with a stack overflow while it is parsed instead of returning units or throwing
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.OptionsFor(IEnumerable<string>)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `C3F494`, PENDING
  - Falsified if: the options it returns parse under a language version other than Latest, so a file using current syntax loses declarations to error recovery
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.CodeUnits(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `FAC37C`, PENDING
  - Falsified if: a member of a nested type, or a declaration inside a file-scoped namespace, is not yielded and so belongs to no unit
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Units(SyntaxTree, AssuranceExemptionPredicate, AssuranceNamedValues)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `0A09EC`, PENDING
  - Falsified if: two units of one file are returned under the same name
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Describe(SyntaxTree, MemberDeclarationSyntax, AssuranceExemptionPredicate, AssuranceNamedValues)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `EA9465`, PENDING
  - Falsified if: a declaration with attributes reports its start line at its first modifier or keyword instead of at its first attribute, so its block would land between the attribute and the declaration
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.TopLevel(SyntaxTree, IReadOnlyList<GlobalStatementSyntax>)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `93C0F6`, PENDING
  - Falsified if: editing a local function declared among a file's top-level statements leaves the top-level unit's fingerprint unchanged
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Fingerprint(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `F25577`, PENDING
  - Falsified if: changing any token of a declaration, or code under an inactive #if branch inside it, leaves its fingerprint unchanged
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.TokenStream(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `F05AC3`, PENDING
  - Falsified if: editing only a comment, whitespace or the annotation block above a declaration changes its token stream
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.FingerprintOfFile(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `2EDB57`, PENDING
  - Falsified if: one file checked out with CRLF line endings and with LF line endings gets two different file fingerprints
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Hash(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `992CC1`, PENDING
  - Falsified if: an edited declaration found by trying on average about 17 million token-level variants, such as a changed numeric literal, hashes to the same six characters as the original, so the decision recorded against that value still applies
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.StreamOf(IEnumerable<SyntaxToken>, bool)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `76B8D7`, PENDING
  - Falsified if: a directive or disabled text in the leading trivia of a declaration's second or later token is left out of its stream
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.HiddenCode(SyntaxTriviaList)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `3C5CFB`, PENDING
  - Falsified if: rewriting code inside an inactive #else branch within a method leaves the method's fingerprint unchanged
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Normalized(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `B4B560`, PENDING
  - Falsified if: a verbatim or raw string that spans lines gets a different fingerprint in a CRLF checkout than in an LF checkout of the same commit
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Tokens(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `7FD968`, PENDING
  - Falsified if: adding, removing or renaming a member of an enum leaves the enum's fingerprint unchanged
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.HeaderTokens(TypeDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `B6F075`, PENDING
  - Falsified if: changing a type's attributes, base list or type-parameter constraints leaves the type's fingerprint unchanged
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsCodeUnit(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `33CB75`, PENDING
  - Falsified if: a declaration kind that carries executable code (a method, constructor, operator, conversion, indexer or event with accessors) is answered false and so belongs to no unit
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.ExemptionFor(MemberDeclarationSyntax, bool, bool)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `15A7B2`, PENDING
  - Falsified if: an expression-bodied method that calls another member of its type with a literal argument, such as Run(true), is reported exempt
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsNamedValue(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `C5F526`, PENDING
  - Falsified if: a static readonly field of a type other than Guid, IntPtr, UIntPtr, nint or nuint, such as a Regex built from one string literal, is answered a named value and so needs no block
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsStatedByLiterals(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `065A01`, PENDING
  - Falsified if: an initializer that calls something, such as Guid.NewGuid() or Guid.Parse of a literal, or passes an array to a constructor, is answered stated by literals
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsLiteral(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `8CB2D6`, PENDING
  - Falsified if: a name or an arithmetic expression over literals, such as (IntPtr)(1 + 2), is answered a literal
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.SystemTypeName(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `F5BE9D`, PENDING
  - Falsified if: a type of the same name under another namespace, such as Vendor.Interop.Guid, is returned as Guid
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.DottedName(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `0CF67D`, PENDING
  - Falsified if: a generic name or an invocation inside the chain, such as Factory().Guid, yields a dotted name instead of null
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsFixedValue(FieldDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `EA2CF4`, PENDING
  - Falsified if: a const or static readonly field with an initializer is answered not fixed and reported exempt as storage
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsInert(ExpressionSyntax?)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `224F23`, PENDING
  - Falsified if: under the strict predicate, an instance field whose initializer is a chain of about 20,000 binary operators ends the process with a stack overflow instead of being answered
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.ThrowsInertly(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `CFDF20`, PENDING
  - Falsified if: a throw expression whose exception is built from a call, such as throw new X(Describe()), is answered inert
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsTrivialProperty(BasePropertyDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `4CF9C4`, PENDING
  - Falsified if: a property whose accessors return or assign a field whose name does not correspond to the property, such as Count returning _total, is answered trivial
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsCorrespondingMemberAccess(ExpressionSyntax, string?)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `3D126E`, PENDING
  - Falsified if: an expression that calls, indexes or applies an operator to the corresponding member, such as _count + 1 or _items[0], is answered a corresponding member access
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsFieldAssignmentFromValue(ExpressionSyntax, string?)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `F310E9`, PENDING
  - Falsified if: a setter that assigns anything other than the implicit value to the corresponding field, such as _count = value + 1, is answered a plain assignment
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.SimpleNameOf(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `0537AB`, PENDING
  - Falsified if: a constructor, operator, indexer or destructor is given a simple name, so a body returning a same-named member is answered trivial
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.AssignsParametersOnly(ConstructorDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `F5A703`, PENDING
  - Falsified if: a constructor that assigns a parameter to a member whose name does not correspond to it, such as _count = total, is answered parameter-assigning
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.AssignedMemberName(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `40A850`, PENDING
  - Falsified if: for x, this.x or A.B.x it yields something other than the last identifier x
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Corresponds(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `2203E1`, PENDING
  - Falsified if: a property FooBar returning _foobar is answered corresponding although only a leading underscore and the first letter's case are the convention, so a property re-pointed at a field that differs only in inner letter case stays exempt
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsSingleMemberAccess(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `7FC203`, PENDING
  - Falsified if: an expression-bodied property returning a member access chain of about 20,000 segments ends the process with a stack overflow instead of being answered
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsDelegationToOwnMember(ExpressionSyntax, SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `ABA696`, PENDING
  - Falsified if: an expression body forwarding its parameters to a method imported with using static, such as Kill(string path) calling Delete(path) from System.IO.File, is answered a delegation to its own type and reported exempt
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsForwardedParameter(ExpressionSyntax, SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `6BF7D2`, PENDING
  - Falsified if: a literal argument, or an expression over a parameter such as path + suffix, is answered a forwarded parameter
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsConstant(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `CAE7B9`, PENDING
  - Falsified if: an expression body returning a field or a computed value, such as _limit * 2, is answered constant
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsThrowNew(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `08E5C9`, PENDING
  - Falsified if: an expression body that throws an existing exception object, such as throw _error, is answered a throw of a new exception
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsOverrideOrOperator(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `271DCC`, PENDING
  - Falsified if: a method named other than ToString, GetHashCode or Equals, and not an operator or conversion, is answered true
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.OnlyDelegates(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `4206CB`, PENDING
  - Falsified if: a ToString or GetHashCode whose expression body calls an outside API with a literal argument, such as Process.Start of a literal command followed by ToString(), is answered only-delegating and reported exempt
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.ArrowBody(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `19406A`, PENDING
  - Falsified if: a block-bodied member is given an arrow body, so a body of several statements is judged as a single expression
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.SingleReturnedExpression(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `7B4228`, PENDING
  - Falsified if: a block body with any statement before its return is answered as that return's expression
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.SuppliesAnImplementation(MemberDeclarationSyntax, bool)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `3436F1`, PENDING
  - Falsified if: a record property with a block-bodied or expression-bodied accessor is answered compiler-supplied and reported exempt
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Unwrap(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `941528`, PENDING
  - Falsified if: an expression other than a parenthesized one, such as a cast or a checked expression, is unwrapped to its operand
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.ContainingTypes(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, Spec=none cited, `4ECA50`, PENDING
  - Falsified if: a declaration's containing types are yielded outermost first, so the record-member case judges a member by its outermost type
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, Spec=none cited, `557605`, PENDING
  - Falsified if: an assurance.config.json outside the granted root, reached through a '..' segment, a link below the root or a sibling directory whose name begins with the root's, decides how a file is scanned
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.MaxConfigurationBytes` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, Spec=none cited, `6152DB`, PENDING
  - Falsified if: a configuration file of several megabytes is read and parsed on the thread that rescans the open file
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.CSharpComponentAssuranceScanner(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, Spec=none cited, `6F6F0F`, PENDING
  - Falsified if: the root prefix is built without a trailing separator, so a sibling directory such as work2 beside the root work counts as inside it
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.Scan(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, Spec=none cited, `6236D4`, PENDING
  - Falsified if: a file under a configuration that watches named values is scanned with the default scanner, so its named values are listed as relevant units with no block
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.ConfigurationFor(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, Spec=none cited, `C93A51`, PENDING
  - Falsified if: a relative path whose '..' segments leave the root, or whose directory below the root is a link, yields a configuration file outside the root
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.ScannerFor(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, Spec=none cited, `ECF614`, PENDING
  - Falsified if: a configuration file that is a link, or one larger than MaxConfigurationBytes, is read and parsed
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.IsRoot(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, Spec=none cited, `D160B2`, PENDING
  - Falsified if: a directory above the root is answered as the root, so the walk reads the configuration there
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.IsUnderRoot(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, Spec=none cited, `FE85CC`, PENDING
  - Falsified if: a path in a sibling directory whose name begins with the root's, such as work2 beside the root work, is answered as under the root
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.IsLink(FileSystemInfo)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, Spec=none cited, `AD21F3`, PENDING
  - Falsified if: a directory junction or a symbolic link is answered false, so the walk follows it out of the root
- `Broiler.Code.Language.CSharp.Roslyn.CSharpLanguageService` in `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` - Security=High, Spec=none cited, `75FA82`, PENDING
  - Falsified if: an analysis whose project graph is unavailable, or whose document is outside the evaluated compile set, returns no project diagnostic saying why
- `Broiler.Code.Language.CSharp.Roslyn.CSharpLanguageService.Analyze(ICodeTextSnapshot, string, string, string?, IReadOnlyDictionary<string, string>?, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` - Security=High, Spec=none cited, `750037`, PENDING
  - Falsified if: a diagnostic located in another compile input is adorned on the analysed snapshot at that file's offsets
- `Broiler.Code.Language.CSharp.Roslyn.CSharpLanguageService.ResolveReferences(EvaluatedProjectGraph, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` - Security=High, Spec=none cited, `E33764`, PENDING
  - Falsified if: a reference path the graph lists but that is missing on disk is handed to MetadataReference.CreateFromFile and fails the analysis
- `Broiler.Code.Language.CSharp.Roslyn.CSharpLanguageService.ReadOverlayOrFile(string, IReadOnlyDictionary<string, string>?)` in `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` - Security=High, Spec=none cited, `519411`, PENDING
  - Falsified if: a compile path that exists but cannot be opened, such as a directory or an access-denied file, throws UnauthorizedAccessException out of Analyze instead of being read as empty text
- `Broiler.Code.Language.CSharp.Roslyn.WorkspaceTrust` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, Spec=none cited, `7D49B0`, PENDING
  - Falsified if: Untrusted is not the zero value, so a default-initialised trust setting lets evaluation run
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=Critical, Spec=none cited, `797B08`, PENDING
  - Falsified if: an evaluator whose Trust is Untrusted, or any value other than Trusted, starts a dotnet process
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.DesignTimeEvaluator(string)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, Spec=none cited, `7B19AE`, PENDING
  - Falsified if: an empty or whitespace workspace root is accepted and the process's current directory becomes the containment root
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.EvaluateAsync(string, string?, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=Critical, Spec=none cited, `7B6211`, PENDING
  - Falsified if: an evaluation whose dotnet build never exits keeps running past TimeoutSeconds, because only the caller's cancellation token ends the wait
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.Parse(string, string?, string)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, Spec=none cited, `2A2F09`, PENDING
  - Falsified if: well-formed JSON of another shape, such as a Compile item whose FullPath is a number or an Items.Compile that is not an array, throws InvalidOperationException, which the JsonException handler in EvaluateAsync does not catch
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.IsInsideWorkspace(string)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, Spec=none cited, `A7C384`, PENDING
  - Falsified if: on a case-sensitive file system a project in a sibling directory whose name differs from the workspace root's only in letter case passes the containment check
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.TryKill(Process)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, Spec=none cited, `C7D5EB`, PENDING
  - Falsified if: a process tree that cannot be fully terminated makes Kill throw AggregateException or Win32Exception, which escapes TryKill and replaces the cancellation EvaluateAsync rethrows
- `Broiler.Code.Language.CSharp.Roslyn.UntrustedGraphSource` in `src/Broiler.Code.Language.CSharp.Roslyn/EvaluatedProjectGraph.cs` - Security=High, Spec=none cited, `15E1EF`, PENDING
  - Falsified if: the source used before trust answers any project with a graph, so its compile and reference paths are read before the user trusted the workspace
- `Broiler.Code.Language.CSharp.Roslyn.UntrustedGraphSource.TryGetGraph(string, string?, out EvaluatedProjectGraph?, out GraphUnavailable?)` in `src/Broiler.Code.Language.CSharp.Roslyn/EvaluatedProjectGraph.cs` - Security=High, Spec=none cited, `E44D11`, PENDING
  - Falsified if: for some project path TryGetGraph returns true or a non-null graph, or reports a reason other than WorkspaceNotTrusted
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, Spec=none cited, `361447`, PENDING
  - Falsified if: a span is emitted whose start plus length exceeds the length of the line it was lexed from
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.Lex(ReadOnlySpan<char>, LineState, List<CodeClassificationSpan>)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, Spec=none cited, `B85F2E`, PENDING
  - Falsified if: some input line makes the main loop revisit an index without advancing, so Lex never returns
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.LexDirective(ReadOnlySpan<char>, int, List<CodeClassificationSpan>)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, Spec=none cited, `ECF1CC`, PENDING
  - Falsified if: a directive whose name runs to the end of the line produces a PreprocessorText span of zero length or one starting past the line end
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.LexLiteral(ReadOnlySpan<char>, int, List<CodeClassificationSpan>, out LineState, out bool)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, Spec=none cited, `C19B2E`, PENDING
  - Falsified if: a raw string opened with 256 or more quotes is carried as a wrapped quote count, so a shorter run of quotes closes it
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.LexCharacter(ReadOnlySpan<char>, int, List<CodeClassificationSpan>)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, Spec=none cited, `281435`, PENDING
  - Falsified if: a character literal that ends the line with a backslash produces a span that extends past the line end
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.ContinueVerbatimString(ReadOnlySpan<char>, int, byte, List<CodeClassificationSpan>, out bool)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, Spec=none cited, `0C3C1F`, PENDING
  - Falsified if: a doubled quote inside a verbatim string is taken as the closing quote, so the rest of the string is lexed as code
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.ContinueRawString(ReadOnlySpan<char>, int, byte, byte, List<CodeClassificationSpan>, out bool)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, Spec=none cited, `C0A2DC`, PENDING
  - Falsified if: a run of fewer quotes than the opening delimiter closes the raw string
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.ScanNumber(ReadOnlySpan<char>, int)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, Spec=none cited, `F3224D`, PENDING
  - Falsified if: a number whose last character on the line is an exponent marker returns an index past the line end
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.IndexOfBlockCommentEnd(ReadOnlySpan<char>)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, Spec=none cited, `762A41`, PENDING
  - Falsified if: for a span containing the comment terminator it returns an offset other than the index just past its slash
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, Spec=none cited, `0CE6B3`, PENDING
  - Falsified if: GetLineSpans returns a span that extends past the length of that line in the snapshot it was classified from
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier.Classify(ICodeTextSnapshot, CodeClassificationResult?, CodeTextChange?, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, Spec=none cited, `5AD983`, PENDING
  - Falsified if: a previous result whose snapshot version is not exactly one less than the new snapshot's is reused incrementally
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier.Full(ICodeTextSnapshot, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, Spec=none cited, `E6389C`, PENDING
  - Falsified if: a line is lexed with a start state other than the end state returned for the line before it
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier.Incremental(ICodeTextSnapshot, Result, CodeTextChange, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, Spec=none cited, `DC561E`, PENDING
  - Falsified if: after an edit, some line's spans differ from what a full classification of the new snapshot gives that line
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier.ClassifyLine(ICodeTextSnapshot, int, LineState, List<CodeClassificationSpan>, ref LineReader, out LineState)` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, Spec=none cited, `C720EA`, PENDING
  - Falsified if: spans of the previous line remain in the scratch list and are returned among this line's spans
- `Broiler.Code.Linux.CodeHost` in `src/Broiler.Code.Linux/CodeHost.cs` - Security=High, Spec=none cited, `6DB86A`, PENDING
  - Falsified if: the window's loop drains a dispatcher other than the one the session and the shell post to, so worker results are applied on the thread that produced them
- `Broiler.Code.Linux.CodeHost.RunAsync(string?, bool, CancellationToken)` in `src/Broiler.Code.Linux/CodeHost.cs` - Security=High, Spec=none cited, `3AC252`, PENDING
  - Falsified if: window.RunAsync is entered on a thread other than the one that constructed the window, so the dispatcher's Drain throws on the first frame
- `Broiler.Code.Linux.CodeShellFactory` in `src/Broiler.Code.Linux/CodeShellFactory.cs` - Security=High, Spec=none cited, `DE99B6`, PENDING
  - Falsified if: a workspace path that does not name an existing directory is granted as the workspace root
- `Broiler.Code.Linux.CodeShellFactory.OpenWorkspaceAsync(CodeShell, string?, CancellationToken)` in `src/Broiler.Code.Linux/CodeShellFactory.cs` - Security=High, Spec=none cited, `3D41E0`, PENDING
  - Falsified if: a path argument naming a file or a missing directory is granted as the workspace root instead of the scratch root
- `Broiler.Code.Linux.CodeShellFactory.ScratchRoot()` in `src/Broiler.Code.Linux/CodeShellFactory.cs` - Security=High, Spec=none cited, `893B77`, PENDING
  - Falsified if: a relative XDG_DATA_HOME is used as given, so the scratch workspace is created under the process's current directory
- `Broiler.Code.Linux.CodeWindow` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `27C58D`, PENDING
  - Falsified if: without --ignore-focus, keys typed after the window lost focus keep reaching the editor on later frames because the evdev devices stay active
- `Broiler.Code.Linux.CodeWindow.CodeWindow(BSize, Action<string>, bool, int)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `3DE714`, PENDING
  - Falsified if: a pollMilliseconds of zero or less reaches PeriodicTimer as a period below one millisecond
- `Broiler.Code.Linux.CodeWindow.TryGetText(out string)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `B03E24`, PENDING
  - Falsified if: with no X11 clipboard open, TryGetText returns true or a text other than the empty string
- `Broiler.Code.Linux.CodeWindow.SetText(string)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `B5EF0D`, PENDING
  - Falsified if: on a host with no X11 clipboard, SetText throws instead of dropping the text
- `Broiler.Code.Linux.CodeWindow.RunAsync(CancellationToken)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `B2CC6F`, PENDING
  - Falsified if: after WaitForNextTickAsync resumes on a thread-pool thread, the next Drain runs off the thread that constructed the dispatcher and throws InvalidOperationException
- `Broiler.Code.Linux.CodeWindow.DisposeAsync()` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `3A89A2`, PENDING
  - Falsified if: a second DisposeAsync disposes the X11 surface or the clipboard connection again
- `Broiler.Code.Linux.LinuxCodeInput` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, Spec=none cited, `52B01F`, PENDING
  - Falsified if: a key typed while another application has focus is dispatched to the editor although the window never reported focus to SetActiveAsync
- `Broiler.Code.Linux.LinuxCodeInput.IsAvailable` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, Spec=none cited, `380C67`, PENDING
  - Falsified if: IsAvailable reads false while a keyboard or mouse device is open, so SetActiveAsync(false) returns without stopping it
- `Broiler.Code.Linux.LinuxCodeInput.StartAsync(CancellationToken)` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, Spec=none cited, `5A21D5`, PENDING
  - Falsified if: an opened keyboard delivers key events to the pending queue before SetActiveAsync(true) has started it
- `Broiler.Code.Linux.LinuxCodeInput.SetActiveAsync(bool, CancellationToken)` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, Spec=none cited, `14B20F`, PENDING
  - Falsified if: SetActiveAsync(false) returns normally while the keyboard device is still started, so keys typed into another application are queued for the editor
- `Broiler.Code.Linux.LinuxCodeInput.DisposeAsync()` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, Spec=none cited, `724DAA`, PENDING
  - Falsified if: after DisposeAsync completes, the keyboard or mouse device is still open or its events still reach the pending queue
- `Broiler.Code.Linux.LinuxCodeInput.OpenKeyboardAsync(CancellationToken)` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, Spec=none cited, `5E3814`, PENDING
  - Falsified if: when opening the first available keyboard throws, the next available keyboard is not tried and keyboard input stays disabled
- `Broiler.Code.Linux.LinuxCodeInput.OpenMouseAsync(CancellationToken)` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, Spec=none cited, `D690DE`, PENDING
  - Falsified if: when opening the first available mouse throws, the next available mouse is not tried and mouse input stays disabled
- `Broiler.Code.Linux.LinuxFileDialogs` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `D8426B`, PENDING
  - Falsified if: a grant from an open, save or folder request roots storage at a directory other than the chosen folder or the directory that contains the chosen file
- `Broiler.Code.Linux.LinuxFileDialogs.LinuxFileDialogs()` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `BEF9D7`, PENDING
  - Falsified if: IsAvailable is true while neither zenity nor kdialog is present on PATH
- `Broiler.Code.Linux.LinuxFileDialogs.RequestOpenAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `33ED51`, PENDING
  - Falsified if: a chooser the user cancelled (helper exits non-zero) yields a grant instead of null
- `Broiler.Code.Linux.LinuxFileDialogs.RequestSaveAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `105FBA`, PENDING
  - Falsified if: the grant for a chosen save path lets storage write outside the directory that contains that path
- `Broiler.Code.Linux.LinuxFileDialogs.RequestFolderAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `F9F393`, PENDING
  - Falsified if: the grant for a chosen folder is rooted at its parent directory rather than at the folder itself
- `Broiler.Code.Linux.LinuxFileDialogs.RunAsync(FileDialogRequest, DialogMode, CancellationToken)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `3C076F`, PENDING
  - Falsified if: a helper that writes more than a pipe buffer of warnings to standard error never exits, because standard error is redirected and never read, and the dialog never returns
- `Broiler.Code.Linux.LinuxFileDialogs.Arguments(string, FileDialogRequest, DialogMode)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `EF8CCD`, PENDING
  - Falsified if: a suggested name beginning with a dash is passed to kdialog as a bare argument, where it is parsed as an option instead of a start path
- `Broiler.Code.Linux.LinuxFileDialogs.FolderGrant(string?)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `1C003D`, PENDING
  - Falsified if: the storage for a chosen folder is rooted anywhere other than that folder
- `Broiler.Code.Linux.LinuxFileDialogs.Grant(string?)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `F14BCF`, PENDING
  - Falsified if: the storage for a chosen file is rooted anywhere other than that file's own directory, or the returned name carries a directory part
- `Broiler.Code.Linux.LinuxFileDialogs.FindHelper()` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, Spec=none cited, `8CB137`, PENDING
  - Falsified if: a zenity file without execute permission earlier on PATH is chosen over an executable helper later on PATH, and the dialog then fails to start
- `Broiler.Code.Linux.Program` in `src/Broiler.Code.Linux/Program.cs` - Security=High, Spec=none cited, `D26BF0`, PENDING
  - Falsified if: input is read while another window has focus although --ignore-focus was not among the arguments
- `Broiler.Code.Linux.Program.Main(string[])` in `src/Broiler.Code.Linux/Program.cs` - Security=High, Spec=none cited, `C7D1FB`, PENDING
  - Falsified if: input is read while another window has focus although --ignore-focus was not among the arguments
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `CCE5C4`, PENDING
  - Falsified if: an insert entry naming a path outside the root, or a file the component does not cover, gets a block written into that file
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Done` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `734E8F`, PENDING
  - Falsified if: Done equals Refused or UsageError, so the exit code cannot tell a clean check from one that found a violation
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Refused` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `7C5B04`, PENDING
  - Falsified if: Refused is zero, so a check that found a violation exits as a success and the gate passes
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.UsageError` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `8285EA`, PENDING
  - Falsified if: UsageError is zero, so a check given an unknown option exits as a success having applied no rule
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Run(IReadOnlyList<string>, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `02034E`, PENDING
  - Falsified if: an unknown subcommand, or an option the subcommand does not accept, exits 0
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.ScannerFor(AssuranceComponentConfig?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `8CC1F8`, PENDING
  - Falsified if: with no configuration the scanner is built with a predicate other than the strict one, so a unit the strict predicate counts as relevant is listed as exempt
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.List(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `2ADA2B`, PENDING
  - Falsified if: a run in which a covered file could not be read, or a --strict run whose --files list names an uncovered path, exits 0
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Insert(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `AAA6E4`, PENDING
  - Falsified if: with --dry-run, a file with applied entries is still written to disk
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.LoadConfig(string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `D4F931`, PENDING
  - Falsified if: an assurance.config.json at the root that fails to parse comes back as null instead of stopping the run with a configuration error
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.RootOf(Options)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `9BD9C8`, PENDING
  - Falsified if: a --root that names a file or a missing path is returned instead of raising a usage error
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.PrepareOut(string?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `AC1291`, PENDING
  - Falsified if: probing an existing --json target truncates it or deletes it
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.WriteOut(string, string, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, Spec=none cited, `E2CFD9`, PENDING
  - Falsified if: a report that could not be written returns true and prints that it was written
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, Spec=none cited, `CCE5C4`, PENDING
  - Falsified if: generate writes a source file or artefact in a run that also reported a refusal
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Generate(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, Spec=none cited, `929E74`, PENDING
  - Falsified if: a run with a refusal, or with an existing report the generator did not write and no --adopt, still writes a file
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Check(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, Spec=none cited, `39CE19`, PENDING
  - Falsified if: a run whose only problem is a covered file that could not be read as UTF-8 exits 0
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Status(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, Spec=none cited, `87451D`, PENDING
  - Falsified if: a unit whose human line still reads PENDING is included in the count printed for units a person has decided
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.OwnedConfig(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, Spec=none cited, `444F24`, PENDING
  - Falsified if: a component with no assurance.config.json at its root, or one whose mode is external, gets a configuration back and a write command proceeds
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.ReadableConfig(string, Options, string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, Spec=none cited, `9F9308`, PENDING
  - Falsified if: a --config path that does not exist falls back to the configuration at the root instead of raising a usage error
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.WorkflowCommand(AssuranceViolation, string?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, Spec=none cited, `0CF246`, PENDING
  - Falsified if: a violation whose message spans two lines is emitted as two output lines, so the second one can start a workflow command of its own
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.EscapeData(string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, Spec=none cited, `C0E98F`, PENDING
  - Falsified if: a message holding a carriage return or line feed is emitted with the raw character instead of %0D or %0A
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.EscapeProperty(string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, Spec=none cited, `DC0025`, PENDING
  - Falsified if: a file path containing a comma or a colon is emitted unescaped, so it ends the file property and starts another
- `Broiler.Code.Review.Cli.Assurance.AssuranceJson` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceJson.cs` - Security=High, Spec=none cited, `007837`, PENDING
  - Falsified if: an assessments entry carrying a field for the human line is read into an assessment instead of refused
- `Broiler.Code.Review.Cli.Assurance.AssuranceJson.AssessmentFields` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceJson.cs` - Security=High, Spec=none cited, `62AFDF`, PENDING
  - Falsified if: the list names a field for the human line, so an assessments entry can set it
- `Broiler.Code.Review.Cli.Assurance.AssuranceJson.ReadAssessments(string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceJson.cs` - Security=High, Spec=none cited, `04D3A0`, PENDING
  - Falsified if: a document whose top-level object carries a property other than schema, assessments and $comment is read instead of refused
- `Broiler.Code.Review.Cli.Assurance.AssuranceJson.ReadEntry(JsonElement, int)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceJson.cs` - Security=High, Spec=none cited, `E4100D`, PENDING
  - Falsified if: an entry carrying a property the schema does not define, such as one named for the human line, comes back with no problem
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand` in `src/Broiler.Code.Review.Cli/Assurance/AssurancePruneCommand.cs` - Security=High, Spec=none cited, `CCE5C4`, PENDING
  - Falsified if: after prune, a block whose human line names a person or reads STALE has lost a line
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Prune(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssurancePruneCommand.cs` - Security=High, Spec=none cited, `E283D3`, PENDING
  - Falsified if: a covered file is written in a run given --dry-run, or in a component whose configuration is missing or says mode external
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.ReadBack(ComponentSourceFile, AssuranceSourceText, string, CSharpAssuranceFileScanner)` in `src/Broiler.Code.Review.Cli/Assurance/AssurancePruneCommand.cs` - Security=High, Spec=none cited, `4D2D2F`, PENDING
  - Falsified if: a written file whose units or file fingerprint scan differently from the file as read is left on disk instead of getting its original bytes back
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, Spec=none cited, `CA4609`, PENDING
  - Falsified if: a file whose bytes are not valid UTF-8 is accepted, so a later write replaces those bytes with U+FFFD
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.Utf8Bom` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, Spec=none cited, `FFCB5B`, PENDING
  - Falsified if: a file that opens with EF BB BF is written back without those three bytes
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.Utf16LittleEndianBom` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, Spec=none cited, `AD46DD`, PENDING
  - Falsified if: a file that opens with FF FE is not refused as UTF-16 or UTF-32 before decoding
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.Utf16BigEndianBom` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, Spec=none cited, `844152`, PENDING
  - Falsified if: a file that opens with FE FF is not refused as UTF-16 before decoding
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.StrictUtf8` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, Spec=none cited, `3E00C8`, PENDING
  - Falsified if: decoding a byte sequence that is not valid UTF-8, such as a lone 0x80, yields U+FFFD instead of throwing
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.TryRead(string, out AssuranceSourceText?, out string?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, Spec=none cited, `BEA0D1`, PENDING
  - Falsified if: a file containing a byte such as 0xFF in its body is returned as source text instead of with a problem
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.TryWrite(string, string, out string?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, Spec=none cited, `5A02F9`, PENDING
  - Falsified if: a file whose bytes changed on disk after TryRead is overwritten instead of refused
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, Spec=none cited, `ABFA27`, PENDING
  - Falsified if: an artefact path whose directory is a junction leading outside the component root is written through it
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.Load(string, AssuranceComponentConfig)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, Spec=none cited, `265A60`, PENDING
  - Falsified if: a covered file that is not valid UTF-8 is left out of the corpus with no IO entry in Problems, so generate writes the rest and check reports nothing about it
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.CheckWritable(string, string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, Spec=none cited, `E072E0`, PENDING
  - Falsified if: an artefact path that is itself a symbolic link passes without a ComponentSourceException
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.SeparateRecords(string, AssuranceArtefactPaths)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, Spec=none cited, `410865`, PENDING
  - Falsified if: a hand-written HUMAN_REVIEW.md at the default path is left out of the result while the configuration points the per-unit record at another file
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.AdrRecords(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, Spec=none cited, `036BD4`, PENDING
  - Falsified if: an ADR number resolves when the ADR directory holds no *.md file whose name begins with that number followed by a hyphen
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.TryWrite(AssuranceArtefact, out string?)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, Spec=none cited, `3D2205`, PENDING
  - Falsified if: a new artefact file that appeared on disk after Load is overwritten instead of refused
- `Broiler.Code.Review.Cli.Assurance.ComponentSources` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `71E8E4`, PENDING
  - Falsified if: a .cs file reached through a junction, or inside a directory holding a .git entry, is returned among the covered files
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.BuildOutput` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `1F5999`, PENDING
  - Falsified if: a .cs file under an obj directory directly below a project root is returned as covered
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Discover(string, AssuranceComponentConfig?)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `1AB32E`, PENDING
  - Falsified if: a configured project that lies inside a nested checkout is walked and its files covered instead of refused
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Restrict(ComponentSourceSet, string, IEnumerable<string>, out IReadOnlyList<ComponentUnknownPath>)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `8689F7`, PENDING
  - Falsified if: a --files list whose only line names an uncovered path returns a set that still holds covered files
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Find(ComponentSourceSet, string, string, out ComponentUnknownPath?)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `8A8F8C`, PENDING
  - Falsified if: on Linux a path that differs from a covered file only in letter case returns that file
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Normalize(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `AC07D0`, PENDING
  - Falsified if: a path such as src/../../x.cs comes back without a leading ../, so it is taken for a file inside the root
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.AssemblyNameOf(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `B1A16D`, PENDING
  - Falsified if: a project whose only AssemblyName sits in a PropertyGroup with a Condition returns that name instead of refusing
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.CheckClosedAssemblies(AssuranceComponentConfig, IEnumerable<ComponentProject>)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `9AE3E7`, PENDING
  - Falsified if: a configuration whose closedToEscapeHatch names an assembly no configured project builds passes without an exception
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Walk(string, string, HashSet<string>, bool)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `E07ED9`, PENDING
  - Falsified if: a subdirectory that is a junction is descended into and its .cs files are yielded with no skip reason
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Unenterable(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `1C607B`, PENDING
  - Falsified if: on Windows a directory segment spelt .git. (which the file system opens as .git) returns null instead of a .git barrier
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.IsNestedCheckout(string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `057A2F`, PENDING
  - Falsified if: a directory whose .git entry is a file, as in a submodule checkout, is reported as not a nested checkout
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.IsLink(string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `895528`, PENDING
  - Falsified if: a directory junction, or a path whose attributes cannot be read, returns false
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.CompileItems(string, string, ComponentProject)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `C2DC5E`, PENDING
  - Falsified if: an include using an MSBuild property other than the two it expands is resolved or dropped instead of yielded as unresolved
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.ProjectFiles(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `1476CB`, PENDING
  - Falsified if: an Import whose literal path leads outside the component root is loaded
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.DirectoryBuildFiles(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `F30897`, PENDING
  - Falsified if: a Directory.Build.props in a directory above the component root is returned
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Resolve(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `0A7161`, PENDING
  - Falsified if: an include such as ../../../../**/*.cs, whose wildcard base lies outside the component root, has that base directory enumerated
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.IsUnder(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `49023D`, PENDING
  - Falsified if: a path in a sibling directory whose name begins with the directory name, such as root2/x.cs against root, counts as under it
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Load(string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `177EB9`, PENDING
  - Falsified if: a project file declaring an external entity has it resolved, so text from a file outside the component appears in an element value
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.GuessProjects(string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `EDE036`, PENDING
  - Falsified if: a .csproj inside a directory holding a .git entry is returned as a product project
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.LooksLikeProduct(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `DDA2BD`, PENDING
  - Falsified if: a project that declares IsTestProject true is reported as a product
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Relative(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, Spec=none cited, `75666A`, PENDING
  - Falsified if: a path outside the root comes back neither rooted nor starting with ../, so Discover takes it for a file inside the root
- `<top-level statements>` in `src/Broiler.Code.Review.Cli/Program.cs` - Security=High, Spec=none cited, `01FBE8`, PENDING
  - Falsified if: a stale review of Broiler.CSS/src/Broiler.CSS/Parsing/Tokenizer.cs is dropped from check's warnings when the changed-paths file lists only the Broiler.CSS gitlink
- `Broiler.Code.Review.Cli.ReviewReport` in `src/Broiler.Code.Review.Cli/ReviewReport.cs` - Security=High, Spec=none cited, `B86628`, PENDING
  - Falsified if: a recorded file whose text the storage refuses to read, such as a symbolic link, is reported as reviewed and unchanged
- `Broiler.Code.Review.Cli.ReviewReport.EvaluateAsync(string, IReadOnlyList<string>, CancellationToken)` in `src/Broiler.Code.Review.Cli/ReviewReport.cs` - Security=High, Spec=none cited, `2DB7D3`, PENDING
  - Falsified if: a recorded file whose text the storage refuses to read, such as a symbolic link, is reported as reviewed and unchanged
- `Broiler.Code.Review.Cli.ReviewReport.Regressions(IReadOnlyList<ReviewedFile>)` in `src/Broiler.Code.Review.Cli/ReviewReport.cs` - Security=High, Spec=none cited, `ED3B11`, PENDING
  - Falsified if: a file whose record says reviewed but whose content hash no longer matches is missing from the returned list
- `Broiler.Code.Review.Cli.InventoryOptions` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, Spec=none cited, `D8619C`, PENDING
  - Falsified if: a source file under a directory whose name only contains an excluded name, such as src/Broiler.Svg/ or src/binaries/, is left out of the inventory
- `Broiler.Code.Review.Cli.SourceInventory` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, Spec=none cited, `443E0E`, PENDING
  - Falsified if: a file added at a shallower path with the same identity, such as docs/Broiler.CSS/Parsing/Tokenizer.cs, displaces Broiler.CSS/src/Broiler.CSS/Parsing/Tokenizer.cs from the inventory
- `Broiler.Code.Review.Cli.SourceInventory.Enumerate(string, InventoryOptions?)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, Spec=none cited, `95817F`, PENDING
  - Falsified if: a file added at a shallower path with the same identity, such as docs/Broiler.CSS/Parsing/Tokenizer.cs, displaces Broiler.CSS/src/Broiler.CSS/Parsing/Tokenizer.cs from the inventory
- `Broiler.Code.Review.Cli.SourceInventory.IdentityOf(string)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, Spec=none cited, `CE03B4`, PENDING
  - Falsified if: Broiler.HTML/Broiler.CSS/src/Broiler.CSS/Parsing/Tokenizer.cs and Broiler.CSS/src/Broiler.CSS/Parsing/Tokenizer.cs reduce to different identities
- `Broiler.Code.Review.Cli.SourceInventory.ComponentDepth(string)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, Spec=none cited, `E0F13A`, PENDING
  - Falsified if: Broiler.Browser/Broiler.DOM/src/Broiler.DOM/Node.cs reports a depth no greater than Broiler.DOM/src/Broiler.DOM/Node.cs, so the copy wins the fold
- `Broiler.Code.Review.Cli.SourceInventory.Walk(string, string, InventoryOptions, List<string>)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, Spec=none cited, `0ACF3C`, PENDING
  - Falsified if: a symbolic link or junction to a directory outside the root is descended and the files under it are listed
- `Broiler.Code.Review.Cli.SourceInventory.IsGenerated(string)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, Spec=none cited, `F35499`, PENDING
  - Falsified if: a hand-written file no tool writes, such as Broiler.Input/src/Broiler.Input.Linux/AssemblyInfo.cs, is classed as generated and left out of the inventory
- `Broiler.Code.Review.Assurance.AssuranceAnnotation` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `48FCDA`, PENDING
  - Falsified if: a human line whose head is a placeholder such as TODO, followed by the unit's current fingerprint, yields a non-null name and stops blocking release
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.Previous` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `5DCCC5`, PENDING
  - Falsified if: a stale line whose preserved entry reads A@B@0A1B2C is split at its first '@', yielding the fingerprint B@0A1B2C
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.RecordedFingerprint` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `9DFC74`, PENDING
  - Falsified if: a machine line whose only Fingerprint key sits inside the value of another field reports that value's hex as the recorded fingerprint
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.ExemptReason` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `E8E581`, PENDING
  - Falsified if: a machine line whose only EXEMPT key sits inside the value of another field makes the unit exempt
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.HumanIsPending` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `913ECB`, PENDING
  - Falsified if: a body of exactly PENDING is read as not pending
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.HumanIsStale` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `24F979`, PENDING
  - Falsified if: a body opening with STALE and a preserved entry is read as not stale, so the earlier name is returned as the current decider
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.Reviewer` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `A5E68A`, PENDING
  - Falsified if: a head that is a placeholder such as TODO or NONE is returned as the name of the person who decided
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.HumanFingerprint` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `CD1BE3`, PENDING
  - Falsified if: a body whose only fingerprint sits inside another part's value, after that part's own key, is returned as the version decided on
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.HumanAssessment` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `331AD2`, PENDING
  - Falsified if: the fingerprint part of the old line is returned among the assessment parts, so the rewritten line carries two fingerprints
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.TryParse(AssuranceLines, int, out AssuranceAnnotation?)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `EA7190`, PENDING
  - Falsified if: a machine line followed by a line that is neither a criterion nor a human line is returned as a parsed block
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.TryParseStrict(AssuranceLines, int, out AssuranceAnnotation?, out string?)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `FAFD32`, PENDING
  - Falsified if: a machine line holding a part with no '=' sign is accepted as a parsed block
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.Field(string)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `6F1036`, PENDING
  - Falsified if: a lookup for EXEMPT returns the value of a field keyed exempt or Exempt
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.Body(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `DAFA8E`, PENDING
  - Falsified if: a line indented with tabs yields a body missing its first characters or still holding part of the marker
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.ParseFields(string)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, Spec=none cited, `E601E6`, PENDING
  - Falsified if: a field whose value itself contains '=' is split at the last '=', so its key absorbs part of the value
- `Broiler.Code.Review.Assurance.AssuranceBanner` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, Spec=none cited, `D473DF`, PENDING
  - Falsified if: a comment below the header opening with '// Unverified:' is not reported as a summary line
- `Broiler.Code.Review.Assurance.AssuranceBanner.SpdxCopyrightPrefix` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, Spec=none cited, `7156DE`, PENDING
  - Falsified if: a file whose first line is '// SPDX-FileCopyrightText: 2026 Broiler Platform contributors' is treated as carrying no generated header
- `Broiler.Code.Review.Assurance.AssuranceBanner.GeneratedMarker` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, Spec=none cited, `E40FCE`, PENDING
  - Falsified if: the marker line that closes a header Broiler.VM wrote is not recognised as the end of that header
- `Broiler.Code.Review.Assurance.AssuranceBanner.Banner` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, Spec=none cited, `BBCDE0`, PENDING
  - Falsified if: a second copy of the banner line pasted below the header is not counted as a duplicate banner
- `Broiler.Code.Review.Assurance.AssuranceBanner.BannerRule` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, Spec=none cited, `59EB43`, PENDING
  - Falsified if: the dashed rule line of a header Broiler.VM wrote is refused by the strip as a line the generator does not write
- `Broiler.Code.Review.Assurance.AssuranceBanner.SpdxLicensePrefix` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, Spec=none cited, `AFD5B4`, PENDING
  - Falsified if: the licence line of a header Broiler.VM wrote is refused by the strip as a line the generator does not write
- `Broiler.Code.Review.Assurance.AssuranceBanner.SpdxPrefix` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, Spec=none cited, `5707F3`, PENDING
  - Falsified if: a file opening with '// SPDX-License-Identifier: MIT' and no generated header gets a second header stacked above that line instead of a refusal
- `Broiler.Code.Review.Assurance.AssuranceBanner.RowLabels` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, Spec=none cited, `60C2A1`, PENDING
  - Falsified if: a comment below the header opening with '// Unverified:' is not reported as a summary line
- `Broiler.Code.Review.Assurance.AssuranceCandidates` in `src/Broiler.Code.Review/Assurance/AssuranceCandidates.cs` - Security=High, Spec=none cited, `19D2C6`, PENDING
  - Falsified if: a unit's state is resolved against the fingerprint or block of another unit in the same file, so a changed method stops blocking release under its neighbour's block
- `Broiler.Code.Review.Assurance.AssuranceCandidates.None` in `src/Broiler.Code.Review/Assurance/AssuranceCandidates.cs` - Security=High, Spec=none cited, `E3A72F`, PENDING
  - Falsified if: another reason constant carries the same text, so a unit with a malformed or half block is marked insertable
- `Broiler.Code.Review.Assurance.AssuranceCandidates.Classify(AssuranceLines, AssuranceScannedFile)` in `src/Broiler.Code.Review/Assurance/AssuranceCandidates.cs` - Security=High, Spec=none cited, `B22316`, PENDING
  - Falsified if: a unit's state is resolved against the fingerprint or block of another unit in the same file, so a changed method stops blocking release under its neighbour's block
- `Broiler.Code.Review.Assurance.AssuranceCandidates.ReasonFor(AssuranceFileUnit, AssuranceCandidate, AssuranceScannedFile, bool)` in `src/Broiler.Code.Review/Assurance/AssuranceCandidates.cs` - Security=High, Spec=none cited, `42E451`, PENDING
  - Falsified if: a unit whose leading trivia holds a stray human line with no machine line above it is reported insertable
- `Broiler.Code.Review.Assurance.AssuranceChecks` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `896DF1`, PENDING
  - Falsified if: a covered file whose annotation blocks or generated header differ from what generate would write passes Run with no violation
- `Broiler.Code.Review.Assurance.AssuranceChecks.Run(AssurancePlan, AssuranceComponentConfig, AssuranceCheckOptions)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `BFB66A`, PENDING
  - Falsified if: a relevant unit that carries no block in an indexable covered file produces no J1 violation
- `Broiler.Code.Review.Assurance.AssuranceChecks.MissingCriteria(IEnumerable<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `293283`, PENDING
  - Falsified if: a unit assessed Critical with no criterion line is absent from the returned messages
- `Broiler.Code.Review.Assurance.AssuranceChecks.MissingCriteriaOf(IEnumerable<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `24C868`, PENDING
  - Falsified if: a block that is not an exemption and assesses security as High with no criterion line is not yielded
- `Broiler.Code.Review.Assurance.AssuranceChecks.CriteriaBelowHighOf(IEnumerable<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `4BF82B`, PENDING
  - Falsified if: a block assessed Medium that carries a criterion line is not yielded, so a component that refuses one passes the check
- `Broiler.Code.Review.Assurance.AssuranceChecks.Orphans(AssurancePlannedFile)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `306FF8`, PENDING
  - Falsified if: a criterion comment that stands under no attached AI line produces no J2 violation
- `Broiler.Code.Review.Assurance.AssuranceChecks.SpecViolations(IEnumerable<AssuranceCorpusUnit>, IReadOnlySet<string>, string)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `4AAEE2`, PENDING
  - Falsified if: a Spec citation of ADR-0042 when the records hold no 0042 produces no J2 violation
- `Broiler.Code.Review.Assurance.AssuranceChecks.FingerprintViolations(IEnumerable<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `ABAE2C`, PENDING
  - Falsified if: a block whose recorded fingerprint differs from the one the unit's current tokens compute produces no J3 violation
- `Broiler.Code.Review.Assurance.AssuranceChecks.InventedApprovals(AssurancePlan)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `AB77B2`, PENDING
  - Falsified if: an alias that a generated artefact prints on a human line, and no human line in the source files carries, produces no J4 violation
- `Broiler.Code.Review.Assurance.AssuranceChecks.StaleArtefacts(AssurancePlan, bool)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `329817`, PENDING
  - Falsified if: with sources-only off, a report or manifest whose text differs from what the generator would write produces no J5 violation
- `Broiler.Code.Review.Assurance.AssuranceChecks.ReviewClaims(AssurancePlan, bool)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `DAAAF2`, PENDING
  - Falsified if: with sources-only on, a review claim written into the manifest comment on disk produces no J9 violation
- `Broiler.Code.Review.Assurance.AssuranceChecks.Unresolved(AssurancePlan)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, Spec=none cited, `74A884`, PENDING
  - Falsified if: with the release option on, a relevant unit left in a state that blocks a release produces no J11 violation
- `Broiler.Code.Review.Assurance.AssuranceArtefactPaths` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `35C1E6`, PENDING
  - Falsified if: a default artefact path is rooted or has a '..' or '.git' segment, which reaches the writer unchecked because only configured values pass CheckRelative
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `E258A8`, PENDING
  - Falsified if: an artefacts path whose segment Windows resolves to '.git', such as '.git./info/x', is accepted by Parse, so generate on Windows writes inside the git directory
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.FileName` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `AF4861`, PENDING
  - Falsified if: the opt-in file name matches a file components carry for another reason, so insert and generate write to a component that never opted in
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.CurrentSchema` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `0498C8`, PENDING
  - Falsified if: Parse accepts a schema number other than the one whose properties it defines
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Parse(string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `8BA3DA`, PENDING
  - Falsified if: a top-level property this schema does not define, such as a misspelled exclude, is accepted instead of raising AssuranceConfigException
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.ProjectList(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `54B79C`, PENDING
  - Falsified if: a project path with a '..' segment, a leading slash or a drive letter is returned instead of refused
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Exclusions(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `4CD578`, PENDING
  - Falsified if: an exclude object with no glob, or with a property other than glob and reason, is accepted instead of refused
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Overrides(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `035C64`, PENDING
  - Falsified if: an spdxOverrides entry with no glob is accepted instead of raising AssuranceConfigException
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.SpdxOf(JsonElement, string, bool)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `FD6E84`, PENDING
  - Falsified if: a copyright value that already carries its comment prefix, or spans two lines, is accepted into the generated header
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.ArtefactPaths(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `F7B64B`, PENDING
  - Falsified if: an artefacts entry with a '..' segment or a leading slash is accepted, so generate writes outside the component root
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.CommentLines(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `A15C57`, PENDING
  - Falsified if: a manifestComment whose first line lacks the generator's recognition prefix is accepted, so the next generate refuses to replace the manifest it wrote
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Symbols(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `CFB2D5`, PENDING
  - Falsified if: a preprocessor symbol holding a space or punctuation other than underscore is returned instead of refused
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Glob(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `1E3147`, PENDING
  - Falsified if: a glob that AssuranceGlob.TryParse refuses, such as one with a backslash or a '..' segment, is returned instead of raising AssuranceConfigException
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.StringList(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `0F58E7`, PENDING
  - Falsified if: an array element that is not a string, or is blank, is added to the list instead of refused
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.RelativePath(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `8513F6`, PENDING
  - Falsified if: an adrDirectory or artefacts value with a '..' segment is returned without passing CheckRelative
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.CheckRelative(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `56B0EB`, PENDING
  - Falsified if: a segment that Windows resolves to '.git', such as '.git.' in '.git./hooks/x', is accepted, so an artefact path lands inside the git directory
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.SingleLine(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `BE0454`, PENDING
  - Falsified if: a component, reason, license or regenerateCommand value holding a line feed is returned instead of refused
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.CheckSingleLine(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `5B6EAE`, PENDING
  - Falsified if: a value containing a carriage return, U+0085, U+2028 or U+2029 is returned instead of raising AssuranceConfigException
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.RequiredString(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `EBCC94`, PENDING
  - Falsified if: a JSON number, or a string of only spaces, is returned as a value instead of refused
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.RequiredBool(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, Spec=none cited, `B489D0`, PENDING
  - Falsified if: the JSON string "false" or a number is read as a boolean instead of raising AssuranceConfigException
- `Broiler.Code.Review.Assurance.AssuranceCorpusUnit` in `src/Broiler.Code.Review/Assurance/AssuranceCorpus.cs` - Security=High, Spec=none cited, `492720`, PENDING
  - Falsified if: a candidate the exemption predicate does not exempt reads as not relevant, so the J1 and J11 rules skip it
- `Broiler.Code.Review.Assurance.AssuranceCorpusUnit.IsRelevant` in `src/Broiler.Code.Review/Assurance/AssuranceCorpus.cs` - Security=High, Spec=none cited, `DC666B`, PENDING
  - Falsified if: a candidate the exemption predicate does not exempt reads as not relevant, so the J1 and J11 rules skip it
- `Broiler.Code.Review.Assurance.AssuranceDocument` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `59E35A`, PENDING
  - Falsified if: signing a unit writes a body carrying a fingerprint part that the unit's human line did not already hold
- `Broiler.Code.Review.Assurance.AssuranceDocument.AssuranceDocument(AssuranceLines, IReadOnlyList<AssuranceUnit>, int, IAssuranceUnitScanner?, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `112B63`, PENDING
  - Falsified if: a document constructed with a null scanner reports HasUnitScanner true
- `Broiler.Code.Review.Assurance.AssuranceDocument.Read(string, IAssuranceUnitScanner?, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `EB72F1`, PENDING
  - Falsified if: Read with no scanner yields a Summary or sets BannerIsReproducible true
- `Broiler.Code.Review.Assurance.AssuranceDocument.ApprovalBody(AssuranceUnit, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `7D6D15`, PENDING
  - Falsified if: for a unit whose line names a different alias beside a fingerprint part, the returned body still carries a fingerprint part
- `Broiler.Code.Review.Assurance.AssuranceDocument.Approve(AssuranceUnit, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `D1F2FB`, PENDING
  - Falsified if: signing an exempt unit, or signing with a placeholder name such as TODO, returns Applied
- `Broiler.Code.Review.Assurance.AssuranceDocument.Withdraw(AssuranceUnit)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `C57ABC`, PENDING
  - Falsified if: Withdraw leaves a name or a fingerprint part on the human line instead of the bare pending word
- `Broiler.Code.Review.Assurance.AssuranceDocument.Rewrite(AssuranceUnit, AssuranceAnnotation, string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `614506`, PENDING
  - Falsified if: a rewrite changes a line other than the annotation's human line and the header's own lines
- `Broiler.Code.Review.Assurance.AssuranceDocument.RecountBanner()` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `6D4DDE`, PENDING
  - Falsified if: header lines are replaced in a document whose header did not reproduce when it was read
- `Broiler.Code.Review.Assurance.AssuranceDocument.Summarize()` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `707900`, PENDING
  - Falsified if: a unit whose criterion line is present but empty is left out of the Criteria count that AssuranceSummary.Of includes it in
- `Broiler.Code.Review.Assurance.AssuranceDocument.ReproducesBanner(AssuranceSummary)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `08F3BE`, PENDING
  - Falsified if: a header that differs from the rendered block in one row, or carries a second copyright line, is reported as reproduced
- `Broiler.Code.Review.Assurance.AssuranceDocument.FromScanner(AssuranceLines, IAssuranceUnitScanner, string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `75DC54`, PENDING
  - Falsified if: a unit whose block is an exemption line is returned with IsExempt false
- `Broiler.Code.Review.Assurance.AssuranceDocument.FromAnnotations(AssuranceLines)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `63D0DF`, PENDING
  - Falsified if: a machine line with no human line under it, directly or after one criterion line, is returned as a unit
- `Broiler.Code.Review.Assurance.AssuranceDocument.AnnotationAbove(AssuranceLines, int)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, Spec=none cited, `65ECB4`, PENDING
  - Falsified if: a block separated from the declaration by an attribute line that is not the declaration's own, such as an assembly attribute, is attached to that declaration
- `Broiler.Code.Review.Assurance.AssuranceArtefact` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `C97211`, PENDING
  - Falsified if: an artefact that does not exist on disk reports IsCurrent true
- `Broiler.Code.Review.Assurance.AssuranceArtefact.IsCurrent` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `3794DB`, PENDING
  - Falsified if: an artefact whose text on disk differs from the desired text only in its line endings reports IsCurrent true
- `Broiler.Code.Review.Assurance.AssurancePlan` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `938B0D`, PENDING
  - Falsified if: a plan with a refused file lists that file among Changes
- `Broiler.Code.Review.Assurance.AssurancePlan.Changes` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `3E9313`, PENDING
  - Falsified if: an artefact whose text on disk differs from its desired text is left out of Changes
- `Broiler.Code.Review.Assurance.AssuranceGenerator` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `049625`, PENDING
  - Falsified if: Plan gives a file a desired text whose human line names an alias the line as read did not carry
- `Broiler.Code.Review.Assurance.AssuranceGenerator.GeneratedNotice` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `D36857`, PENDING
  - Falsified if: the notice is not how the report, record and manifest renderers open their text, so a regenerated artefact reads as hand-written to IsGenerated
- `Broiler.Code.Review.Assurance.AssuranceGenerator.Plan(AssuranceCorpus, IAssuranceFileScanner, AssuranceComponentConfig)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `1558C3`, PENDING
  - Falsified if: a plan in which a file was refused carries for that file a desired text other than the text as read
- `Broiler.Code.Review.Assurance.AssuranceGenerator.IsGenerated(string)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `25C109`, PENDING
  - Falsified if: a text whose only notice line is its eleventh line is reported as generated
- `Broiler.Code.Review.Assurance.AssuranceGenerator.Artefact(AssuranceCorpus, string, AssuranceArtefactKind, string)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `CF0CB4`, PENDING
  - Falsified if: a component artefact absent from the corpus is built with Exists true, or with the desired text as its current text
- `Broiler.Code.Review.Assurance.AssuranceGenerator.PlanFile(AssuranceSource, IAssuranceFileScanner, AssuranceComponentConfig, List<AssuranceViolation>)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `50FC57`, PENDING
  - Falsified if: a file whose code units or file fingerprint differ between the text as read and the desired text is planned with that desired text instead of refused
- `Broiler.Code.Review.Assurance.AssuranceGenerator.Refresh(string, IReadOnlyList<AssuranceCorpusUnit>, List<AssuranceViolation>)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `FF9BF1`, PENDING
  - Falsified if: a file with a human line that Refreshed or RefuseInventedApproval refuses comes back as text instead of null
- `Broiler.Code.Review.Assurance.AssuranceGenerator.RefreshedFields(AssuranceAnnotation, string)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `C74CFD`, PENDING
  - Falsified if: a machine line with no fingerprint field comes back with one added
- `Broiler.Code.Review.Assurance.AssuranceGenerator.Units(AssuranceSource, AssuranceLines, AssuranceScannedFile)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `57D97E`, PENDING
  - Falsified if: a returned unit carries a file path or assembly other than the source's
- `Broiler.Code.Review.Assurance.AssuranceGenerator.ChangedCode(AssuranceScannedFile, AssuranceScannedFile, AssuranceLines)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, Spec=none cited, `EE73F6`, PENDING
  - Falsified if: two scans whose units match in name and fingerprint but differ in exemption return null
- `Broiler.Code.Review.Assurance.AssuranceGlob` in `src/Broiler.Code.Review/Assurance/AssuranceGlob.cs` - Security=High, Spec=none cited, `458A43`, PENDING
  - Falsified if: a name-only pattern of eight '*a' groups followed by '*b' takes seconds to reject a 40-character file name made of 'a', because the expression has no match timeout
- `Broiler.Code.Review.Assurance.AssuranceGlob.TryParse(string?, out AssuranceGlob?, out string?)` in `src/Broiler.Code.Review/Assurance/AssuranceGlob.cs` - Security=High, Spec=none cited, `94206A`, PENDING
  - Falsified if: a pattern with a '..' segment, or a drive prefix such as 'C:/src', is accepted
- `Broiler.Code.Review.Assurance.AssuranceGlob.IsMatch(string)` in `src/Broiler.Code.Review/Assurance/AssuranceGlob.cs` - Security=High, Spec=none cited, `333AFA`, PENDING
  - Falsified if: a name-only pattern of eight '*a' groups followed by '*b' takes seconds to reject a 40-character file name made of 'a', because the expression has no match timeout
- `Broiler.Code.Review.Assurance.AssuranceGlob.ToRegex(string)` in `src/Broiler.Code.Review/Assurance/AssuranceGlob.cs` - Security=High, Spec=none cited, `86EA4E`, PENDING
  - Falsified if: a '.' in the pattern, as in '*.g.cs', matches any character, so 'xAgBcs' matches
- `Broiler.Code.Review.Assurance.AssuranceHeader` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `6ED41D`, PENDING
  - Falsified if: Strip deletes a copyright line that the configuration does not state for the file without returning a refusal
- `Broiler.Code.Review.Assurance.AssuranceHeader.NormalizedRowLabels` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `4415CB`, PENDING
  - Falsified if: a normalized label keeps its trailing colon or its upper case, so a comment opening 'relevant units : 3' below the header is not reported
- `Broiler.Code.Review.Assurance.AssuranceHeader.StrictVocabulary` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `DAF119`, PENDING
  - Falsified if: a term holds an upper-case letter, so IsSummaryComment, which compares terms ordinally against lower-cased text, never matches it
- `Broiler.Code.Review.Assurance.AssuranceHeader.NarrowOpenings` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `AA4C2E`, PENDING
  - Falsified if: an opening keeps the leading slashes and space of the banner or the marker, so a line whose comment body opens with the banner text is not accepted by IsSummaryLine
- `Broiler.Code.Review.Assurance.AssuranceHeader.IsSummaryLine(string, AssuranceForgeryVocabulary)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `35AD38`, PENDING
  - Falsified if: under the narrow vocabulary a '//' line whose body opens with 'unverified:' in lower case is not accepted as a summary line
- `Broiler.Code.Review.Assurance.AssuranceHeader.BannerCount(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `AF95F5`, PENDING
  - Falsified if: an indented or lower-case copy of the banner line is not counted
- `Broiler.Code.Review.Assurance.AssuranceHeader.DuplicateBanners(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `9E099B`, PENDING
  - Falsified if: a text carrying two banner lines returns null
- `Broiler.Code.Review.Assurance.AssuranceHeader.IsSummaryComment(string, AssuranceForgeryVocabulary)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `E77B11`, PENDING
  - Falsified if: a comment 'Relevant-Units : 3', with a hyphen and a space before the colon, is not reported under the narrow vocabulary
- `Broiler.Code.Review.Assurance.AssuranceHeader.ForgedSummary(string, string, IReadOnlyList<AssuranceCommentLine>, AssuranceForgeryVocabulary)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `F41069`, PENDING
  - Falsified if: a row-label comment inside a documentation comment below the generated marker returns null
- `Broiler.Code.Review.Assurance.AssuranceHeader.CommentContent(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `54B271`, PENDING
  - Falsified if: a block-comment line ' * Exempt: 3 */' comes back still holding its asterisks or closing delimiter, so the label check misses it
- `Broiler.Code.Review.Assurance.AssuranceHeader.WordsOf(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `9B9031`, PENDING
  - Falsified if: a run of two or more characters that are neither letters nor digits between two words comes back as more than one space
- `Broiler.Code.Review.Assurance.AssuranceHeader.GeneratedHeaderLines(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `A8E833`, PENDING
  - Falsified if: a text that opens with a complete generated header is returned without its marker line
- `Broiler.Code.Review.Assurance.AssuranceHeader.Strip(AssuranceLines, IReadOnlyList<string>, AssuranceForgeryVocabulary, string, out IReadOnlyList<string>)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `64DCF0`, PENDING
  - Falsified if: a second header under the first, whose copyright line names a holder the configuration does not state, is removed without a refusal
- `Broiler.Code.Review.Assurance.AssuranceHeader.IsHeaderShaped(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `FBBD80`, PENDING
  - Falsified if: a documentation line, or the machine line of an assurance block, counts as header-shaped
- `Broiler.Code.Review.Assurance.AssuranceHeader.LeadingRunIsGeneratedCopy(AssuranceLines)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, Spec=none cited, `BCE73F`, PENDING
  - Falsified if: a leading run that holds a documentation line or an assurance block line before its own marker is reported as a copy
- `Broiler.Code.Review.Assurance.AssuranceHumanLine` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, Spec=none cited, `6C9EE3`, PENDING
  - Falsified if: Refreshed writes a body naming an alias that the line as written did not carry
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.PreviousMarker` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, Spec=none cited, `FC3986`, PENDING
  - Falsified if: the marker is spelled differently from the one Broiler.VM writes in a stale line, so a stale line that component wrote fails IsDefined and its file is refused
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.FingerprintMarker` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, Spec=none cited, `D3DF95`, PENDING
  - Falsified if: the marker lacks its trailing equals sign, so ReviewerNames drops any name that begins with the letters of the field name
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.Refreshed(AssuranceAnnotation, string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, Spec=none cited, `B9FC2E`, PENDING
  - Falsified if: a bare alias whose machine line records a fingerprint other than the current one comes back bound to the current fingerprint
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.IsDefined(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, Spec=none cited, `71E984`, PENDING
  - Falsified if: a body whose head is a placeholder such as TODO, followed by a fingerprint part, is reported as a defined shape
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.ReviewerNames(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, Spec=none cited, `B7EFD7`, PENDING
  - Falsified if: the name inside a stale line's previous-decision part is missing from the returned set
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.RefuseInventedApproval(string, string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, Spec=none cited, `6C4972`, PENDING
  - Falsified if: an after body naming an alias absent from the before body returns without throwing
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.BodiesIn(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, Spec=none cited, `3AD2CA`, PENDING
  - Falsified if: a human line indented with tabs is not returned
- `Broiler.Code.Review.Assurance.AssuranceInsertion` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, Spec=none cited, `160B8B`, PENDING
  - Falsified if: text returned by Apply holds a newly inserted human line that reads anything other than PENDING
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Validate(AssuranceAssessment)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, Spec=none cited, `93EBB6`, PENDING
  - Falsified if: an entry assessed High or Critical whose falsification criterion is only whitespace is returned with no problem
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Render(AssuranceAssessment, string)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, Spec=none cited, `B9DC7B`, PENDING
  - Falsified if: a rendered block's last line is anything other than the human marker followed by PENDING, whatever the entry holds
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Apply(string, string, IAssuranceFileScanner, IReadOnlyList<AssuranceAssessment>, bool, string?)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, Spec=none cited, `E5D891`, PENDING
  - Falsified if: two entries naming the same unit of one file are applied, or one of them is, instead of both being refused
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Resolve(IReadOnlyList<AssuranceCandidate>, AssuranceAssessment)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, Spec=none cited, `34BB23`, PENDING
  - Falsified if: an entry whose fingerprint matches none of the units of that name is resolved to one of them instead of being refused as stale
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Verify(string, string, string, IAssuranceFileScanner, AssuranceScannedFile, IReadOnlyList<AssuranceCandidate>, Dictionary<int, IReadOnlyList<string>>)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, Spec=none cited, `BA8C95`, PENDING
  - Falsified if: an inserted text that differs from the original in a character outside the inserted lines is answered with null
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Closed(string, string?, string[], List<string>)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, Spec=none cited, `84F930`, PENDING
  - Falsified if: a vocabulary value that differs from an allowed one only in letter case, such as 'high', adds no problem
- `Broiler.Code.Review.Assurance.AssuranceInsertion.IsOneLine(string)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, Spec=none cited, `79917E`, PENDING
  - Falsified if: a value holding a U+2028 line separator or a lone carriage return is reported as one line
- `Broiler.Code.Review.Assurance.AssuranceLines` in `src/Broiler.Code.Review/Assurance/AssuranceLines.cs` - Security=High, Spec=none cited, `995134`, PENDING
  - Falsified if: a text mixing CRLF, LF and lone CR endings does not render back byte for byte after being split and left unedited
- `Broiler.Code.Review.Assurance.AssuranceLines.AssuranceLines(string)` in `src/Broiler.Code.Review/Assurance/AssuranceLines.cs` - Security=High, Spec=none cited, `8C9593`, PENDING
  - Falsified if: a lone CR, an LF or a CRLF pair ends a line at a position other than where the C# parser starts its next line, so line N here is not the parser's line N
- `Broiler.Code.Review.Assurance.AssuranceLines.Render()` in `src/Broiler.Code.Review/Assurance/AssuranceLines.cs` - Security=High, Spec=none cited, `96E435`, PENDING
  - Falsified if: a text split and left unedited renders to a string that differs from the original in any character
- `Broiler.Code.Review.Assurance.AssuranceManifest` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `2C1E3B`, PENDING
  - Falsified if: a manifest whose recorded unit or file fingerprint differs from the value the tree computes now produces no J7 violation
- `Broiler.Code.Review.Assurance.AssuranceManifest.FilesArrayOpening` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `1EED07`, PENDING
  - Falsified if: the opening text matches a generated manifest at a position other than the start of its files array
- `Broiler.Code.Review.Assurance.AssuranceManifest.ArraysOf(string)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `D5575B`, PENDING
  - Falsified if: two manifests whose files or units arrays differ return equal strings
- `Broiler.Code.Review.Assurance.AssuranceManifest.Violations(string, IEnumerable<AssuranceManifestFile>, IEnumerable<AssuranceCorpusUnit>, string, string?)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `4AFDF1`, PENDING
  - Falsified if: a manifest entry whose fingerprint differs from the one its unit computes now produces no J7 violation
- `Broiler.Code.Review.Assurance.AssuranceManifest.Entry(AssuranceCorpusUnit)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `CD42F7`, PENDING
  - Falsified if: the expected entry for a unit carries a fingerprint, exempt flag or exemption other than the unit's own, so a changed unit matches its old manifest entry
- `Broiler.Code.Review.Assurance.AssuranceManifest.ShapeViolations(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `0489BC`, PENDING
  - Falsified if: a manifest with a fourth top-level property, or with its three properties in another order, yields no shape message
- `Broiler.Code.Review.Assurance.AssuranceManifest.FileViolations(string, IEnumerable<AssuranceManifestFile>, string)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `FAEA34`, PENDING
  - Falsified if: a covered file whose recorded file fingerprint differs from the one it computes now produces no message
- `Broiler.Code.Review.Assurance.AssuranceManifest.ReadUnits(string, string, List<string>)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `4C8862`, PENDING
  - Falsified if: a manifest whose units member is missing or is not an array is read as an empty list of entries instead of being reported
- `Broiler.Code.Review.Assurance.AssuranceManifest.ReadFiles(string, string, List<string>)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `EC7686`, PENDING
  - Falsified if: a manifest whose files member is missing or is not an array is read as an empty list of file entries instead of being reported
- `Broiler.Code.Review.Assurance.AssuranceManifest.Text(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, Spec=none cited, `C812A6`, PENDING
  - Falsified if: a property holding a JSON number, null, array or object is read as anything other than the empty string
- `Broiler.Code.Review.Assurance.AssurancePruning` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, Spec=none cited, `A23C54`, PENDING
  - Falsified if: the text Apply returns differs from the text as read in a line other than an assurance comment of a block whose human line reads exactly PENDING
- `Broiler.Code.Review.Assurance.AssurancePruning.Apply(string, string, IAssuranceFileScanner)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, Spec=none cited, `D0807E`, PENDING
  - Falsified if: a block whose human line reads anything other than exactly PENDING, such as PENDING followed by a fingerprint part, loses a line in the text Apply returns
- `Broiler.Code.Review.Assurance.AssurancePruning.CodeDifference(AssuranceScannedFile, AssuranceScannedFile)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, Spec=none cited, `B9CB54`, PENDING
  - Falsified if: two scans whose units match in name and fingerprint but differ in exemption are answered with null
- `Broiler.Code.Review.Assurance.AssurancePruning.RemovalFor(AssuranceCandidate, AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, Spec=none cited, `4FB20A`, PENDING
  - Falsified if: a block assessed High or Critical, or a block whose own exemption reason is what exempts its unit, is given lines to remove
- `Broiler.Code.Review.Assurance.AssurancePruning.Verify(string, string, string, IAssuranceFileScanner, AssuranceScannedFile, IReadOnlyList<AssuranceCandidate>, IReadOnlyDictionary<int, (int First, int Count, AssurancePruneKind Kind)>, IReadOnlyList<(int Line, string Text, string Separator)>)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, Spec=none cited, `94EC85`, PENDING
  - Falsified if: a pruned text that differs from the original outside the removed lines, or whose remaining block lost its criterion or changed a field, is answered with null
- `Broiler.Code.Review.Assurance.AssurancePruning.SameAssessment(AssuranceAnnotation, AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, Spec=none cited, `EE93FA`, PENDING
  - Falsified if: two blocks whose human lines differ, or whose machine fields differ in order, are answered the same assessment
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, Spec=none cited, `E4AB3C`, PENDING
  - Falsified if: a generated line stating a claim term with a count other than the annotations' count, and no negation before it in its clause, yields no J9 violation
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.Negations` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, Spec=none cited, `B64764`, PENDING
  - Falsified if: the list differs from the owning component's negation list, so a line one tool reports as a claim the other accepts as a denial
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.ClauseSeparators` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, Spec=none cited, `ED7CE7`, PENDING
  - Falsified if: a negation in an earlier clause, ended by a colon, semicolon or full stop, still excuses a claim term standing after it
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.Violations(string, IReadOnlyList<string>, IReadOnlyList<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, Spec=none cited, `59C6E3`, PENDING
  - Falsified if: a line stating a claim term with a number after it that differs from the annotations' count, and no negation before it in its clause, yields no violation
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.WholeWord(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, Spec=none cited, `85E526`, PENDING
  - Falsified if: a claim term standing alone between spaces is not found, so its line is never judged
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.IsSupported(string, int, string, IReadOnlyList<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, Spec=none cited, `C3BA8D`, PENDING
  - Falsified if: a claim term with no negation before it in its clause and a number after it that differs from the annotations' count is treated as supported
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.IsWordBefore(string, string, int)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, Spec=none cited, `BB5B2C`, PENDING
  - Falsified if: a negation standing after the claim term, or only inside a longer word such as cannot, is taken as denying it
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.Supported(string, IReadOnlyList<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, Spec=none cited, `6CF914`, PENDING
  - Falsified if: the count returned for the terms naming the current signed state includes a Stale, Unknown or exempt unit
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.FirstNumberAfter(string, int)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, Spec=none cited, `2D1281`, PENDING
  - Falsified if: a number standing before the claim term is returned as the count stated after it
- `Broiler.Code.Review.Assurance.AssuranceRules` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `BBF1D1`, PENDING
  - Falsified if: a criterion, exemption reason or Spec value containing a claim phrase passes VocabularyProblems with no problem
- `Broiler.Code.Review.Assurance.AssuranceRules.ReviewClaimTerms` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `C1DD35`, PENDING
  - Falsified if: a term of the owning component's J9 list is missing here, so a generated line using it passes this tool's check
- `Broiler.Code.Review.Assurance.AssuranceRules.SecurityRequiringACriterion` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `DE9342`, PENDING
  - Falsified if: the list lacks High or Critical, so a block at that level passes without a criterion line
- `Broiler.Code.Review.Assurance.AssuranceRules.SecurityWritingNoCriterion` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `8B2E2B`, PENDING
  - Falsified if: the list holds High or Critical, so prune takes the criterion off a block at the top of the vocabulary and insert refuses one there
- `Broiler.Code.Review.Assurance.AssuranceRules.CriterionProblems(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `1B72D8`, PENDING
  - Falsified if: a criterion stating one of the format's field names followed by an equals sign and a value yields no problem
- `Broiler.Code.Review.Assurance.AssuranceRules.ExemptionReasonProblems(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `320DC4`, PENDING
  - Falsified if: an exemption reason containing a claim phrase yields no problem
- `Broiler.Code.Review.Assurance.AssuranceRules.SpecProblems(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `19E322`, PENDING
  - Falsified if: a Spec value containing a claim phrase yields no problem
- `Broiler.Code.Review.Assurance.AssuranceRules.ReviewClaimsIn(string)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `0C28B4`, PENDING
  - Falsified if: a claim phrase the expression matches is left out of the returned list, so the text carrying it is accepted
- `Broiler.Code.Review.Assurance.AssuranceRules.VocabularyProblems(AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `703D05`, PENDING
  - Falsified if: a non-exempt block missing one of the five required fields yields no problem
- `Broiler.Code.Review.Assurance.AssuranceRules.RequiresFalsificationCriterion(AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `1B1FA3`, PENDING
  - Falsified if: a non-exempt block whose security value is High or Critical returns false
- `Broiler.Code.Review.Assurance.AssuranceRules.CarriesCriterionBelowHigh(AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `C3A4FA`, PENDING
  - Falsified if: a block that states an exemption, or one assessed High, is answered true, so prune takes its criterion line
- `Broiler.Code.Review.Assurance.AssuranceRules.FieldOnACriterion()` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `D5C99A`, PENDING
  - Falsified if: a format field name followed by an equals sign and a value, written in lower case or with spaces around the sign, is not matched
- `Broiler.Code.Review.Assurance.AssuranceRules.ReviewClaim()` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, Spec=none cited, `897B3A`, PENDING
  - Falsified if: a claim phrase written in mixed case, or with a tab between its two words, is not matched
- `Broiler.Code.Review.Assurance.IAssuranceFileScanner` in `src/Broiler.Code.Review/Assurance/AssuranceScannedFile.cs` - Security=High, Spec=none cited, `DFA515`, PENDING
  - Falsified if: a marker text standing inside a string literal or disabled code is reported among the assurance comment lines
- `Broiler.Code.Review.Assurance.IAssuranceFileScanner.ScanFile(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceScannedFile.cs` - Security=High, Spec=none cited, `CAA86D`, PENDING
  - Falsified if: a change to a declaration's tokens leaves the fingerprint ScanFile reports for it unchanged
- `Broiler.Code.Review.Assurance.IAssuranceUnitScanner` in `src/Broiler.Code.Review/Assurance/AssuranceUnit.cs` - Security=High, Spec=none cited, `A317C5`, PENDING
  - Falsified if: a change to a declaration's tokens leaves its reported fingerprint unchanged, so a human line recorded against the old code still matches
- `Broiler.Code.Review.Assurance.IAssuranceUnitScanner.Scan(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceUnit.cs` - Security=High, Spec=none cited, `0003C4`, PENDING
  - Falsified if: a declaration the owning component does not exempt is reported with IsExempt true, so it drops out of the relevant total
- `Broiler.Code.Review.Assurance.AssuranceUnitState` in `src/Broiler.Code.Review/Assurance/AssuranceUnitState.cs` - Security=High, Spec=none cited, `842B64`, PENDING
  - Falsified if: the enum's zero value is a state other than New, so a state that was never resolved reads as more than unassessed
- `Broiler.Code.Review.Assurance.AssuranceStateMachine` in `src/Broiler.Code.Review/Assurance/AssuranceUnitState.cs` - Security=High, Spec=none cited, `8008E0`, PENDING
  - Falsified if: a human line already reading STALE resolves to a state that does not block release, whatever fingerprint the unit now has
- `Broiler.Code.Review.Assurance.AssuranceStateMachine.Resolve(AssuranceAnnotation?, bool, string?)` in `src/Broiler.Code.Review/Assurance/AssuranceUnitState.cs` - Security=High, Spec=none cited, `D5E6D4`, PENDING
  - Falsified if: a human line naming a person whose recorded fingerprint differs from currentFingerprint resolves to a state that does not block release
- `Broiler.Code.Review.Assurance.AssuranceStateMachine.BlocksRelease(AssuranceUnitState)` in `src/Broiler.Code.Review/Assurance/AssuranceUnitState.cs` - Security=High, Spec=none cited, `90C958`, PENDING
  - Falsified if: a state other than the current signed one and Exempt, such as Unknown or Stale, is reported as not blocking release
- `Broiler.Code.Review.Assurance.AssuranceVocabulary` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `7D094C`, PENDING
  - Falsified if: a name made of a placeholder word with an invisible combining mark appended, such as PENDING followed by a variation selector, is accepted as an alias
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.AiMarker` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `0D2482`, PENDING
  - Falsified if: the marker differs by a byte from the one the owning component writes, so a block this tool writes is not attached to its unit there
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.FalsifiedIfMarker` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `08E4C4`, PENDING
  - Falsified if: the marker differs by a byte from the owning component's, so a criterion line this tool writes is not read as one there
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.HumanMarker` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `BBB9E0`, PENDING
  - Falsified if: the marker differs by a byte from the owning component's, so a human line the editor writes is not read as one there
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.Pending` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `CC1510`, PENDING
  - Falsified if: a human line reading exactly PENDING is read as naming a person
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.Stale` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `3B0062`, PENDING
  - Falsified if: a human line opening with STALE and a previous name is not recognised as stale and is read as naming a person
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.ToBeFilled` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `11804D`, PENDING
  - Falsified if: the placeholder differs from the owning component's, so a human line left for the generator is read as carrying a real fingerprint
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.FingerprintWidth` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `AD2165`, PENDING
  - Falsified if: a hex value of five or seven characters is accepted as a well-formed fingerprint
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.FingerprintField` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `6E00F2`, PENDING
  - Falsified if: the field name differs from the owning component's, so a fingerprint stated on a human line is not found and the line reads as left for the generator
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.ExemptField` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `BE996E`, PENDING
  - Falsified if: the field name differs from the owning component's, so a unit this tool reads as exempt blocks release there, or the reverse
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.HumanFieldMarkers` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `D911D4`, PENDING
  - Falsified if: a human line carrying a part after the name that opens with none of the four field prefixes, such as a second name, is read as a defined shape
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.IsWellFormedFingerprint(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `6A4AEF`, PENDING
  - Falsified if: a lowercase hex value, or one of a length other than six, is accepted as a fingerprint
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.MaxAliasLength` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `AA3F31`, PENDING
  - Falsified if: a name of 65 characters is accepted as an alias
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.PlaceholderWords` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `B3440B`, PENDING
  - Falsified if: a name whose only word is NOBODY, TODO or PENDING in mixed case is accepted as an alias
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.IsAlias(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `083C05`, PENDING
  - Falsified if: a name made of a placeholder word with an invisible combining mark appended, such as PENDING followed by a variation selector, is accepted as an alias
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.IsWritableReviewer(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `9610D6`, PENDING
  - Falsified if: a name that the alias rule refuses once trimmed is accepted for writing on a human line
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.AllHex(string)` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, Spec=none cited, `850147`, PENDING
  - Falsified if: a lowercase hex digit or a non-ASCII digit is accepted
- `Broiler.Code.Review.FileReview` in `src/Broiler.Code.Review/FileReview.cs` - Security=High, Spec=none cited, `0CBA60`, PENDING
  - Falsified if: code outside this assembly sets ReviewedContentHash on a record, so it can claim a hash for content nothing hashed
- `Broiler.Code.Review.FileReview.WithDecision(ReviewStatus, string, string, DateTimeOffset, string?)` in `src/Broiler.Code.Review/FileReview.cs` - Security=High, Spec=none cited, `53984F`, PENDING
  - Falsified if: a decision other than Unreviewed recorded with a name made only of whitespace is stored instead of throwing
- `Broiler.Code.Review.NoteAnchoring` in `src/Broiler.Code.Review/NoteAnchoring.cs` - Security=High, Spec=none cited, `1BE3D5`, PENDING
  - Falsified if: a note whose anchor text occurs at two or more places in the file, none of them its recorded line, is placed on one of them as Moved instead of reported as Ambiguous
- `Broiler.Code.Review.NoteAnchoring.Place(FileReview, string)` in `src/Broiler.Code.Review/NoteAnchoring.cs` - Security=High, Spec=none cited, `403C96`, PENDING
  - Falsified if: notes anchored on the LF text of a file come back Orphaned when the same file is passed with CRLF line endings or a leading byte-order mark
- `Broiler.Code.Review.NoteAnchoring.Place(ReviewNote, string[])` in `src/Broiler.Code.Review/NoteAnchoring.cs` - Security=High, Spec=none cited, `8033AA`, PENDING
  - Falsified if: a note whose anchor text is still at its recorded line is reported as Moved or Ambiguous because the same text also occurs elsewhere in the file
- `Broiler.Code.Review.NoteAnchoring.MatchesAt(string[], string[], int)` in `src/Broiler.Code.Review/NoteAnchoring.cs` - Security=High, Spec=none cited, `958350`, PENDING
  - Falsified if: a start index near int.MaxValue passes the bounds test through integer overflow and lines is indexed past its end
- `Broiler.Code.Review.ReviewContentHash` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, Spec=none cited, `136B7E`, PENDING
  - Falsified if: a Reviewed record keeps Current freshness after its file gains or loses indentation or trailing whitespace
- `Broiler.Code.Review.ReviewContentHash.Algorithm` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, Spec=none cited, `6BDB9F`, PENDING
  - Falsified if: the prefix Compute writes differs from the one IsKnownAlgorithm accepts, so a freshly recorded hash evaluates as Unknown
- `Broiler.Code.Review.ReviewContentHash.ByteOrderMark` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, Spec=none cited, `78F9B3`, PENDING
  - Falsified if: the constant is a character other than U+FEFF, so a file whose first character is that character hashes the same as the file without it
- `Broiler.Code.Review.ReviewContentHash.Compute(string)` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, Spec=none cited, `EBDA35`, PENDING
  - Falsified if: two texts that differ only in trailing whitespace, indentation or letter case produce the same value
- `Broiler.Code.Review.ReviewContentHash.Matches(string?, string)` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, Spec=none cited, `D8CF0E`, PENDING
  - Falsified if: a recorded value made of the algorithm prefix and only part of the current digest is reported as matching the text
- `Broiler.Code.Review.ReviewContentHash.IsKnownAlgorithm(string?)` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, Spec=none cited, `91AFA9`, PENDING
  - Falsified if: a value that is the prefix without its colon, or the prefix followed by further letters such as sha256-nlfx:, is accepted as a known algorithm
- `Broiler.Code.Review.ReviewContentHash.Normalize(string)` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, Spec=none cited, `9881D8`, PENDING
  - Falsified if: two texts that differ other than in line-ending style or a single leading U+FEFF normalize to the same string
- `Broiler.Code.Review.ReviewCoverageTotals` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, Spec=none cited, `59DEC7`, PENDING
  - Falsified if: VerifiedPercent counts files in StaleApprovals, Flagged or Unreviewed toward its figure, so the coverage gate passes below its minimum
- `Broiler.Code.Review.ReviewCoverageTotals.VerifiedPercent` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, Spec=none cited, `B17075`, PENDING
  - Falsified if: a file counted in StaleApprovals raises VerifiedPercent
- `Broiler.Code.Review.ReviewCoverage` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, Spec=none cited, `A88676`, PENDING
  - Falsified if: a file listed with a Reviewed state that is Stale or Unknown raises the VerifiedPercent of the totals returned
- `Broiler.Code.Review.ReviewCoverage.Overall(IEnumerable<ReviewedFile>, string)` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, Spec=none cited, `62601C`, PENDING
  - Falsified if: a file in the input is left out of Total, which raises VerifiedPercent
- `Broiler.Code.Review.ReviewCoverage.Count(string, IEnumerable<ReviewedFile>)` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, Spec=none cited, `982D8D`, PENDING
  - Falsified if: a file whose state is Reviewed with Stale or Unknown freshness is added to the count behind VerifiedPercent
- `Broiler.Code.Review.ReviewJson` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, Spec=none cited, `565A83`, PENDING
  - Falsified if: Read returns ReviewStatus.Reviewed for a record whose status field is missing, misspelled or not a string
- `Broiler.Code.Review.ReviewJson.Write(FileReview)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, Spec=none cited, `22AD25`, PENDING
  - Falsified if: a record with a declared status and a non-empty content hash, written by Write and read back by Read, comes back with a different status, content hash or number of notes
- `Broiler.Code.Review.ReviewJson.Read(string, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, Spec=none cited, `9F281D`, PENDING
  - Falsified if: a record whose JSON repeats a property name in one object makes Read throw instead of returning null or a record
- `Broiler.Code.Review.ReviewJson.ReadNote(JsonObject, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, Spec=none cited, `39D936`, PENDING
  - Falsified if: a startLine of int.MinValue wraps to int.MaxValue when converted to zero-based instead of being clamped to line 0
- `Broiler.Code.Review.ReviewJson.ToWire(ReviewStatus)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, Spec=none cited, `86FF42`, PENDING
  - Falsified if: a Question, NeedsChange or InReview status is written as reviewed
- `Broiler.Code.Review.ReviewJson.FromWire(string?)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, Spec=none cited, `52FF2D`, PENDING
  - Falsified if: a status string other than exactly reviewed, such as Reviewed in capitals or with a trailing space, is read as ReviewStatus.Reviewed
- `Broiler.Code.Review.ReviewJson.GetString(JsonObject, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, Spec=none cited, `E6D89B`, PENDING
  - Falsified if: a property holding a number, a boolean or an object is returned as a string instead of null
- `Broiler.Code.Review.ReviewJson.GetInt(JsonObject, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, Spec=none cited, `290BFD`, PENDING
  - Falsified if: a number outside the int range or with a fraction, such as 1.5, is returned as a truncated or wrapped int instead of null
- `Broiler.Code.Review.ReviewJson.GetTimestamp(JsonObject, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, Spec=none cited, `C313DC`, PENDING
  - Falsified if: a timestamp with an explicit offset such as +02:00 is returned without being converted to UTC
- `Broiler.Code.Review.ReviewStateEvaluator` in `src/Broiler.Code.Review/ReviewStateEvaluator.cs` - Security=High, Spec=none cited, `96A140`, PENDING
  - Falsified if: a record with status Reviewed reports Current freshness for text whose normalized form differs from the text it was hashed from
- `Broiler.Code.Review.ReviewStateEvaluator.Evaluate(FileReview?, string?)` in `src/Broiler.Code.Review/ReviewStateEvaluator.cs` - Security=High, Spec=none cited, `9D7EDE`, PENDING
  - Falsified if: a record with status Reviewed evaluated with null content reports IsVerified as true
- `Broiler.Code.Review.ReviewStateEvaluator.FreshnessOf(FileReview, string?)` in `src/Broiler.Code.Review/ReviewStateEvaluator.cs` - Security=High, Spec=none cited, `9ED4E9`, PENDING
  - Falsified if: a record whose reviewedContentHash is absent or carries an unknown algorithm prefix evaluates as Current or Stale instead of Unknown
- `Broiler.Code.Review.ReviewStatus` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, Spec=none cited, `864469`, PENDING
  - Falsified if: the zero member is not Unreviewed, so a status left at its default reads as a recorded decision
- `Broiler.Code.Review.ReviewFreshness` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, Spec=none cited, `5B69A0`, PENDING
  - Falsified if: the zero member is Current, so a Reviewed state whose freshness was left at its default reports IsVerified as true
- `Broiler.Code.Review.ReviewState` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, Spec=none cited, `5F4BB4`, PENDING
  - Falsified if: a state with Status Reviewed and Freshness Stale, Unknown or NotReviewed reports IsVerified as true
- `Broiler.Code.Review.ReviewState.None` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, Spec=none cited, `1109EB`, PENDING
  - Falsified if: ReviewState.None reports IsVerified, IsStaleApproval or IsAttested as true
- `Broiler.Code.Review.ReviewState.IsVerified` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, Spec=none cited, `6E7763`, PENDING
  - Falsified if: a Question, InReview or NeedsChange state with Current freshness reports IsVerified as true
- `Broiler.Code.Review.ReviewState.IsStaleApproval` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, Spec=none cited, `995F9F`, PENDING
  - Falsified if: a Reviewed state with Stale freshness reports IsStaleApproval as false, so the stale check passes a changed file
- `Broiler.Code.Review.ReviewStore` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `A6B16F`, PENDING
  - Falsified if: a record is written for a source path whose .. segments climb out of the workspace, placing the file outside .broiler-review
- `Broiler.Code.Review.ReviewStore.ReviewDirectory` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `13B8F2`, PENDING
  - Falsified if: a record written by WriteAsync lands outside the directory ReadAllAsync walks, so it is never read back
- `Broiler.Code.Review.ReviewStore.RecordSuffix` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `027EB4`, PENDING
  - Falsified if: a record path built by RecordPathFor does not end with the suffix SourcePathFor requires, so ReadAllAsync skips the record
- `Broiler.Code.Review.ReviewStore.RecordPathFor(string)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `88489F`, PENDING
  - Falsified if: a source path whose .. segments climb out of the workspace, or one that normalizes to a path under .broiler-review/, gets a record path instead of null
- `Broiler.Code.Review.ReviewStore.SourcePathFor(string)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `B68AE4`, PENDING
  - Falsified if: a path outside .broiler-review, or one without the .review.json suffix, is given a source path instead of null
- `Broiler.Code.Review.ReviewStore.IsRecordPath(string)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `0BC6D8`, PENDING
  - Falsified if: a path such as .broiler-review2/x.cs, which only begins with the directory name, is reported as a record path
- `Broiler.Code.Review.ReviewStore.ReadAsync(string, CancellationToken)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `0824A1`, PENDING
  - Falsified if: a record file that exists but does not parse is returned as an empty Unreviewed record instead of a failure
- `Broiler.Code.Review.ReviewStore.WriteAsync(FileReview, CancellationToken)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `4F7F3A`, PENDING
  - Falsified if: a record whose path climbs out of the workspace or normalizes under .broiler-review/ is written to storage instead of refused
- `Broiler.Code.Review.ReviewStore.ReadAllAsync(CancellationToken)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `170E96`, PENDING
  - Falsified if: a record that fails to parse is dropped without an entry in Unreadable, so its file reads as unreviewed with no warning
- `Broiler.Code.Review.ReviewStore.CollectAsync(string, Dictionary<string, FileReview>, List<StorageFailure>, CancellationToken)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, Spec=none cited, `7816B9`, PENDING
  - Falsified if: a record in a nested subdirectory of .broiler-review is missing from the result or keyed by its record path instead of its source path
- `Broiler.Code.Windows.CodeHost` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=High, Spec=none cited, `79386E`, PENDING
  - Falsified if: work posted by the analysis or review layers from a worker thread runs on that thread instead of being queued to the window dispatcher and drained on the message-loop thread
- `Broiler.Code.Windows.CodeHost.Run(CodeWindow, string?)` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=High, Spec=none cited, `D85FBC`, PENDING
  - Falsified if: work posted by the analysis or review layers from a worker thread runs on that thread instead of being queued to the window dispatcher and drained on the message-loop thread
- `Broiler.Code.Windows.CodeUiHost` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=Critical, Spec=none cited, `5E5342`, PENDING
  - Falsified if: clipboard text is served from a string held in this process rather than from the Win32 clipboard, so a missing clipboard reads as working
- `Broiler.Code.Windows.CodeUiHost.TryGetText(out string)` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=Critical, Spec=none cited, `B03E24`, PENDING
  - Falsified if: a paste with no clipboard attached returns true or text from an in-process store instead of false and an empty string
- `Broiler.Code.Windows.CodeUiHost.SetText(string)` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=Critical, Spec=none cited, `B5EF0D`, PENDING
  - Falsified if: text set while no clipboard is attached is kept in this process and later returned by TryGetText
- `Broiler.Code.Windows.CodeShellFactory` in `src/Broiler.Code.Windows/CodeShellFactory.cs` - Security=High, Spec=none cited, `F954EC`, PENDING
  - Falsified if: a command-line path that names no existing directory is granted as the workspace root instead of the scratch directory
- `Broiler.Code.Windows.CodeShellFactory.OpenWorkspaceAsync(CodeShell, string?)` in `src/Broiler.Code.Windows/CodeShellFactory.cs` - Security=High, Spec=none cited, `8A2A76`, PENDING
  - Falsified if: a command-line path that names no existing directory is granted as the workspace root instead of the scratch directory
- `Broiler.Code.Windows.CodeShellFactory.ScratchRoot()` in `src/Broiler.Code.Windows/CodeShellFactory.cs` - Security=High, Spec=none cited, `A271B1`, PENDING
  - Falsified if: the scratch root is created somewhere other than LocalApplicationData\Broiler\Code\scratch, such as the temp directory a cleaner can empty
- `Broiler.Code.Windows.CodeWindow` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, Spec=none cited, `1D541C`, PENDING
  - Falsified if: UI work posted from a worker thread runs on a thread other than the one pumping this window's message loop
- `Broiler.Code.Windows.CodeWindow.CodeWindow()` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, Spec=none cited, `8E4DC8`, PENDING
  - Falsified if: the dispatcher is created on a thread other than the one that runs the message loop, so CheckAccess answers for a thread that never drains it
- `Broiler.Code.Windows.CodeWindow.OnNativeWindowMessage(IntPtr, uint, IntPtr, IntPtr)` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, Spec=none cited, `A244C0`, PENDING
  - Falsified if: a committed IME composition is inserted twice, once from the composition message and again from the WM_CHAR the default window procedure generates for the same result string
- `Broiler.Code.Windows.CodeWindow.AttachServices(UiSession, StandardCodeEditor)` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, Spec=none cited, `7EDD26`, PENDING
  - Falsified if: the clipboard or text-input service is bound to a window handle other than this window's NativeHandle, so paste or IME composition acts on another window
- `Broiler.Code.Windows.Program` in `src/Broiler.Code.Windows/Program.cs` - Security=High, Spec=none cited, `A32934`, PENDING
  - Falsified if: an argument beginning with a dash is opened as the workspace directory instead of being treated as an option
- `Broiler.Code.Windows.Program.Main(string[])` in `src/Broiler.Code.Windows/Program.cs` - Security=High, Spec=none cited, `D0FC29`, PENDING
  - Falsified if: an argument beginning with a dash is opened as the workspace directory instead of being treated as an option
- `Broiler.Code.Windows.WindowsFileDialogs` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, Spec=none cited, `10DF41`, PENDING
  - Falsified if: comdlg32 or shell32 is told a buffer holds more characters than it does, so the chosen path is written past the end of the buffer
- `Broiler.Code.Windows.WindowsFileDialogs.MaxPath` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, Spec=none cited, `0D563F`, PENDING
  - Falsified if: the file and path buffers sized from this value hold fewer characters than the length reported with them to comdlg32 and shell32
- `Broiler.Code.Windows.WindowsFileDialogs.RequestOpenAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, Spec=none cited, `CCDE94`, PENDING
  - Falsified if: a name typed into the open dialog for a file that does not exist is returned as a grant
- `Broiler.Code.Windows.WindowsFileDialogs.RequestSaveAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, Spec=none cited, `25872F`, PENDING
  - Falsified if: choosing an existing file in the save dialog returns a grant without the dialog asking whether to overwrite it
- `Broiler.Code.Windows.WindowsFileDialogs.RequestFolderAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, Spec=none cited, `1E281B`, PENDING
  - Falsified if: the grant for a chosen folder is rooted at its parent directory rather than at the folder itself
- `Broiler.Code.Windows.WindowsFileDialogs.Browse(string)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, Spec=none cited, `1C93F2`, PENDING
  - Falsified if: a buffer handed to shell32 holds fewer characters than shell32 is told or assumes it holds, so the display name or the chosen path is written past its end
- `Broiler.Code.Windows.WindowsFileDialogs.Show(FileDialogRequest, bool, int)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, Spec=none cited, `11CBC4`, PENDING
  - Falsified if: a suggested name of 32768 characters or more leaves the file buffer without a terminating NUL when comdlg32 reads it
- `Broiler.Code.Windows.WindowsFileDialogs.BuildFilter(IReadOnlyList<FileDialogFilter>)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, Spec=none cited, `2BE11D`, PENDING
  - Falsified if: the returned filter does not end in two NUL characters, so comdlg32 reads past the end of the array
- `Broiler.Code.Windows.WindowsFileDialogs.OpenFileName` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, Spec=none cited, `025C85`, PENDING
  - Falsified if: the struct size differs from the native OPENFILENAMEW size on the running architecture (152 bytes on x64, 88 on x86), so comdlg32 reads the file buffer and its length from the wrong offsets
- `Broiler.Code.Windows.WindowsFileDialogs.BrowseInfo` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, Spec=none cited, `86B63E`, PENDING
  - Falsified if: a field is out of BROWSEINFOW order or width, so shell32 reads the display-name buffer pointer or the flags from the wrong offset
- `Broiler.Code.Windows.WindowsFileDialogs.SHBrowseForFolder(ref BrowseInfo)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, Spec=none cited, `C657D2`, PENDING
  - Falsified if: the import binds to the ANSI SHBrowseForFolderA, so the UTF-16 title and display-name buffers are read and written as single-byte text
- `Broiler.Code.Windows.WindowsFileDialogs.SHGetPathFromIDListEx(IntPtr, char*, int, int)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, Spec=none cited, `7C322F`, PENDING
  - Falsified if: the length argument reaches shell32 as a value other than the buffer's character count, so the path is written past the end of the buffer
- `Broiler.Code.Windows.WindowsFileDialogs.CoTaskMemFree(IntPtr)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, Spec=none cited, `9CEDB0`, PENDING
  - Falsified if: the import binds to an entry other than ole32 CoTaskMemFree, so the item list is released by an allocator other than the one that created it
- `Broiler.Code.Windows.WindowsFileDialogs.OleInitialize(IntPtr)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, Spec=none cited, `F71911`, PENDING
  - Falsified if: the HRESULT is declared as a type other than the 32-bit int ole32 returns, so S_FALSE from an already-initialised thread reads as a failure
- `Broiler.Code.Windows.WindowsFileDialogs.OleUninitialize()` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, Spec=none cited, `BAD4CD`, PENDING
  - Falsified if: the import binds to CoUninitialize instead of OleUninitialize, leaving OLE initialised on the window thread after every folder dialog
- `Broiler.Code.Windows.WindowsFileDialogs.GetOpenFileName(ref OpenFileName)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, Spec=none cited, `19E8B8`, PENDING
  - Falsified if: the BOOL result is marshalled as a one-byte bool, so a cancelled open dialog reads as a chosen file
- `Broiler.Code.Windows.WindowsFileDialogs.GetSaveFileName(ref OpenFileName)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, Spec=none cited, `4B973B`, PENDING
  - Falsified if: the import binds to GetOpenFileNameW instead of GetSaveFileNameW, so a save offers only existing files and never prompts to overwrite
- `Broiler.Code.Windows.WindowsTextInputService` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=Critical, Spec=none cited, `AB5D86`, PENDING
  - Falsified if: a composition string is read past the end of the unmanaged buffer allocated for it
- `Broiler.Code.Windows.WindowsTextInputService.WmImeStartComposition` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `F569F9`, PENDING
  - Falsified if: the value differs from 0x010D, WM_IME_STARTCOMPOSITION, so the start of a composition is never reported and its characters also arrive as plain text input
- `Broiler.Code.Windows.WindowsTextInputService.WmImeComposition` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `3FF446`, PENDING
  - Falsified if: the value differs from 0x010F, WM_IME_COMPOSITION, so neither the composition nor the result string is ever read
- `Broiler.Code.Windows.WindowsTextInputService.WmImeEndComposition` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `271737`, PENDING
  - Falsified if: the value differs from 0x010E, WM_IME_ENDCOMPOSITION, so a cancelled composition leaves the window suppressing later typed characters
- `Broiler.Code.Windows.WindowsTextInputService.GcsCompStr` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `896D93`, PENDING
  - Falsified if: the value differs from 0x0008, GCS_COMPSTR, so the in-progress composition is read from another composition attribute
- `Broiler.Code.Windows.WindowsTextInputService.GcsResultStr` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `500793`, PENDING
  - Falsified if: the value differs from 0x0800, GCS_RESULTSTR, so a committed string is not recognised and the commit is reported as a cancellation
- `Broiler.Code.Windows.WindowsTextInputService.TryHandleMessage(uint, IntPtr, IntPtr)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `77CB51`, PENDING
  - Falsified if: a WM_IME_COMPOSITION carrying both the result and the composition flags reports the in-progress string instead of the committed one
- `Broiler.Code.Windows.WindowsTextInputService.SetCaretRectangle(BRect)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `4E0935`, PENDING
  - Falsified if: the input context taken by ImmGetContext is not released when ImmSetCompositionWindow fails or throws
- `Broiler.Code.Windows.WindowsTextInputService.TryRead(int, out string)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=Critical, Spec=none cited, `7BAD58`, PENDING
  - Falsified if: when the second ImmGetCompositionStringW call returns fewer bytes than the first, the text returned includes the uninitialised tail of the buffer
- `Broiler.Code.Windows.WindowsTextInputService.Point` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `825C58`, PENDING
  - Falsified if: a field is wider than 32 bits, so the composition form no longer matches its native 28-byte layout and the IME reads the caret position from the wrong offset
- `Broiler.Code.Windows.WindowsTextInputService.Rect` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `96B7E9`, PENDING
  - Falsified if: the fields are not four 32-bit values in left, top, right, bottom order, so the IME reads the caret area from the wrong offsets
- `Broiler.Code.Windows.WindowsTextInputService.CompositionForm` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `7817E6`, PENDING
  - Falsified if: the struct size differs from the native 28-byte COMPOSITIONFORM or its fields are out of style, position, area order, so the IME reads past it or from the wrong offsets
- `Broiler.Code.Windows.WindowsTextInputService.ImmGetContext(IntPtr)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `BF4351`, PENDING
  - Falsified if: the window parameter or the returned input-context handle is declared narrower than IntPtr, so a 64-bit handle is truncated
- `Broiler.Code.Windows.WindowsTextInputService.ImmReleaseContext(IntPtr, IntPtr)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `D89C7D`, PENDING
  - Falsified if: the import binds to an entry other than ImmReleaseContext, so every composition read leaks the input context it took
- `Broiler.Code.Windows.WindowsTextInputService.ImmGetCompositionStringW(IntPtr, int, IntPtr, int)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `AF6C17`, PENDING
  - Falsified if: the import binds to the ANSI ImmGetCompositionStringA, so the byte count and the text are not UTF-16 and the composition is decoded wrongly
- `Broiler.Code.Windows.WindowsTextInputService.ImmSetCompositionWindow(IntPtr, ref CompositionForm)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, Spec=none cited, `3A7217`, PENDING
  - Falsified if: the composition form is passed by value instead of by pointer, so the IME reads the form from an arbitrary address
- `Broiler.Code.Workspaces.CodeWorkspace` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `85D810`, PENDING
  - Falsified if: an operation on a document opened through a file-dialog grant reaches the file at the same relative path under the workspace root instead of the granted file
- `Broiler.Code.Workspaces.CodeWorkspace.CodeWorkspace(IWorkspaceStorage, WorkspaceIdFactory?)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `AFFEC6`, PENDING
  - Falsified if: on a storage provider that reports CaseInsensitivePaths, 'Src/A.cs' and 'src/a.cs' are registered as two items over one file
- `Broiler.Code.Workspaces.CodeWorkspace.StorageFor(WorkspaceItemId)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `A07458`, PENDING
  - Falsified if: a document opened through a file-dialog grant resolves to the workspace's own storage, so its reads and saves use the same relative path under the workspace root
- `Broiler.Code.Workspaces.CodeWorkspace.AddItem(string, WorkspaceItemKind, WorkspaceItemId, bool, string?)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `6CF57B`, PENDING
  - Falsified if: a Compile Include that climbs above the workspace root with '..' is registered with a usable ID instead of being refused with BRW0001
- `Broiler.Code.Workspaces.CodeWorkspace.OpenDocumentAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `A611DA`, PENDING
  - Falsified if: opening an item that is already open returns a new SourceDocument with a second buffer instead of the open instance
- `Broiler.Code.Workspaces.CodeWorkspace.OpenGrantedDocumentAsync(IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `50FFC1`, PENDING
  - Falsified if: a path that climbs out of the granted directory with '..' is read through the grant instead of failing with OutsideGrant
- `Broiler.Code.Workspaces.CodeWorkspace.SaveDocumentAsAsync(WorkspaceItemId, IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `E518E5`, PENDING
  - Falsified if: a Save As onto a path whose key another open document already holds re-points that key to this document without refusing, leaving two open documents over one file
- `Broiler.Code.Workspaces.CodeWorkspace.SaveDocumentAsync(WorkspaceItemId, bool, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `FA6A8B`, PENDING
  - Falsified if: an edit accepted while the storage write is still pending is marked saved, so the document reports clean although the file lacks that edit
- `Broiler.Code.Workspaces.CodeWorkspace.SaveAllAsync(CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `59626B`, PENDING
  - Falsified if: a document whose save failed or conflicted stops Save All, so dirty documents after it are neither attempted nor reported
- `Broiler.Code.Workspaces.CodeWorkspace.RenameItemAsync(WorkspaceItemId, string, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `806AE0`, PENDING
  - Falsified if: renaming a document opened through a file-dialog grant renames the file at the same relative path under the workspace root instead of the granted file
- `Broiler.Code.Workspaces.CodeWorkspace.HasExternalChangeAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `076D96`, PENDING
  - Falsified if: for a document opened through a file-dialog grant the revision is read from the same relative path under the workspace root instead of from the granted file
- `Broiler.Code.Workspaces.CodeWorkspace.KeyFor(WorkspaceItemId, string)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `EA8EF3`, PENDING
  - Falsified if: an item opened through a grant is keyed on its bare relative path, so it collides with the workspace item at the same relative path
- `Broiler.Code.Workspaces.CodeWorkspace.GrantKey(IWorkspaceStorage, string)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, Spec=none cited, `322DBF`, PENDING
  - Falsified if: two grants whose storages list different first roots produce the same key for one relative path, so opening the second returns the first's open document
- `Broiler.Code.Workspaces.Model.WorkspaceItemId` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, Spec=none cited, `E62C7B`, PENDING
  - Falsified if: two items minted by one factory compare equal, so StorageFor returns one item's grant for the other
- `Broiler.Code.Workspaces.Model.WorkspaceItemId.IsNone` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, Spec=none cited, `5E896F`, PENDING
  - Falsified if: IsNone is false for WorkspaceItemId.None, so OpenGrantedDocumentAsync gives every new granted document the ID 0 and one grant replaces another
- `Broiler.Code.Workspaces.Model.WorkspaceIdFactory` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, Spec=none cited, `481E1F`, PENDING
  - Falsified if: two calls to Next on one factory from different threads return the same ID
- `Broiler.Code.Workspaces.Model.WorkspaceIdFactory.WorkspaceIdFactory(long)` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, Spec=none cited, `F3F3D9`, PENDING
  - Falsified if: a factory created with a positive highestSeen N returns N itself or a lower positive value from its first Next
- `Broiler.Code.Workspaces.Model.WorkspaceIdFactory.Next()` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, Spec=none cited, `FDFD03`, PENDING
  - Falsified if: Next returns a value it already returned when called from two threads at once
- `Broiler.Code.Workspaces.Projects.DeclaredProjectFile` in `src/Broiler.Code.Workspaces/Projects/DeclaredProjectFile.cs` - Security=High, Spec=none cited, `B06DBD`, PENDING
  - Falsified if: a project file whose DOCTYPE declares an external entity gets the contents of the file or URL it names substituted into the parsed values
- `Broiler.Code.Workspaces.Projects.DeclaredProjectFile.Parse(string, string)` in `src/Broiler.Code.Workspaces/Projects/DeclaredProjectFile.cs` - Security=High, Spec=none cited, `F5EDED`, PENDING
  - Falsified if: a project file whose internal DTD expands its entities past ten million characters is parsed instead of failing with an XmlException
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, Spec=none cited, `2D6A28`, PENDING
  - Falsified if: a classic .sln made of many lines that begin with Project("{ and never close the brace takes time that grows with the square of its size to parse
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile.SlnProjectPattern` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, Spec=none cited, `BD6E9B`, PENDING
  - Falsified if: a match attempt that starts on one line scans on through every later line, so a file of many unterminated Project lines costs time quadratic in its size
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile.Parse(string, string)` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, Spec=none cited, `ADAC76`, PENDING
  - Falsified if: a file whose name ends in .slnx in any letter case is handed to the classic .sln matcher, so its projects are silently dropped
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile.ParseSlnx(string, string)` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, Spec=none cited, `24B745`, PENDING
  - Falsified if: a .slnx whose internal DTD nests entity references expands past ten million characters instead of failing with an XmlException
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile.ParseSln(string, string)` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, Spec=none cited, `BD371B`, PENDING
  - Falsified if: a classic .sln made of many lines that begin with Project("{ and never close the brace takes time that grows with the square of its size to parse
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, Spec=none cited, `AD192A`, PENDING
  - Falsified if: two overlapping RecordAsync calls for one document share the same .tmp file, so the entry left on disk holds the older text or interleaved bytes that no longer parse
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.Options` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, Spec=none cited, `3066F2`, PENDING
  - Falsified if: an entry written with these options does not read back with the same id, path, text and revision
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.ForWorkspace(string, string)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, Spec=none cited, `29B579`, PENDING
  - Falsified if: two workspace roots that differ only in letter case, which are different directories on a case-sensitive file system, get the same journal directory and overwrite each other's entries
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.RecordAsync(WorkspaceItemId, WorkspaceItem, string, CancellationToken)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, Spec=none cited, `34781E`, PENDING
  - Falsified if: two overlapping RecordAsync calls for one document share the same .tmp file, so the entry left on disk holds the older text or interleaved bytes that no longer parse
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.Forget(WorkspaceItemId)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, Spec=none cited, `3BE34A`, PENDING
  - Falsified if: Forget removes the entry file of a different document, or leaves the entry of its own document on disk
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.Clear()` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, Spec=none cited, `A248D3`, PENDING
  - Falsified if: Clear deletes files outside the journal directory, for example by following a link placed inside it
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.ReadAllAsync(CancellationToken)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, Spec=none cited, `63FA80`, PENDING
  - Falsified if: one entry file that cannot be opened for lack of permission makes ReadAllAsync throw instead of skipping it and returning the other entries
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.EntryPath(WorkspaceItemId)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, Spec=none cited, `66DC4A`, PENDING
  - Falsified if: two different ids map to the same entry file, or an id maps to a file outside the journal directory
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `EAF628`, PENDING
  - Falsified if: a relative path that names or passes through a symbolic link inside the granted root reads or writes a file outside that root
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.FileSystemWorkspaceStorage(string)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `914CC6`, PENDING
  - Falsified if: on a case-sensitive volume under macOS or Windows the provider still reports CaseInsensitivePaths, so renaming a.cs onto an existing A.cs overwrites A.cs
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.ReadTextAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `0D3BDD`, PENDING
  - Falsified if: a relative path through a symbolic link inside the granted root returns the text of a file outside it
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.WriteTextAsync(string, string, TextEncodingInfo, string?, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `0DA4C9`, PENDING
  - Falsified if: a symbolic link already present at the file's .broiler-tmp sibling makes a save write the new text to the link's target outside the granted root
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.ListAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `EC7076`, PENDING
  - Falsified if: an entry that is a symbolic link or junction appears in the listing
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.StatAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `5E493B`, PENDING
  - Falsified if: a directory is reported with a non-zero size or a revision, or a missing path is reported as found
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.DeleteAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `C04576`, PENDING
  - Falsified if: deleting a directory that contains a symbolic link or junction removes files under the link's target outside the granted root
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.RenameAsync(string, string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `C594C6`, PENDING
  - Falsified if: a rename onto an existing entry whose name differs by more than letter case replaces it instead of failing with Conflict
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.TryResolve(string, out string, out StorageFailure?)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `D812AE`, PENDING
  - Falsified if: a relative path that names or passes through a symbolic link or junction inside the granted root resolves to a location outside it and is not refused
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.Decode(byte[])` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, Spec=none cited, `C81C03`, PENDING
  - Falsified if: a file that is not valid UTF-8, such as Windows-1252 text, is returned with U+FFFD in place of its bytes and reported as utf-8, so the next save rewrites it
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, Spec=none cited, `DB7ACF`, PENDING
  - Falsified if: an implementation reads, writes, lists or deletes an entry outside GrantedRoots for a path with .. segments or one that names a link
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.ReadTextAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, Spec=none cited, `CD76AB`, PENDING
  - Falsified if: an implementation returns the text of an entry outside the granted roots for a path with .. segments or one that names a link
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.WriteTextAsync(string, string, Model.TextEncodingInfo, string?, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, Spec=none cited, `7949B7`, PENDING
  - Falsified if: a write whose expectedRevision differs from the entry's current revision replaces the entry instead of failing with Conflict
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.ListAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, Spec=none cited, `A8D97A`, PENDING
  - Falsified if: an implementation lists an entry that lies outside the granted roots, such as the target of a link
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.StatAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, Spec=none cited, `542CE2`, PENDING
  - Falsified if: an implementation reports the size or revision of an entry outside the granted roots instead of failing with OutsideGrant
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.DeleteAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, Spec=none cited, `944079`, PENDING
  - Falsified if: an implementation removes an entry outside the granted roots, such as files under a link inside a deleted directory
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.RenameAsync(string, string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, Spec=none cited, `346EC9`, PENDING
  - Falsified if: a provider without the Rename capability moves the entry instead of failing with Unsupported
- `Broiler.Code.Workspaces.Storage.WorkspacePath` in `src/Broiler.Code.Workspaces/Storage/WorkspacePath.cs` - Security=High, Spec=none cited, `813AEB`, PENDING
  - Falsified if: a relative path whose .. segments climb above its start, such as src/../../outside, is returned as a usable path instead of null
- `Broiler.Code.Workspaces.Storage.WorkspacePath.Normalize(string)` in `src/Broiler.Code.Workspaces/Storage/WorkspacePath.cs` - Security=High, Spec=none cited, `A4BCB6`, PENDING
  - Falsified if: a relative path whose .. segments climb above its start, such as src/../../outside, is returned as a usable path instead of null
- `Broiler.Code.Workspaces.Storage.WorkspacePath.IsContained(string, string)` in `src/Broiler.Code.Workspaces/Storage/WorkspacePath.cs` - Security=High, Spec=none cited, `6C5A15`, PENDING
  - Falsified if: a candidate that only shares a name prefix with the root, such as src2/a against root src, is reported as contained
- `Broiler.Code.Workspaces.Storage.WorkspacePath.IsReservedDeviceName(string)` in `src/Broiler.Code.Workspaces/Storage/WorkspacePath.cs` - Security=High, Spec=none cited, `6786A8`, PENDING
  - Falsified if: a segment of COM or LPT followed by a superscript digit U+00B9, U+00B2 or U+00B3, which Windows also reserves as a device name, is not recognized as reserved
- `Broiler.Code.Workspaces.WorkspaceLoader` in `src/Broiler.Code.Workspaces/WorkspaceLoader.cs` - Security=High, Spec=none cited, `8D9C1E`, PENDING
  - Falsified if: a project path in the solution that climbs above the granted root with '..' is read through storage instead of being reported as a BRW0200 diagnostic
- `Broiler.Code.Workspaces.WorkspaceLoader.LoadSolutionAsync(IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Workspaces/WorkspaceLoader.cs` - Security=High, Spec=none cited, `C660AC`, PENDING
  - Falsified if: a solution file that is not well-formed XML escapes the call as an exception instead of returning a failed StorageResult
- `Broiler.Code.Workspaces.WorkspaceLoader.LoadProjectAsync(CodeWorkspace, IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Workspaces/WorkspaceLoader.cs` - Security=High, Spec=none cited, `C68D04`, PENDING
  - Falsified if: a Compile Include that climbs above the granted root with '..' becomes a workspace item with a real id instead of a BRW0001 diagnostic
- `Broiler.Code.Workspaces.WorkspaceLoader.Combine(string, string)` in `src/Broiler.Code.Workspaces/WorkspaceLoader.cs` - Security=High, Spec=none cited, `0F7D4D`, PENDING
  - Falsified if: an include of '../../outside.cs' from a project one level below the root comes back as a path inside the root instead of the un-normalized escape

## 10. What This Record Does Not Say

It is not an approval of the component, and a full table above would not be one either. It
records which declarations somebody stated a decision about, and against which version of
each. It does not record what they read, how long they spent, or whether they were right.

A fingerprint is six hex characters of SHA-256 over a declaration's token texts. It answers
whether a unit changed since a decision was recorded against it. It is not a collision-free
identifier across units and it is not a cryptographic commitment, so it detects a change and
does not resist a forger with commit access.

An assessment is a comment, so changing one moves no fingerprint anywhere, and nothing
mechanical checks that it is right; the check holds its values to their vocabularies and no
further.

1472 of the 1472 assessed units declare `Origin=AI`. Reading a declaration is the only thing
that makes it read.
