# Broiler.Code Code Assurance

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance generate --root Broiler.Code`, which rewrites this file,
`HUMAN_REVIEW.md`, `assurance.manifest.json` and every generated source header from the
product tree.

**No code unit in this component carries a decision on its human line yet.** This report
records that absence precisely. It is not a claim that the code is reviewed, assured or safe,
and the figures below are the measurement of how far from that claim the per-unit record is.

## Summary

| Metric | Value |
|---|---:|
| Files scanned | 104 |
| Files not covered | 0 |
| Files carrying an annotation | 104 |
| Code units | 2227 |
| Relevant | 1541 |
| Exempt by predicate | 686 |
| Annotated | 1541 of 1541 (100%) |
| Human reviewed | 0 of 1541 (0%) |
| Unverified | 1541 |

## Review states

| State | Count |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 1541 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 686 |

## IP risk

| Value | Units |
|---|---:|
| None | 597 |
| Low | 944 |
| Medium | 0 |
| High | 0 |
| Unknown | 0 |
| *not annotated* | 0 |

## Security risk

| Value | Units |
|---|---:|
| None | 59 |
| Low | 404 |
| Medium | 365 |
| High | 677 |
| Critical | 36 |
| *not annotated* | 0 |

## Resource impact

| Metric | Value |
|---|---:|
| Maximum | 9 / 10 |
| Average over annotated units | 2.2 / 10 |
| Units scored | 1541 |

## High-security review areas

- `Broiler.App.LinuxX11Clipboard` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.SelectionClear` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.SelectionRequest` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.SelectionNotify` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.PropertyNotify` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.PropertyNewValue` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.PropertyChangeMask` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.PropModeReplace` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XaAtom` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XaString` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.ConvertTimeout` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.LinuxX11Clipboard(IntPtr, IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.TryOpen()` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.OwnsClipboard` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.TryGetText(out string)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.SetText(string)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.ProcessPendingEvents()` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.Dispose()` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.HandleEvent(ref XEvent)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.AnswerSelectionRequest(ref XSelectionRequestEvent)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.TryWriteRequestedTarget(ref XSelectionRequestEvent, IntPtr, string)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.MaxPropertyBytes()` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.TryConvert(IntPtr, Encoding, out string)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.TryWaitForSelectionNotify(IntPtr, out bool)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.TryReadIncrementally(Encoding, out string)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.Deadline()` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.TryReadProperty(bool, out byte[], out IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.InternAtom(IntPtr, string)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XEvent` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XSelectionRequestEvent` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XSelectionEvent` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XPropertyEvent` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XOpenDisplay(IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XCloseDisplay(IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XDefaultScreen(IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XRootWindow(IntPtr, int)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XCreateSimpleWindow(IntPtr, IntPtr, int, int, uint, uint, uint, IntPtr, IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XDestroyWindow(IntPtr, IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XSelectInput(IntPtr, IntPtr, long)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XInternAtom(IntPtr, string, int)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XSetSelectionOwner(IntPtr, IntPtr, IntPtr, IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XGetSelectionOwner(IntPtr, IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XConvertSelection(IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XChangeProperty(IntPtr, IntPtr, IntPtr, IntPtr, int, int, byte[], int)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XChangeProperty(IntPtr, IntPtr, IntPtr, IntPtr, int, int, IntPtr[], int)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XDeleteProperty(IntPtr, IntPtr, IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XGetWindowProperty(IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, int, IntPtr, out IntPtr, out int, out IntPtr, out IntPtr, out IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XSendEvent(IntPtr, IntPtr, int, long, ref XEvent)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XMaxRequestSize(IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XExtendedMaxRequestSize(IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XPending(IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XNextEvent(IntPtr, out XEvent)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XFlush(IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=High, human line PENDING
- `Broiler.App.LinuxX11Clipboard.XFree(IntPtr)` in `src/Broiler.App/LinuxX11Clipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.WindowsClipboard` in `src/Broiler.App/WindowsClipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.WindowsClipboard.CfUnicodeText` in `src/Broiler.App/WindowsClipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.WindowsClipboard.GmemMoveable` in `src/Broiler.App/WindowsClipboard.cs` - Security=High, human line PENDING
- `Broiler.App.WindowsClipboard.TryGetText(out string)` in `src/Broiler.App/WindowsClipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.WindowsClipboard.SetText(string)` in `src/Broiler.App/WindowsClipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.WindowsClipboard.OpenClipboard(IntPtr)` in `src/Broiler.App/WindowsClipboard.cs` - Security=High, human line PENDING
- `Broiler.App.WindowsClipboard.CloseClipboard()` in `src/Broiler.App/WindowsClipboard.cs` - Security=High, human line PENDING
- `Broiler.App.WindowsClipboard.EmptyClipboard()` in `src/Broiler.App/WindowsClipboard.cs` - Security=High, human line PENDING
- `Broiler.App.WindowsClipboard.IsClipboardFormatAvailable(uint)` in `src/Broiler.App/WindowsClipboard.cs` - Security=High, human line PENDING
- `Broiler.App.WindowsClipboard.GetClipboardData(uint)` in `src/Broiler.App/WindowsClipboard.cs` - Security=High, human line PENDING
- `Broiler.App.WindowsClipboard.SetClipboardData(uint, IntPtr)` in `src/Broiler.App/WindowsClipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.WindowsClipboard.GlobalAlloc(uint, UIntPtr)` in `src/Broiler.App/WindowsClipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.WindowsClipboard.GlobalFree(IntPtr)` in `src/Broiler.App/WindowsClipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.WindowsClipboard.GlobalLock(IntPtr)` in `src/Broiler.App/WindowsClipboard.cs` - Security=Critical, human line PENDING
- `Broiler.App.WindowsClipboard.GlobalUnlock(IntPtr)` in `src/Broiler.App/WindowsClipboard.cs` - Security=High, human line PENDING
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
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.Markers` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.CSharpAssuranceFileScanner(IEnumerable<string>?, AssuranceExemptionPredicate, AssuranceNamedValues)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.ScanFile(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.IsCommentLike(SyntaxTrivia)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.LinesOf(SourceText, SyntaxTrivia)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.OpensWithMarker(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.LineOf(SyntaxTree, TextSpan)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceFileScanner.IsBlank(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceFileScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.DefaultSymbols` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.DefaultParseOptions` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.CSharpAssuranceScanner()` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.CSharpAssuranceScanner(IEnumerable<string>?, AssuranceExemptionPredicate, AssuranceNamedValues)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.DefaultPreprocessorSymbols` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Scan(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.OptionsFor(IEnumerable<string>)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.CodeUnits(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Units(SyntaxTree, AssuranceExemptionPredicate, AssuranceNamedValues)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Describe(SyntaxTree, MemberDeclarationSyntax, AssuranceExemptionPredicate, AssuranceNamedValues)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.TopLevel(SyntaxTree, IReadOnlyList<GlobalStatementSyntax>)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Fingerprint(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.TokenStream(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.FingerprintOfFile(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Hash(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.StreamOf(IEnumerable<SyntaxToken>, bool)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.HiddenCode(SyntaxTriviaList)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Normalized(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Tokens(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.HeaderTokens(TypeDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsCodeUnit(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.ExemptionFor(MemberDeclarationSyntax, bool, bool)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsNamedValue(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsStatedByLiterals(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsLiteral(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.SystemTypeName(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.DottedName(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsFixedValue(FieldDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsInert(ExpressionSyntax?)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.ThrowsInertly(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsTrivialProperty(BasePropertyDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsCorrespondingMemberAccess(ExpressionSyntax, string?)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsFieldAssignmentFromValue(ExpressionSyntax, string?)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.SimpleNameOf(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.AssignsParametersOnly(ConstructorDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.AssignedMemberName(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Corresponds(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsSingleMemberAccess(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsDelegationToOwnMember(ExpressionSyntax, SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsForwardedParameter(ExpressionSyntax, SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsConstant(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsThrowNew(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.IsOverrideOrOperator(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.OnlyDelegates(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.ArrowBody(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.SingleReturnedExpression(MemberDeclarationSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.SuppliesAnImplementation(MemberDeclarationSyntax, bool)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.Unwrap(ExpressionSyntax)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpAssuranceScanner.ContainingTypes(SyntaxNode)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.MaxConfigurationBytes` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.CSharpComponentAssuranceScanner(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.Scan(string, string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.ConfigurationFor(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.ScannerFor(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.IsRoot(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.IsUnderRoot(string)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Language.CSharp.Assurance.CSharpComponentAssuranceScanner.IsLink(FileSystemInfo)` in `src/Broiler.Code.Language.CSharp.Assurance/CSharpComponentAssuranceScanner.cs` - Security=High, human line PENDING
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
- `Broiler.Code.Linux.LinuxCodeInput` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxCodeInput.IsAvailable` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxCodeInput.StartAsync(CancellationToken)` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxCodeInput.SetActiveAsync(bool, CancellationToken)` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxCodeInput.DisposeAsync()` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxCodeInput.OpenKeyboardAsync(CancellationToken)` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, human line PENDING
- `Broiler.Code.Linux.LinuxCodeInput.OpenMouseAsync(CancellationToken)` in `src/Broiler.Code.Linux/LinuxCodeInput.cs` - Security=High, human line PENDING
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
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Done` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Refused` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.UsageError` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Run(IReadOnlyList<string>, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.ScannerFor(AssuranceComponentConfig?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.List(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Insert(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.LoadConfig(string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.RootOf(Options)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.PrepareOut(string?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.WriteOut(string, string, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Generate(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Check(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Status(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.OwnedConfig(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.ReadableConfig(string, Options, string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.WorkflowCommand(AssuranceViolation, string?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.EscapeData(string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.EscapeProperty(string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceGateCommands.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceJson` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceJson.AssessmentFields` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceJson.ReadAssessments(string)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceJson.ReadEntry(JsonElement, int)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand` in `src/Broiler.Code.Review.Cli/Assurance/AssurancePruneCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.Prune(Options, TextWriter, TextWriter)` in `src/Broiler.Code.Review.Cli/Assurance/AssurancePruneCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceCommand.ReadBack(ComponentSourceFile, AssuranceSourceText, string, CSharpAssuranceFileScanner)` in `src/Broiler.Code.Review.Cli/Assurance/AssurancePruneCommand.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.Utf8Bom` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.Utf16LittleEndianBom` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.Utf16BigEndianBom` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.StrictUtf8` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.TryRead(string, out AssuranceSourceText?, out string?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.AssuranceSourceText.TryWrite(string, string, out string?)` in `src/Broiler.Code.Review.Cli/Assurance/AssuranceSourceText.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.Load(string, AssuranceComponentConfig)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.CheckWritable(string, string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.SeparateRecords(string, AssuranceArtefactPaths)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.AdrRecords(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentCorpus.TryWrite(AssuranceArtefact, out string?)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentCorpus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.BuildOutput` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Discover(string, AssuranceComponentConfig?)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Restrict(ComponentSourceSet, string, IEnumerable<string>, out IReadOnlyList<ComponentUnknownPath>)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Find(ComponentSourceSet, string, string, out ComponentUnknownPath?)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Normalize(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.AssemblyNameOf(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.CheckClosedAssemblies(AssuranceComponentConfig, IEnumerable<ComponentProject>)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Walk(string, string, HashSet<string>, bool)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Unenterable(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.IsNestedCheckout(string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.IsLink(string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.CompileItems(string, string, ComponentProject)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.ProjectFiles(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.DirectoryBuildFiles(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Resolve(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.IsUnder(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Load(string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.GuessProjects(string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.LooksLikeProduct(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.Assurance.ComponentSources.Relative(string, string)` in `src/Broiler.Code.Review.Cli/Assurance/ComponentSources.cs` - Security=High, human line PENDING
- `<top-level statements>` in `src/Broiler.Code.Review.Cli/Program.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.ReviewReport` in `src/Broiler.Code.Review.Cli/ReviewReport.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.ReviewReport.EvaluateAsync(string, IReadOnlyList<string>, CancellationToken)` in `src/Broiler.Code.Review.Cli/ReviewReport.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.ReviewReport.Regressions(IReadOnlyList<ReviewedFile>)` in `src/Broiler.Code.Review.Cli/ReviewReport.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.InventoryOptions` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.SourceInventory` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.SourceInventory.Enumerate(string, InventoryOptions?)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.SourceInventory.IdentityOf(string)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.SourceInventory.ComponentDepth(string)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.SourceInventory.Walk(string, string, InventoryOptions, List<string>)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Cli.SourceInventory.IsGenerated(string)` in `src/Broiler.Code.Review.Cli/SourceInventory.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.Previous` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.RecordedFingerprint` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.ExemptReason` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.HumanIsPending` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.HumanIsStale` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.Reviewer` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.HumanFingerprint` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.HumanAssessment` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.TryParse(AssuranceLines, int, out AssuranceAnnotation?)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.TryParseStrict(AssuranceLines, int, out AssuranceAnnotation?, out string?)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.Field(string)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.Body(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceAnnotation.ParseFields(string)` in `src/Broiler.Code.Review/Assurance/AssuranceAnnotation.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceBanner` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceBanner.SpdxCopyrightPrefix` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceBanner.GeneratedMarker` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceBanner.Banner` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceBanner.BannerRule` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceBanner.SpdxLicensePrefix` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceBanner.SpdxPrefix` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceBanner.RowLabels` in `src/Broiler.Code.Review/Assurance/AssuranceBanner.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceCandidates` in `src/Broiler.Code.Review/Assurance/AssuranceCandidates.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceCandidates.None` in `src/Broiler.Code.Review/Assurance/AssuranceCandidates.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceCandidates.Classify(AssuranceLines, AssuranceScannedFile)` in `src/Broiler.Code.Review/Assurance/AssuranceCandidates.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceCandidates.ReasonFor(AssuranceFileUnit, AssuranceCandidate, AssuranceScannedFile, bool)` in `src/Broiler.Code.Review/Assurance/AssuranceCandidates.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.Run(AssurancePlan, AssuranceComponentConfig, AssuranceCheckOptions)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.MissingCriteria(IEnumerable<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.MissingCriteriaOf(IEnumerable<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.CriteriaBelowHighOf(IEnumerable<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.Orphans(AssurancePlannedFile)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.SpecViolations(IEnumerable<AssuranceCorpusUnit>, IReadOnlySet<string>, string)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.FingerprintViolations(IEnumerable<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.InventedApprovals(AssurancePlan)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.StaleArtefacts(AssurancePlan, bool)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.ReviewClaims(AssurancePlan, bool)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceChecks.Unresolved(AssurancePlan)` in `src/Broiler.Code.Review/Assurance/AssuranceChecks.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceArtefactPaths` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.FileName` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.CurrentSchema` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Parse(string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.ProjectList(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Exclusions(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Overrides(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.SpdxOf(JsonElement, string, bool)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.ArtefactPaths(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.CommentLines(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Symbols(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.Glob(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.StringList(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.RelativePath(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.CheckRelative(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.SingleLine(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.CheckSingleLine(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.RequiredString(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceComponentConfig.RequiredBool(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceComponentConfig.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceCorpusUnit` in `src/Broiler.Code.Review/Assurance/AssuranceCorpus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceCorpusUnit.IsRelevant` in `src/Broiler.Code.Review/Assurance/AssuranceCorpus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.AssuranceDocument(AssuranceLines, IReadOnlyList<AssuranceUnit>, int, IAssuranceUnitScanner?, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.Read(string, IAssuranceUnitScanner?, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.ApprovalBody(AssuranceUnit, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.Approve(AssuranceUnit, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.Withdraw(AssuranceUnit)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.Rewrite(AssuranceUnit, AssuranceAnnotation, string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.RecountBanner()` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.Summarize()` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.ReproducesBanner(AssuranceSummary)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.FromScanner(AssuranceLines, IAssuranceUnitScanner, string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.FromAnnotations(AssuranceLines)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceDocument.AnnotationAbove(AssuranceLines, int)` in `src/Broiler.Code.Review/Assurance/AssuranceDocument.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceArtefact` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceArtefact.IsCurrent` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssurancePlan` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssurancePlan.Changes` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator.GeneratedNotice` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator.Plan(AssuranceCorpus, IAssuranceFileScanner, AssuranceComponentConfig)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator.IsGenerated(string)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator.Artefact(AssuranceCorpus, string, AssuranceArtefactKind, string)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator.PlanFile(AssuranceSource, IAssuranceFileScanner, AssuranceComponentConfig, List<AssuranceViolation>)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator.Refresh(string, IReadOnlyList<AssuranceCorpusUnit>, List<AssuranceViolation>)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator.RefreshedFields(AssuranceAnnotation, string)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator.Units(AssuranceSource, AssuranceLines, AssuranceScannedFile)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGenerator.ChangedCode(AssuranceScannedFile, AssuranceScannedFile, AssuranceLines)` in `src/Broiler.Code.Review/Assurance/AssuranceGenerator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGlob` in `src/Broiler.Code.Review/Assurance/AssuranceGlob.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGlob.TryParse(string?, out AssuranceGlob?, out string?)` in `src/Broiler.Code.Review/Assurance/AssuranceGlob.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGlob.IsMatch(string)` in `src/Broiler.Code.Review/Assurance/AssuranceGlob.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceGlob.ToRegex(string)` in `src/Broiler.Code.Review/Assurance/AssuranceGlob.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.NormalizedRowLabels` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.StrictVocabulary` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.NarrowOpenings` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.IsSummaryLine(string, AssuranceForgeryVocabulary)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.BannerCount(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.DuplicateBanners(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.IsSummaryComment(string, AssuranceForgeryVocabulary)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.ForgedSummary(string, string, IReadOnlyList<AssuranceCommentLine>, AssuranceForgeryVocabulary)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.CommentContent(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.WordsOf(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.GeneratedHeaderLines(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.Strip(AssuranceLines, IReadOnlyList<string>, AssuranceForgeryVocabulary, string, out IReadOnlyList<string>)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.IsHeaderShaped(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHeader.LeadingRunIsGeneratedCopy(AssuranceLines)` in `src/Broiler.Code.Review/Assurance/AssuranceHeader.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHumanLine` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.PreviousMarker` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.FingerprintMarker` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.Refreshed(AssuranceAnnotation, string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.IsDefined(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.ReviewerNames(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.RefuseInventedApproval(string, string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceHumanLine.BodiesIn(string)` in `src/Broiler.Code.Review/Assurance/AssuranceHumanLine.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceInsertion` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Validate(AssuranceAssessment)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Render(AssuranceAssessment, string)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Apply(string, string, IAssuranceFileScanner, IReadOnlyList<AssuranceAssessment>, bool, string?)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Resolve(IReadOnlyList<AssuranceCandidate>, AssuranceAssessment)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Verify(string, string, string, IAssuranceFileScanner, AssuranceScannedFile, IReadOnlyList<AssuranceCandidate>, Dictionary<int, IReadOnlyList<string>>)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceInsertion.Closed(string, string?, string[], List<string>)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceInsertion.IsOneLine(string)` in `src/Broiler.Code.Review/Assurance/AssuranceInsertion.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceLines` in `src/Broiler.Code.Review/Assurance/AssuranceLines.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceLines.AssuranceLines(string)` in `src/Broiler.Code.Review/Assurance/AssuranceLines.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceLines.Render()` in `src/Broiler.Code.Review/Assurance/AssuranceLines.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest.FilesArrayOpening` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest.ArraysOf(string)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest.Violations(string, IEnumerable<AssuranceManifestFile>, IEnumerable<AssuranceCorpusUnit>, string, string?)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest.Entry(AssuranceCorpusUnit)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest.ShapeViolations(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest.FileViolations(string, IEnumerable<AssuranceManifestFile>, string)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest.ReadUnits(string, string, List<string>)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest.ReadFiles(string, string, List<string>)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceManifest.Text(JsonElement, string)` in `src/Broiler.Code.Review/Assurance/AssuranceManifest.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssurancePruning` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssurancePruning.Apply(string, string, IAssuranceFileScanner)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssurancePruning.CodeDifference(AssuranceScannedFile, AssuranceScannedFile)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssurancePruning.RemovalFor(AssuranceCandidate, AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssurancePruning.Verify(string, string, string, IAssuranceFileScanner, AssuranceScannedFile, IReadOnlyList<AssuranceCandidate>, IReadOnlyDictionary<int, (int First, int Count, AssurancePruneKind Kind)>, IReadOnlyList<(int Line, string Text, string Separator)>)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssurancePruning.SameAssessment(AssuranceAnnotation, AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssurancePruning.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.Negations` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.ClauseSeparators` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.Violations(string, IReadOnlyList<string>, IReadOnlyList<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.WholeWord(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.IsSupported(string, int, string, IReadOnlyList<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.IsWordBefore(string, string, int)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.Supported(string, IReadOnlyList<AssuranceCorpusUnit>)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceReviewClaims.FirstNumberAfter(string, int)` in `src/Broiler.Code.Review/Assurance/AssuranceReviewClaims.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.ReviewClaimTerms` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.SecurityRequiringACriterion` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.SecurityWritingNoCriterion` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.CriterionProblems(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.ExemptionReasonProblems(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.SpecProblems(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.ReviewClaimsIn(string)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.VocabularyProblems(AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.RequiresFalsificationCriterion(AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.CarriesCriterionBelowHigh(AssuranceAnnotation)` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.FieldOnACriterion()` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceRules.ReviewClaim()` in `src/Broiler.Code.Review/Assurance/AssuranceRules.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.IAssuranceFileScanner` in `src/Broiler.Code.Review/Assurance/AssuranceScannedFile.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.IAssuranceFileScanner.ScanFile(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceScannedFile.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.IAssuranceUnitScanner` in `src/Broiler.Code.Review/Assurance/AssuranceUnit.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.IAssuranceUnitScanner.Scan(string, string)` in `src/Broiler.Code.Review/Assurance/AssuranceUnit.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceUnitState` in `src/Broiler.Code.Review/Assurance/AssuranceUnitState.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceStateMachine` in `src/Broiler.Code.Review/Assurance/AssuranceUnitState.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceStateMachine.Resolve(AssuranceAnnotation?, bool, string?)` in `src/Broiler.Code.Review/Assurance/AssuranceUnitState.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceStateMachine.BlocksRelease(AssuranceUnitState)` in `src/Broiler.Code.Review/Assurance/AssuranceUnitState.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.AiMarker` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.FalsifiedIfMarker` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.HumanMarker` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.Pending` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.Stale` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.ToBeFilled` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.FingerprintWidth` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.FingerprintField` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.ExemptField` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.HumanFieldMarkers` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.IsWellFormedFingerprint(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.MaxAliasLength` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.PlaceholderWords` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.IsAlias(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.IsWritableReviewer(string?)` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.Assurance.AssuranceVocabulary.AllHex(string)` in `src/Broiler.Code.Review/Assurance/AssuranceVocabulary.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.FileReview` in `src/Broiler.Code.Review/FileReview.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.FileReview.WithDecision(ReviewStatus, string, string, DateTimeOffset, string?)` in `src/Broiler.Code.Review/FileReview.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.NoteAnchoring` in `src/Broiler.Code.Review/NoteAnchoring.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.NoteAnchoring.Place(FileReview, string)` in `src/Broiler.Code.Review/NoteAnchoring.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.NoteAnchoring.Place(ReviewNote, string[])` in `src/Broiler.Code.Review/NoteAnchoring.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.NoteAnchoring.MatchesAt(string[], string[], int)` in `src/Broiler.Code.Review/NoteAnchoring.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewContentHash` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewContentHash.Algorithm` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewContentHash.ByteOrderMark` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewContentHash.Compute(string)` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewContentHash.Matches(string?, string)` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewContentHash.IsKnownAlgorithm(string?)` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewContentHash.Normalize(string)` in `src/Broiler.Code.Review/ReviewContentHash.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewCoverageTotals` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewCoverageTotals.VerifiedPercent` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewCoverage` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewCoverage.Overall(IEnumerable<ReviewedFile>, string)` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewCoverage.Count(string, IEnumerable<ReviewedFile>)` in `src/Broiler.Code.Review/ReviewCoverage.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewJson` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewJson.Write(FileReview)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewJson.Read(string, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewJson.ReadNote(JsonObject, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewJson.ToWire(ReviewStatus)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewJson.FromWire(string?)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewJson.GetString(JsonObject, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewJson.GetInt(JsonObject, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewJson.GetTimestamp(JsonObject, string)` in `src/Broiler.Code.Review/ReviewJson.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStateEvaluator` in `src/Broiler.Code.Review/ReviewStateEvaluator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStateEvaluator.Evaluate(FileReview?, string?)` in `src/Broiler.Code.Review/ReviewStateEvaluator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStateEvaluator.FreshnessOf(FileReview, string?)` in `src/Broiler.Code.Review/ReviewStateEvaluator.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStatus` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewFreshness` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewState` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewState.None` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewState.IsVerified` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewState.IsStaleApproval` in `src/Broiler.Code.Review/ReviewStatus.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore.ReviewDirectory` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore.RecordSuffix` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore.RecordPathFor(string)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore.SourcePathFor(string)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore.IsRecordPath(string)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore.ReadAsync(string, CancellationToken)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore.WriteAsync(FileReview, CancellationToken)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore.ReadAllAsync(CancellationToken)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
- `Broiler.Code.Review.ReviewStore.CollectAsync(string, Dictionary<string, FileReview>, List<StorageFailure>, CancellationToken)` in `src/Broiler.Code.Review/ReviewStore.cs` - Security=High, human line PENDING
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
| Units carrying a criterion | 1252 |
| Units required to carry one | 713 |
| Required and missing | 0 |

A `Broiler-Falsified-If:` line states, at the declaration, the observation that would make
the unit wrong. `Security=High` says a unit is risky, which is a set and not a test; the
criterion is the test. It is required where `Security` is `High` or `Critical`, permitted
elsewhere, and `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Code` names every unit that owes one and carries none.

The line is a comment, so it is outside every fingerprint by construction: rewording a
criterion moves no recorded value here, in a file header or in
`assurance.manifest.json`, and invalidates nothing. That is the intended reading - a
criterion is an instruction to whoever reads the unit, not part of what a review is bound to.

## Exemption

Exemption is decided by one predicate in `CSharpAssuranceScanner`, not per unit, so
that the rule is reviewable in one place rather than in several hundred.

| Case | Units |
|---|---:|
| TrivialPropertyOrAccessor | 130 |
| ParameterAssigningConstructor | 9 |
| TrivialExpressionBodiedMember | 11 |
| CompilerSuppliedRecordOrEnumMember | 143 |
| DelegatingOverrideOrOperator | 2 |
| InsideAssemblyMarker | 0 |
| FieldDeclaringStorage | 254 |
| EnumMemberOfADeclaredVocabulary | 137 |
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

`assurance.manifest.json` lists **every** code unit in the 9 covered assemblies -
2227 of them, exempt and relevant alike - with the fingerprint of its declaration.
This manifest is a change-detection record, not a review. A unit listed there is watched, not reviewed:
the entry records what the declaration's tokens hashed to when the generator last ran, and
nothing else. What the manifest adds is that a unit the exemption predicate treats as
trivial is no longer invisible: a semantic change to one moves a value in a generated file
the check compares byte for byte. `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Code` holds the manifest to the tree.

Beside the units it lists **every covered file** - 104 of them - with a
fingerprint over the complete token stream of its compilation unit. A unit entry exists only
for a declaration kind the scanner enumerates, and an enumeration is a whitelist: an
`[assembly: ...]` attribute is a member of nothing and can be in no unit at all.
Nothing in a covered file can change without something moving here, whatever kind of declaration it is. Comments are outside the stream, because a token's
text is its own characters, so the generated header above and the annotation lines below move
no file fingerprint - which is what lets one generation be a fixed point.

## Verification

The generator and the check are one computation: `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Code` works out what the
generator would write and compares it with the tree byte for byte, so a record edited by
hand, or left behind by code that moved, is reported rather than trusted.

| Mode | Command | Effect |
|---|---|---|
| Generate | `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance generate --root Broiler.Code` | Fills every `Fingerprint=TBF`, refreshes a decision the code has outrun into `STALE; Previous=...`, rewrites the generated headers, `HUMAN_REVIEW.md`, `assurance.manifest.json` and this file. |
| Check | `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Code` | Reports every generated artefact that is not byte-identical to what the generator would produce, every relevant unit with no annotation, every annotation this system cannot read, every fingerprint out of date and every unit at the top of the security vocabulary without a criterion. |
| Release | `dotnet run --project Broiler.Code/src/Broiler.Code.Review.Cli -c Release -- assurance check --root Broiler.Code --release` | The check, and additionally every relevant unit left in a state that blocks a release. |

The fingerprint is six hex characters - 24 bits - of SHA-256 over the declaration's token
texts, joined by single spaces. Trivia is excluded because a token's text is its own
characters and never the comments or whitespace around it, so `dotnet format` moves no
fingerprint and an annotation is never part of what it describes. The value answers whether a
unit changed since it was reviewed. It is not a collision-free identifier across units and it
is not a cryptographic commitment.
