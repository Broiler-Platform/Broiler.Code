# Broiler.Code Code Assurance

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`dotnet tool run hairs -- assurance generate --root .`, which rewrites this file,
`HUMAN_REVIEW.md`, `hairs.manifest.json` and every generated source header from the
product tree.

**No code unit in this component carries a decision on its human line yet.** This report
records that absence precisely. It is not a claim that the code is reviewed, assured or safe,
and the figures below are the measurement of how far from that claim the per-unit record is.

## Summary

| Metric | Value |
|---|---:|
| Files scanned | 54 |
| Files not covered | 0 |
| Files carrying an annotation | 54 |
| Code units | 1243 |
| Relevant | 799 |
| Exempt by predicate | 444 |
| Annotated | 799 of 799 (100%) |
| Human reviewed | 0 of 799 (0%) |
| Unverified | 799 |

## Review states

| State | Count |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 799 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 444 |

## IP risk

| Value | Units |
|---|---:|
| None | 311 |
| Low | 488 |
| Medium | 0 |
| High | 0 |
| Unknown | 0 |
| *not annotated* | 0 |

## Security risk

| Value | Units |
|---|---:|
| None | 45 |
| Low | 235 |
| Medium | 249 |
| High | 255 |
| Critical | 15 |
| *not annotated* | 0 |

## Resource impact

| Metric | Value |
|---|---:|
| Maximum | 9 / 10 |
| Average over annotated units | 2.2 / 10 |
| Units scored | 799 |

## High-security review areas

- `Broiler.Code.Core.CodeAnalysisController` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.CodeAnalysisController.CodeAnalysisController(UiCodeEditor, SourceBufferDocument, ICodeClassifier, IUiDispatcher, IAnalysisScheduler?, int)` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.CodeAnalysisController.Dispose()` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.CodeAnalysisController.Refresh()` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.CodeAnalysisController.Start(ICodeTextSnapshot, CodeTextChange?)` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.CodeAnalysisController.RunInBackground(long, CodeClassificationResult?, ICodeTextSnapshot, CodeTextChange?, CancellationToken)` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.CodeAnalysisController.Publish(long, CodeClassificationResult)` in `src/Broiler.Code.Core/CodeAnalysisController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.DesktopInputRouter` in `src/Broiler.Code.Core/Hosting/DesktopInputRouter.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.DesktopInputRouter.Header(InputDeviceId)` in `src/Broiler.Code.Core/Hosting/DesktopInputRouter.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.EvdevInputRouter` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.EvdevInputRouter.PointerPosition` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.EvdevInputRouter.SetViewport(BSize)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.EvdevInputRouter.SetAbsolutePointer(double, double, InputDeviceId, long)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.EvdevInputRouter.FromMouseMove(MouseMoveEvent)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.EvdevInputRouter.FromMouseButton(MouseButtonEvent)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.EvdevInputRouter.FromWheel(MouseWheelEvent)` in `src/Broiler.Code.Core/Hosting/EvdevInputRouter.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.UiThreadDispatcher` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.UiThreadDispatcher(Action?)` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.PendingCount` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.CheckAccess()` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.Post(Action)` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Hosting.UiThreadDispatcher.Drain()` in `src/Broiler.Code.Core/Hosting/UiThreadDispatcher.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.SetCurrentDocument(WorkspaceItemId)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.SetCaretLine(int)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.Refresh()` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.OnBufferChanged(TextSnapshot, TextChange)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.Approve()` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.Withdraw()` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.Act(Func<AssuranceDocument, AssuranceUnit, string, AssuranceEditResult>)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.Apply(string)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.Narrow(string, string)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.Rebuild()` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.AssuranceController.ScannerFor(WorkspaceItemId)` in `src/Broiler.Code.Core/Review/AssuranceController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.GitCommand` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.GitCommand.Timeout` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.GitCommand.RunAsync(string, string, CancellationToken)` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.GitIdentity` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.GitIdentity.ResolveReviewerAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/GitCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.GitRevisionProvider` in `src/Broiler.Code.Core/Review/GitRevisionProvider.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.GitRevisionProvider.GetCurrentRevisionAsync(CancellationToken)` in `src/Broiler.Code.Core/Review/GitRevisionProvider.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.LoadAsync(CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.Publish(Dictionary<string, FileReview>, Dictionary<string, ReviewState>, IReadOnlyList<StorageFailure>, long)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.ContentOfAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.SetCurrentDocument(WorkspaceItemId)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.ReviewFor(string)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.RecordDecisionAsync(ReviewStatus, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.AddNoteAsync(ReviewNoteKind, string, int, int, string?, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.ResolveNoteAsync(string, string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.RemoveNoteAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.CommitAsync(FileReview, string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.Target()` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.ReadTextAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.WriteTextAsync(string, string, Hairs.Storage.TextEncodingInfo, string?, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.ListAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.StatAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.DeleteAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Review.ReviewController.WorkspaceHairsStorage.RenameAsync(string, string, CancellationToken)` in `src/Broiler.Code.Core/Review/ReviewController.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.ReviewStatusItems` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.ReviewUnitItems` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.AttachWorkspace(CodeWorkspace, Workspaces.Recovery.RecoveryJournal?)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.AttachReview(CodeWorkspace)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.LoadReviewAsync(ReviewController, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.Reviewer` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.InvokeAsync(string, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.NewProjectAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OpenAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OpenFolderAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.ApplyRevisionProviderFor(FileGrant)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.SaveActiveAsAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OpenSolutionAsync(FileGrant, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OpenDocumentAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.RefreshCommands()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.SyncReviewState()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.SyncReviewUnitInput()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OnReviewUnitSelected(object?, UiComboBoxSelectionChangedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.SyncReviewStatusInput()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OnReviewStatusSelected(object?, UiComboBoxSelectionChangedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.RecordPickedStatusAsync(string)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OnToolbarButtonClicked(object?, UiButtonClickEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OnMenuItemInvoked(object?, UiMenuItemInvokedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OnExplorerNodeActivated(object?, TreeNodeEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OnProblemActivated(object?, TreeNodeEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OnReviewChanged(object?, EventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OnEditorSelectionChanged(object?, CodeSelectionChangedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.OnReviewNoteSubmitted(object?, UiEditSubmittedEventArgs)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.AddNoteFromInputAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.SignUnit(bool)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.RecordReviewAsync(ReviewStatus, CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.NavigateAsync(WorkspaceItemId, ProblemEntry)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.SaveActiveAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.SaveAllAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.CodeShell.DetachWorkspace()` in `src/Broiler.Code.Core/Shell/CodeShell.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.DocumentCoordinator` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.DocumentCoordinator.OpenAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.DocumentCoordinator.CloseAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.DocumentCoordinator.CloseAllAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.DocumentCoordinator.SaveAllAsync(CancellationToken)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.DocumentCoordinator.OnCloseRequested(object?, UiTabCloseRequestedEventArgs)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.DocumentCoordinator.OnBufferChanged(WorkspaceItemId)` in `src/Broiler.Code.Core/Shell/DocumentCoordinator.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.IFileDialogService` in `src/Broiler.Code.Core/Shell/FileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.IFileDialogService.RequestOpenAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Core/Shell/FileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.IFileDialogService.RequestSaveAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Core/Shell/FileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.IFileDialogService.RequestFolderAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Core/Shell/FileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.WorkspaceBootstrap` in `src/Broiler.Code.Core/Shell/WorkspaceBootstrap.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.WorkspaceBootstrap.OpenAsync(CodeShell, IWorkspaceStorage, CancellationToken)` in `src/Broiler.Code.Core/Shell/WorkspaceBootstrap.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.WorkspaceBootstrap.FindSolutionAsync(IWorkspaceStorage, CancellationToken)` in `src/Broiler.Code.Core/Shell/WorkspaceBootstrap.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Shell.WorkspaceBootstrap.AddSourcesAsync(CodeWorkspace, IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Core/Shell/WorkspaceBootstrap.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Templates.CodeTemplateService` in `src/Broiler.Code.Core/Templates/CodeTemplateService.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Templates.CodeTemplateService.WriteAsync(TemplateResult, CancellationToken)` in `src/Broiler.Code.Core/Templates/CodeTemplateService.cs` - Security=High, human line PENDING
- `Broiler.Code.Core.Templates.CodeTemplateService.AddProjectReferenceAsync(string, string, CancellationToken)` in `src/Broiler.Code.Core/Templates/CodeTemplateService.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.CSharpLanguageService` in `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.CSharpLanguageService.Analyze(ICodeTextSnapshot, string, string, string?, IReadOnlyDictionary<string, string>?, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.CSharpLanguageService.ResolveReferences(EvaluatedProjectGraph, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.CSharpLanguageService.ReadOverlayOrFile(string, IReadOnlyDictionary<string, string>?)` in `src/Broiler.Code.Language.CSharp.Roslyn/CSharpLanguageService.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.WorkspaceTrust` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.DesignTimeEvaluator(string)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.EvaluateAsync(string, string?, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.Parse(string, string?, string)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.IsInsideWorkspace(string)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.DesignTimeEvaluator.TryKill(Process)` in `src/Broiler.Code.Language.CSharp.Roslyn/DesignTimeEvaluator.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.UntrustedGraphSource` in `src/Broiler.Code.Language.CSharp.Roslyn/EvaluatedProjectGraph.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Roslyn.UntrustedGraphSource.TryGetGraph(string, string?, out EvaluatedProjectGraph?, out GraphUnavailable?)` in `src/Broiler.Code.Language.CSharp.Roslyn/EvaluatedProjectGraph.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.Lex(ReadOnlySpan<char>, LineState, List<CodeClassificationSpan>)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.LexDirective(ReadOnlySpan<char>, int, List<CodeClassificationSpan>)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.LexLiteral(ReadOnlySpan<char>, int, List<CodeClassificationSpan>, out LineState, out bool)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.LexCharacter(ReadOnlySpan<char>, int, List<CodeClassificationSpan>)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.ContinueVerbatimString(ReadOnlySpan<char>, int, byte, List<CodeClassificationSpan>, out bool)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.ContinueRawString(ReadOnlySpan<char>, int, byte, byte, List<CodeClassificationSpan>, out bool)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.ScanNumber(ReadOnlySpan<char>, int)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.CSharpLineLexer.IndexOfBlockCommentEnd(ReadOnlySpan<char>)` in `src/Broiler.Code.Language.CSharp.Syntax/CSharpLineLexer.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier.Classify(ICodeTextSnapshot, CodeClassificationResult?, CodeTextChange?, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier.Full(ICodeTextSnapshot, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier.Incremental(ICodeTextSnapshot, Result, CodeTextChange, CancellationToken)` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Syntax.PortableCSharpClassifier.ClassifyLine(ICodeTextSnapshot, int, LineState, List<CodeClassificationSpan>, ref LineReader, out LineState)` in `src/Broiler.Code.Language.CSharp.Syntax/PortableCSharpClassifier.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeHost` in `src/Broiler.Code.Linux/CodeHost.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeHost.RunAsync(string?, bool, CancellationToken)` in `src/Broiler.Code.Linux/CodeHost.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeShellFactory` in `src/Broiler.Code.Linux/CodeShellFactory.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeShellFactory.OpenWorkspaceAsync(CodeShell, string?, CancellationToken)` in `src/Broiler.Code.Linux/CodeShellFactory.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeShellFactory.ScratchRoot()` in `src/Broiler.Code.Linux/CodeShellFactory.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeWindow` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeWindow.CodeWindow(BSize, Action<string>, bool, int)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeWindow.TryGetText(out string)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeWindow.SetText(string)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeWindow.RunAsync(CancellationToken)` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.CodeWindow.DisposeAsync()` in `src/Broiler.Code.Linux/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs.LinuxFileDialogs()` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs.RequestOpenAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs.RequestSaveAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs.RequestFolderAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs.RunAsync(FileDialogRequest, DialogMode, CancellationToken)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs.Arguments(string, FileDialogRequest, DialogMode)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs.FolderGrant(string?)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs.Grant(string?)` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxFileDialogs.FindHelper()` in `src/Broiler.Code.Linux/LinuxFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.Program` in `src/Broiler.Code.Linux/Program.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.Program.Main(string[])` in `src/Broiler.Code.Linux/Program.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeHost` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeHost.Run(CodeWindow, string?)` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeUiHost` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.CodeUiHost.TryGetText(out string)` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.CodeUiHost.SetText(string)` in `src/Broiler.Code.Windows/CodeHost.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.CodeShellFactory` in `src/Broiler.Code.Windows/CodeShellFactory.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeShellFactory.OpenWorkspaceAsync(CodeShell, string?)` in `src/Broiler.Code.Windows/CodeShellFactory.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeShellFactory.ScratchRoot()` in `src/Broiler.Code.Windows/CodeShellFactory.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeWindow` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeWindow.CodeWindow()` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeWindow.OnNativeWindowMessage(IntPtr, uint, IntPtr, IntPtr)` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeWindow.AttachServices(UiSession, StandardCodeEditor)` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.CodeWindow.SetFocus(IntPtr)` in `src/Broiler.Code.Windows/CodeWindow.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.Program` in `src/Broiler.Code.Windows/Program.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.Program.Main(string[])` in `src/Broiler.Code.Windows/Program.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.MaxPath` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.RequestOpenAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.RequestSaveAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.RequestFolderAsync(FileDialogRequest, CancellationToken)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.Browse(string)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.Show(FileDialogRequest, bool, int)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.BuildFilter(IReadOnlyList<FileDialogFilter>)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.OpenFileName` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.BrowseInfo` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.SHBrowseForFolder(ref BrowseInfo)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.SHGetPathFromIDListEx(IntPtr, char*, int, int)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.CoTaskMemFree(IntPtr)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.OleInitialize(IntPtr)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.OleUninitialize()` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.GetOpenFileName(ref OpenFileName)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsFileDialogs.GetSaveFileName(ref OpenFileName)` in `src/Broiler.Code.Windows/WindowsFileDialogs.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.WmImeStartComposition` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.WmImeComposition` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.WmImeEndComposition` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.GcsCompStr` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.GcsResultStr` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.TryHandleMessage(uint, IntPtr, IntPtr)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.SetCaretRectangle(BRect)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.TryRead(int, out string)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=Critical, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.Point` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.Rect` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.CompositionForm` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.ImmGetContext(IntPtr)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.ImmReleaseContext(IntPtr, IntPtr)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.ImmGetCompositionStringW(IntPtr, int, IntPtr, int)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Windows.WindowsTextInputService.ImmSetCompositionWindow(IntPtr, ref CompositionForm)` in `src/Broiler.Code.Windows/WindowsTextInputService.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.CodeWorkspace(IWorkspaceStorage, WorkspaceIdFactory?)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.StorageFor(WorkspaceItemId)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.AddItem(string, WorkspaceItemKind, WorkspaceItemId, bool, string?)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.OpenDocumentAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.OpenGrantedDocumentAsync(IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.SaveDocumentAsAsync(WorkspaceItemId, IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.SaveDocumentAsync(WorkspaceItemId, bool, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.SaveAllAsync(CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.RenameItemAsync(WorkspaceItemId, string, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.HasExternalChangeAsync(WorkspaceItemId, CancellationToken)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.KeyFor(WorkspaceItemId, string)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.CodeWorkspace.GrantKey(IWorkspaceStorage, string)` in `src/Broiler.Code.Workspaces/CodeWorkspace.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Model.WorkspaceItemId` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Model.WorkspaceItemId.IsNone` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Model.WorkspaceIdFactory` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Model.WorkspaceIdFactory.WorkspaceIdFactory(long)` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Model.WorkspaceIdFactory.Next()` in `src/Broiler.Code.Workspaces/Model/WorkspaceIds.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Projects.DeclaredProjectFile` in `src/Broiler.Code.Workspaces/Projects/DeclaredProjectFile.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Projects.DeclaredProjectFile.Parse(string, string)` in `src/Broiler.Code.Workspaces/Projects/DeclaredProjectFile.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile.SlnProjectPattern` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile.Parse(string, string)` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile.ParseSlnx(string, string)` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Projects.DeclaredSolutionFile.ParseSln(string, string)` in `src/Broiler.Code.Workspaces/Projects/DeclaredSolutionFile.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.Options` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.ForWorkspace(string, string)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.RecordAsync(WorkspaceItemId, WorkspaceItem, string, CancellationToken)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.Forget(WorkspaceItemId)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.Clear()` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.ReadAllAsync(CancellationToken)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Recovery.RecoveryJournal.EntryPath(WorkspaceItemId)` in `src/Broiler.Code.Workspaces/Recovery/RecoveryJournal.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.FileSystemWorkspaceStorage(string)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.ReadTextAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.WriteTextAsync(string, string, TextEncodingInfo, string?, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.ListAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.StatAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.DeleteAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.RenameAsync(string, string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.TryResolve(string, out string, out StorageFailure?)` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.FileSystemWorkspaceStorage.Decode(byte[])` in `src/Broiler.Code.Workspaces/Storage/FileSystemWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.ReadTextAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.WriteTextAsync(string, string, Model.TextEncodingInfo, string?, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.ListAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.StatAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.DeleteAsync(string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.IWorkspaceStorage.RenameAsync(string, string, CancellationToken)` in `src/Broiler.Code.Workspaces/Storage/IWorkspaceStorage.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.WorkspacePath` in `src/Broiler.Code.Workspaces/Storage/WorkspacePath.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.WorkspacePath.Normalize(string)` in `src/Broiler.Code.Workspaces/Storage/WorkspacePath.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.WorkspacePath.IsContained(string, string)` in `src/Broiler.Code.Workspaces/Storage/WorkspacePath.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.Storage.WorkspacePath.IsReservedDeviceName(string)` in `src/Broiler.Code.Workspaces/Storage/WorkspacePath.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.WorkspaceLoader` in `src/Broiler.Code.Workspaces/WorkspaceLoader.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.WorkspaceLoader.LoadSolutionAsync(IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Workspaces/WorkspaceLoader.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.WorkspaceLoader.LoadProjectAsync(CodeWorkspace, IWorkspaceStorage, string, CancellationToken)` in `src/Broiler.Code.Workspaces/WorkspaceLoader.cs` - Security=High, human line PENDING
- `Broiler.Code.Workspaces.WorkspaceLoader.Combine(string, string)` in `src/Broiler.Code.Workspaces/WorkspaceLoader.cs` - Security=High, human line PENDING

## Falsification criteria

| Metric | Value |
|---|---:|
| Units carrying a criterion | 626 |
| Units required to carry one | 270 |
| Required and missing | 0 |

A `// Hairs-Falsified-If:` line states, at the declaration, the observation that would make
the unit wrong. `Security=High` says a unit is risky, which is a set and not a test; the
criterion is the test. It is required where `Security` is `High` or `Critical`, permitted
elsewhere, and `dotnet tool run hairs -- assurance check --root .` names every unit that owes one and carries none.

The line is a comment, so it is outside every fingerprint by construction: rewording a
criterion moves no recorded value here, in a file header or in
`hairs.manifest.json`, and invalidates nothing. That is the intended reading - a
criterion is an instruction to whoever reads the unit, not part of what a review is bound to.

## Exemption

Exemption is decided by one predicate in `CSharpAssuranceScanner`, not per unit, so
that the rule is reviewable in one place rather than in several hundred.

| Case | Units |
|---|---:|
| TrivialPropertyOrAccessor | 99 |
| ParameterAssigningConstructor | 5 |
| TrivialExpressionBodiedMember | 11 |
| CompilerSuppliedRecordOrEnumMember | 64 |
| DelegatingOverrideOrOperator | 2 |
| InsideAssemblyMarker | 0 |
| FieldDeclaringStorage | 183 |
| EnumMemberOfADeclaredVocabulary | 80 |
| DeclaredInSource | 0 |

## Per-unit exemptions

| Metric | Value |
|---|---:|
| Per-unit exemptions | 0 |

A per-unit `EXEMPT=<reason>` line exempts one unit by a reason a human wrote, for what the
predicate cannot see. Nothing mechanical checks that the reason is true, that it describes
the unit it sits on, or that it says anything at all, so every use is counted and named
here.

No unit in this component states a per-unit exemption.

## Files not covered

No file under a covered project's directory, and no file a covered project compiles in
through a `<Compile Include>` it states, is left out of the record.

## Change detection

`hairs.manifest.json` lists **every** code unit in the 6 covered assemblies -
1243 of them, exempt and relevant alike - with the fingerprint of its declaration.
This manifest is a change-detection record, not a review. A unit listed there is watched, not reviewed:
the entry records what the declaration's tokens hashed to when the generator last ran, and
nothing else. What the manifest adds is that a unit the exemption predicate treats as
trivial is no longer invisible: a semantic change to one moves a value in a generated file
the check compares byte for byte. `dotnet tool run hairs -- assurance check --root .` holds the manifest to the tree.

Beside the units it lists **every covered file** - 54 of them - with a
fingerprint over the complete token stream of its compilation unit. A unit entry exists only
for a declaration kind the scanner enumerates, and an enumeration is a whitelist: an
`[assembly: ...]` attribute is a member of nothing and can be in no unit at all.
Nothing in a covered file can change without something moving here, whatever kind of declaration it is. Comments are outside the stream, because a token's
text is its own characters, so the generated header above and the annotation lines below move
no file fingerprint - which is what lets one generation be a fixed point.

## Verification

The generator and the check are one computation: `dotnet tool run hairs -- assurance check --root .` works out what the
generator would write and compares it with the tree byte for byte, so a record edited by
hand, or left behind by code that moved, is reported rather than trusted.

| Mode | Command | Effect |
|---|---|---|
| Generate | `dotnet tool run hairs -- assurance generate --root .` | Fills every `Fingerprint=TBF`, refreshes a decision the code has outrun into `STALE; Previous=...`, rewrites the generated headers, `HUMAN_REVIEW.md`, `hairs.manifest.json` and this file. |
| Check | `dotnet tool run hairs -- assurance check --root .` | Reports every generated artefact that is not byte-identical to what the generator would produce, every relevant unit with no annotation, every annotation this system cannot read, every fingerprint out of date and every unit at the top of the security vocabulary without a criterion. |
| Release | `dotnet tool run hairs -- assurance check --root . --release` | The check, and additionally every relevant unit left in a state that blocks a release. |

The fingerprint is six hex characters - 24 bits - of SHA-256 over the declaration's token
texts, joined by single spaces. Trivia is excluded because a token's text is its own
characters and never the comments or whitespace around it, so `dotnet format` moves no
fingerprint and an annotation is never part of what it describes. The value answers whether a
unit changed since it was reviewed. It is not a collision-free identifier across units and it
is not a cryptographic commitment.
