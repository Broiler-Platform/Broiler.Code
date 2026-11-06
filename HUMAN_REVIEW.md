# Human Review: Broiler.Code

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`dotnet tool run hairs -- assurance generate --root .`, which rewrites this file,
`CODE-ASSURANCE.md`, `hairs.manifest.json` and every generated source header from the
product tree.

> **Status: PENDING.** Human-reviewed: 0 of 799 relevant units. `dotnet tool run hairs -- assurance check --root . --release`
> fails while any relevant unit is without a decision bound to its current fingerprint.

## 1. How To Use This File

Read it; do not edit it. A decision about a code unit is the `// Hairs-Human:` line on that
unit's declaration, and every table below is read out of those lines. There is nothing here
to fill in and nothing here to leave blank.

## 2. How A Review Is Recorded

In one place: the `// Hairs-Human:` line of the assurance annotation that sits on the
declaration being read. Nothing in this file is edited by hand, no second document carries a
per-item checklist, and no list of permitted aliases exists to be added to.

```csharp
// Hairs-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=4A3BFD
// Hairs-Falsified-If: a negative value reaches the running total
// Hairs-Human:        PENDING
```

The last line has four shapes. A human writes three of them; the generator writes the fourth
and may never invent an alias, which the check asserts in both directions.

| Line | Meaning |
|---|---|
| `PENDING` | Nobody has recorded a decision for this unit. The generator leaves it exactly as it stands. |
| `<alias>` | A human states their own alias and leaves the machine field to the generator, which fills it with the declaration's fingerprint at the next run. |
| `<alias>; Fingerprint=<six hex>` | A decision bound to one exact version of one declaration. |
| `STALE; Previous=<alias>@<fingerprint>` | Written by the generator when the code moved after a decision. Only a human clears it, by stating their alias again. |

A human may state their own `IP=`, `Security=`, `Resources=`, `CS=` and `CL=` assessment beside their alias,
which is how a reader disagrees with the machine assessment on the line above: an assessment is
a comment and moves no fingerprint, so there is nowhere else to say it.

**No branch, commit or tag is recorded in this file.** Each decision names the fingerprint of
the declaration it was made against, and the state machine compares that value with the
declaration as it now stands. A commit says a tree moved; a fingerprint says whether this unit
did, which is the narrower and the more useful of the two.

## 3. Summary

| Metric | Value |
|---|---:|
| Files scanned | 54 |
| Code units | 1243 |
| Relevant | 799 |
| Exempt | 444 |
| Assessed | 799 of 799 (100%) |
| Human reviewed | 0 of 799 (0%) |
| Unverified | 799 |
| Aliases naming a decision | 0 |

## 4. Review States

One row per state of the machine that reads the two lines. The states are computed from the
annotations and the current fingerprints; nothing stores them.

| State | Units |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 799 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 444 |

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
| `src/Broiler.Code.Core/Review/ReviewController.cs` | 55 | 35 | 20 | 35 | Low | High | 28/19 |
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
| `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` | 13 | 10 | 3 | 10 | Low | High | 9/4 |
| `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` | 16 | 11 | 5 | 11 | Low | Critical | 11/7 |
| `src/Broiler.Code.Language.CSharp.Roslyn/EvaluatedProjectGraph.cs` | 32 | 17 | 15 | 17 | Low | High | 15/2 |
| `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` | 15 | 15 | 0 | 15 | Low | High | 15/9 |
| `src/Broiler.Code.Language.CSharp.Syntax/KeywordTable.cs` | 7 | 7 | 0 | 7 | None | Medium | 7/0 |
| `src/Broiler.Code.Language.CSharp.Syntax/LineState.cs` | 8 | 3 | 5 | 3 | None | Medium | 2/0 |
| `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` | 16 | 12 | 4 | 12 | Low | High | 11/5 |
| `src/Broiler.Code.Linux/CodeHost.cs` | 4 | 4 | 0 | 4 | Low | High | 4/2 |
| `src/Broiler.Code.Linux/CodeShellFactory.cs` | 4 | 4 | 0 | 4 | Low | High | 4/3 |
| `src/Broiler.Code.Linux/CodeWindow.cs` | 33 | 18 | 15 | 18 | Low | High | 13/6 |
| `src/Broiler.Code.Linux/LinuxFileDialogs.cs` | 20 | 15 | 5 | 15 | Low | High | 10/10 |
| `src/Broiler.Code.Linux/Program.cs` | 2 | 2 | 0 | 2 | Low | High | 2/2 |
| `src/Broiler.Code.Windows/CodeHost.cs` | 13 | 12 | 1 | 12 | Low | Critical | 7/5 |
| `src/Broiler.Code.Windows/CodeShellFactory.cs` | 4 | 4 | 0 | 4 | Low | High | 4/3 |
| `src/Broiler.Code.Windows/CodeWindow.cs` | 30 | 19 | 11 | 19 | Low | High | 18/5 |
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
- `Broiler.Code.Core.Review.ReviewController.LoadAsync(CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `709745`, PENDING
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
- `Broiler.Code.Core.Review.ReviewController.CommitAsync(FileReview, string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `656A69`, PENDING
  - Falsified if: the review maps are written and Changed is raised on a thread-pool thread when a decision is recorded with a revision lookup that completes asynchronously
- `Broiler.Code.Core.Review.ReviewController.Target()` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `77FAB9`, PENDING
  - Falsified if: a target is returned for a selected item that has no open document
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `AF689C`, PENDING
  - Falsified if: a storage operation forwarded through this adapter drops a failure kind or message returned by the underlying workspace storage
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.ReadTextAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `E23C97`, PENDING
  - Falsified if: a failed read from the workspace storage is returned as Succeeded or with a different failure kind
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.WriteTextAsync(string, string, Hairs.Storage.TextEncodingInfo, string?, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `572E2F`, PENDING
  - Falsified if: a write passes an encoding or expected revision different from the caller's, or reports a failed workspace write as Succeeded
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.ListAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `6EF31E`, PENDING
  - Falsified if: an entry returned by the workspace storage is omitted or has its IsDirectory flag inverted
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.StatAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `22E3A2`, PENDING
  - Falsified if: a failed stat from the workspace storage is returned as Succeeded
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.DeleteAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `2E064A`, PENDING
  - Falsified if: a failed delete from the workspace storage is returned as Succeeded
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.RenameAsync(string, string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, Spec=none cited, `E52CB6`, PENDING
  - Falsified if: a failed rename from the workspace storage is returned as Succeeded
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
- `Broiler.Code.Linux.CodeWindow.CodeWindow(BSize, Action<string>, bool, int)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `E750A2`, PENDING
  - Falsified if: a pollMilliseconds of zero or less reaches PeriodicTimer as a period below one millisecond
- `Broiler.Code.Linux.CodeWindow.TryGetText(out string)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `B03E24`, PENDING
  - Falsified if: with no X11 clipboard open, TryGetText returns true or a text other than the empty string
- `Broiler.Code.Linux.CodeWindow.SetText(string)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `B5EF0D`, PENDING
  - Falsified if: on a host with no X11 clipboard, SetText throws instead of dropping the text
- `Broiler.Code.Linux.CodeWindow.RunAsync(CancellationToken)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `BC8B25`, PENDING
  - Falsified if: after WaitForNextTickAsync resumes on a thread-pool thread, the next Drain runs off the thread that constructed the dispatcher and throws InvalidOperationException
- `Broiler.Code.Linux.CodeWindow.DisposeAsync()` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, Spec=none cited, `3A89A2`, PENDING
  - Falsified if: a second DisposeAsync disposes the X11 surface or the clipboard connection again
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
- `Broiler.Code.Windows.CodeWindow.OnNativeWindowMessage(IntPtr, uint, IntPtr, IntPtr)` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, Spec=none cited, `810587`, PENDING
  - Falsified if: a committed IME composition is inserted twice, once from the composition message and again from the WM_CHAR the default window procedure generates for the same result string
- `Broiler.Code.Windows.CodeWindow.AttachServices(UiSession, StandardCodeEditor)` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, Spec=none cited, `7EDD26`, PENDING
  - Falsified if: the clipboard or text-input service is bound to a window handle other than this window's NativeHandle, so paste or IME composition acts on another window
- `Broiler.Code.Windows.CodeWindow.SetFocus(IntPtr)` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, Spec=none cited, `66F237`, PENDING
  - Falsified if: the import binds to an entry other than SetFocus in user32, so focus is not transferred to the render window
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

799 of the 799 assessed units declare `Origin=AI`. Reading a declaration is the only thing
that makes it read.
