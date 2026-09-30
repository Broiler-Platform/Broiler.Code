// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           1
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    Critical
// Criteria:         7/5
// Resource impact:  7/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Broiler.Code.Core.Hosting;
using Broiler.Code.Core.Shell;
using Broiler.Graphics;
using Broiler.Graphics.Geometry;
using Broiler.Graphics.RenderList;
using Broiler.UI;
using Broiler.UI.CodeEditor.Standard;
using Broiler.UI.Standard;

namespace Broiler.Code.Windows;

/// <summary>
/// Composes the session for the Windows head.
///
/// The composition is the head's only real job. Everything it wires — the
/// editor, the workspace, the shell — is built elsewhere; this decides which
/// platform services back them.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=79386E
// Broiler-Falsified-If: work posted by the analysis or review layers from a worker thread runs on that thread instead of being queued to the window dispatcher and drained on the message-loop thread
// Broiler-Human:        PENDING
[SupportedOSPlatform("windows7.0")]
internal static class CodeHost
{
    /// <summary>
    /// The service report without creating a window, so the support claim can
    /// be inspected on a machine with no display.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0F1C9F
    // Broiler-Falsified-If: a service this head backs with an in-process substitute is reported with native quality
    // Broiler-Human:        PENDING
    public static HostServiceReport DescribeServices() => new("Broiler Code (Windows)",
    [
        new HostService(
            HostServiceReport.Dispatcher,
            HostServiceQuality.Native,
            "queued and drained on the window's message-loop thread"),
        new HostService(
            HostServiceReport.InputRouting,
            HostServiceQuality.Native,
            "explicit Broiler.Input events with device identity and a monotonic sequence"),
        new HostService(
            HostServiceReport.Clipboard,
            HostServiceQuality.Native,
            "Win32 clipboard, with no in-memory fallback"),
        new HostService(
            HostServiceReport.TextInput,
            HostServiceQuality.Native,
            "IMM32 composition with candidate placement at the caret"),
        new HostService(
            HostServiceReport.FileDialogs,
            HostServiceQuality.Native,
            "the common dialogs, through comdlg32"),
    ]);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=D85FBC
    // Broiler-Falsified-If: work posted by the analysis or review layers from a worker thread runs on that thread instead of being queued to the window dispatcher and drained on the message-loop thread
    // Broiler-Human:        PENDING
    public static int Run(CodeWindow window, string? workspacePath = null)
    {
        ArgumentNullException.ThrowIfNull(window);

        var host = new CodeUiHost(window);
        (CodeShell shell, StandardCodeEditor editor) =
            CodeShellFactory.Create(new BSize(1280, 840));

        // The real dispatcher, not the Standard immediate one. Everything the
        // analysis and build layers post arrives on a worker thread, and this
        // is what moves it to the UI thread before it touches a control.
        UiSession session = new StandardUiSessionBuilder()
            .WithDispatcher(window.Dispatcher)
            .Build(host);

        // The shell's root, not the bare editor. Adding the editor alone was
        // the earlier mistake: it produced a window with no menu, no toolbar,
        // no explorer, and no document — so every keystroke was refused.
        session.AddRoot(shell.RootElement);
        window.AttachServices(session, editor);
        window.AttachShell(shell);
        host.Attach(window.Clipboard);

        // The same dispatcher the session uses. The review load reads every
        // record and every file it describes on a pool thread; without this it
        // would apply the result there too, racing the paint that reads it.
        shell.Dispatcher = window.Dispatcher;

        // The dialogs are what make Open and Save As real commands rather than
        // menu entries that raise an intent nobody handles.
        shell.FileDialogs = new WindowsFileDialogs(window.NativeHandle);

        // Cancel on a dirty close, until the head has a dialog to ask with.
        // Refusing to close is recoverable; discarding someone's work is not.
        shell.DirtyClosePrompt = (_, _) => ValueTask.FromResult(DirtyCloseChoice.Cancel);

        // Opened before the loop starts so the first frame already has a
        // document. The editor refuses every edit while its document is null.
        CodeShellFactory.OpenWorkspaceAsync(shell, workspacePath).AsTask().GetAwaiter().GetResult();

        // Focus is the session's, not the control's. Setting only
        // editor.HasFocus left UiSession.FocusedElement null, so every key and
        // character was routed by hit-testing the event's position — which for
        // a keyboard event is the origin, landing on the menu. The editor
        // rendered a caret and received nothing.
        window.FocusEditor();

        using (shell)
            return window.Run();
    }
}

/// <summary>
/// The UI host. Clipboard access is delegated to the Win32 implementation and
/// is absent until the window exists — there is deliberately no private string
/// standing in for it, so a missing clipboard is visible rather than silently
/// working only inside this process.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=7; Fingerprint=5E5342
// Broiler-Falsified-If: clipboard text is served from a string held in this process rather than from the Win32 clipboard, so a missing clipboard reads as working
// Broiler-Human:        PENDING
[SupportedOSPlatform("windows7.0")]
internal sealed class CodeUiHost(CodeWindow window) : IUiHost, IUiClipboardHost
{
    private IUiClipboardHost? _clipboard;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B1206E
    // Broiler-Human:        PENDING
    public BSize ViewportSize => window.ClientSize;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7FEB0E
    // Broiler-Human:        PENDING
    public double Scale => window.DpiScale;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=FF02F4
    // Broiler-Human:        PENDING
    public void Attach(IUiClipboardHost? clipboard) => _clipboard = clipboard;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=2; Fingerprint=516BFD
    // Broiler-Human:        PENDING
    public BRenderList CreateRenderList(int capacity = 0) => new(capacity);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4E9908
    // Broiler-Human:        PENDING
    public void Invalidate(UiInvalidation invalidation) => window.Invalidate();

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E72CAA
    // Broiler-Falsified-If: a render list passed to Present is submitted to the window, so the same frame is painted twice
    // Broiler-Human:        PENDING
    public void Present(BRenderList renderList)
    {
        // Presentation belongs to the window: it asks the session for a render
        // list in BuildRenderList and submits it as part of its own frame. A
        // second submission from here would paint the same frame twice, out of
        // step with the swap chain.
        ArgumentNullException.ThrowIfNull(renderList);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=7; Fingerprint=B03E24
    // Broiler-Falsified-If: a paste with no clipboard attached returns true or text from an in-process store instead of false and an empty string
    // Broiler-Human:        PENDING
    public bool TryGetText(out string text)
    {
        if (_clipboard is not null)
            return _clipboard.TryGetText(out text);

        text = string.Empty;
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=B5EF0D
    // Broiler-Falsified-If: text set while no clipboard is attached is kept in this process and later returned by TryGetText
    // Broiler-Human:        PENDING
    public void SetText(string text) => _clipboard?.SetText(text);
}
