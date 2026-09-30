// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           11
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    High
// Criteria:         17/4
// Resource impact:  7/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Runtime.Versioning;
using Broiler.App;
using Broiler.Code.Core.Hosting;
using Broiler.Code.Core.Shell;
using Broiler.Graphics;
using Broiler.Graphics.Color;
using Broiler.Graphics.Geometry;
using Broiler.Graphics.Rendering;
using Broiler.Graphics.RenderList;
using Broiler.Graphics.Windowing;
using Broiler.Graphics.Windows;
using Broiler.UI;
using Broiler.UI.CodeEditor.Standard;

namespace Broiler.Code.Windows;

/// <summary>
/// The Win32/Direct2D host for Broiler Code.
///
/// It owns the window, the four platform services, and nothing else. Shell and
/// workspace behaviour is Broiler.Code.Core's, and the architecture test
/// rejects a copy of either appearing in this assembly.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=1D541C
// Broiler-Falsified-If: UI work posted from a worker thread runs on a thread other than the one pumping this window's message loop
// Broiler-Human:        PENDING
[SupportedOSPlatform("windows7.0")]
internal sealed class CodeWindow : Direct2DWindow
{
    private readonly DesktopInputRouter _input = new("broiler-code-windows");
    private readonly UiThreadDispatcher _dispatcher;
    private WindowsClipboard? _clipboard;
    private WindowsTextInputService? _textInput;
    private StandardCodeEditor? _editor;
    private UiSession? _session;
    private CodeShell? _shell;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8E4DC8
    // Broiler-Falsified-If: the dispatcher is created on a thread other than the one that runs the message loop, so CheckAccess answers for a thread that never drains it
    // Broiler-Human:        PENDING
    public CodeWindow()
        : base(new BWindowOptions
        {
            Title = "Broiler Code",
            ClientWidth = 1280,
            ClientHeight = 840,
            ClearColor = new BColor(0xFF, 0xFF, 0xFF),
            RenderOptions = new BRenderOptions(Antialias: true, VSync: true, SubpixelText: true),
        })
    {
        // The dispatcher is constructed on the thread that will run the message
        // loop, and wakes it by invalidating. Constructing it anywhere else
        // would capture the wrong thread as the UI thread and every
        // CheckAccess would answer for a thread that never pumps.
        _dispatcher = new UiThreadDispatcher(Invalidate);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=50271D
    // Broiler-Falsified-If: a callback posted before a frame is built does not run until after that frame, so a classification that has landed is painted one frame late
    // Broiler-Human:        PENDING
    protected override BRenderList? BuildRenderList(BSize clientSize)
    {
        // Queued UI work runs before the frame is built, so a classification
        // that landed since the last frame is visible in this one rather than
        // the next.
        _dispatcher.Drain();
        return _session?.RenderFrame();
    }

    protected override void OnResized(BSize clientSize, double dpiScale) => Invalidate();

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=FC807B
    // Broiler-Falsified-If: after a press on the explorer tree the editor still holds session focus, so the next key goes to the editor
    // Broiler-Human:        PENDING
    protected override void OnPointerDown(BPointerEventArgs e)
    {
        // Focus follows the click, before the event is dispatched, so the press
        // that moves the caret is handled by the control that now has focus.
        // Without this the editor keeps focus forever once it has it, and the
        // tree never gets a key.
        if (_session is not null && _shell is not null)
        {
            UiElement? target = _shell.ResolveFocusTarget(
                _session.HitTest(new BPoint(e.Position.X, e.Position.Y)));
            if (target is not null)
                SetFocus(target);
        }

        Dispatch(_input.FromPointerButton(e, pressed: true));
    }

    /// <summary>
    /// Moves session focus, and tells the editor whether it has it. The session
    /// decides where keys go; the control only draws the caret, so both have to
    /// be told or the caret and the keystrokes disagree.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C365EB
    // Broiler-Falsified-If: after focus moves from the editor to another control the editor still reports HasFocus and draws a caret
    // Broiler-Human:        PENDING
    private void SetFocus(UiElement target)
    {
        if (_session is null || ReferenceEquals(_session.FocusedElement, target))
            return;

        _session.SetFocus(target);
        if (_editor is not null)
            _editor.HasFocus = ReferenceEquals(target, _editor);
    }

    /// <summary>Gives the editor keyboard focus, for the first frame.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=05D25C
    // Broiler-Falsified-If: after FocusEditor with an editor attached, the session's focused element is not the editor
    // Broiler-Human:        PENDING
    public void FocusEditor()
    {
        if (_editor is not null)
            SetFocus(_editor);
    }

    /// <summary>The shell, for focus routing. It owns which pane a hit belongs to.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=212F78
    // Broiler-Human:        PENDING
    public void AttachShell(CodeShell shell)
    {
        ArgumentNullException.ThrowIfNull(shell);
        _shell = shell;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=7; Fingerprint=780407
    // Broiler-Falsified-If: a pointer move reaches the session as a button change instead of a move
    // Broiler-Human:        PENDING
    protected override void OnPointerMove(BPointerEventArgs e) =>
        Dispatch(_input.FromPointerMove(e));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=7; Fingerprint=738690
    // Broiler-Falsified-If: a button release reaches the session as a press, so the control sees a second press
    // Broiler-Human:        PENDING
    protected override void OnPointerUp(BPointerEventArgs e) =>
        Dispatch(_input.FromPointerButton(e, pressed: false));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=7; Fingerprint=52FBCA
    // Broiler-Falsified-If: a wheel turn reaches the session as a pointer move or button change instead of a wheel event
    // Broiler-Human:        PENDING
    protected override void OnMouseWheel(BMouseWheelEventArgs e) =>
        Dispatch(_input.FromWheel(e));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=7; Fingerprint=C2D643
    // Broiler-Falsified-If: a key press reaches the session as a key release
    // Broiler-Human:        PENDING
    protected override void OnKeyDown(BKeyEventArgs e) =>
        Dispatch(_input.FromKey(e, pressed: true));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=7; Fingerprint=B2583E
    // Broiler-Falsified-If: a key release reaches the session as a key press
    // Broiler-Human:        PENDING
    protected override void OnKeyUp(BKeyEventArgs e) =>
        Dispatch(_input.FromKey(e, pressed: false));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=B23D13
    // Broiler-Falsified-If: a character arriving by WM_CHAR while an IME composition is in progress is inserted alongside the composed text
    // Broiler-Human:        PENDING
    protected override void OnTextInput(BTextInputEventArgs e)
    {
        // While an IME composition is active the composition path owns the
        // text. Letting WM_CHAR through as well would insert every candidate
        // character twice.
        if (_textInput is not null && _composing)
            return;
        Dispatch(_input.FromText(e.Character.ToString()));
    }

    private bool _composing;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=A244C0
    // Broiler-Falsified-If: a committed IME composition is inserted twice, once from the composition message and again from the WM_CHAR the default window procedure generates for the same result string
    // Broiler-Human:        PENDING
    protected override void OnNativeWindowMessage(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam)
    {
        base.OnNativeWindowMessage(hwnd, message, wParam, lParam);
        _textInput?.TryHandleMessage(message, wParam, lParam);
    }

    /// <summary>
    /// Creates the platform services once the window exists. They all need the
    /// HWND, which is not available until then.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7EDD26
    // Broiler-Falsified-If: the clipboard or text-input service is bound to a window handle other than this window's NativeHandle, so paste or IME composition acts on another window
    // Broiler-Human:        PENDING
    public void AttachServices(UiSession session, StandardCodeEditor editor)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(editor);

        _session = session;
        _editor = editor;
        _clipboard = new WindowsClipboard(NativeHandle);
        _textInput = new WindowsTextInputService(NativeHandle);
        _textInput.CompositionChanged += OnCompositionChanged;
    }

    public IUiClipboardHost? Clipboard => _clipboard;

    public UiThreadDispatcher Dispatcher => _dispatcher;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=810A91
    // Broiler-Falsified-If: at a display scale other than 100 percent the IME candidate window is placed at the caret's device-independent coordinates rather than its client-pixel coordinates, away from the caret
    // Broiler-Human:        PENDING
    private void OnCompositionChanged(string? text, bool committed)
    {
        _composing = text is not null && !committed;
        Dispatch(_input.FromComposition(text, committed));

        // The candidate window follows the caret. Done after the editor has
        // seen the composition, so the rectangle reflects where the text now
        // is rather than where it was.
        if (_editor is not null)
            _textInput?.SetCaretRectangle(_editor.GetCaretRect());
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=455872
    // Broiler-Falsified-If: work the session queues while handling an input event does not run until a later event or frame
    // Broiler-Human:        PENDING
    private void Dispatch(UiInputEvent input)
    {
        _session?.DispatchInput(input);

        // Anything the UI queued while handling the event runs before the next
        // frame, on this thread.
        _dispatcher.Drain();
        Invalidate();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5F4ACE
    // Broiler-Falsified-If: after the window is disposed its text-input service still raises CompositionChanged into this window
    // Broiler-Human:        PENDING
    protected override void Dispose(bool disposing)
    {
        if (disposing && _textInput is not null)
        {
            _textInput.CompositionChanged -= OnCompositionChanged;
            _textInput.Dispose();
        }

        base.Dispose(disposing);
    }
}
