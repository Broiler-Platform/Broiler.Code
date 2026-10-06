// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           16
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    High
// Criteria:         13/6
// Resource impact:  8/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Threading;
using System.Threading.Tasks;
using Broiler.App;
using Broiler.Code.Core.Hosting;
using Broiler.Code.Core.Shell;
using Broiler.Graphics;
using Broiler.Graphics.Color;
using Broiler.Graphics.Geometry;
using Broiler.Graphics.Linux.OpenGL;
using Broiler.Graphics.Rendering;
using Broiler.Graphics.RenderList;
using Broiler.UI;

namespace Broiler.Code.Linux;

/// <summary>
/// The X11/OpenGL host for Broiler Code.
///
/// It differs from the Windows head in one structural way: there is no message
/// loop to be called back from. X11 events are polled, evdev events arrive on
/// device threads, and nothing schedules this window. So the loop below <em>is</em>
/// the UI thread — it pumps window events, drains input, drains the dispatcher,
/// and renders, in that order, and everything the analysis layers post is
/// executed by it rather than on the thread that produced it.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=27C58D
// Broiler-Falsified-If: without --ignore-focus, keys typed after the window lost focus keep reaching the editor on later frames because the evdev devices stay active
// Broiler-Human:        PENDING
internal sealed class CodeWindow : IUiHost, IUiClipboardHost, IAsyncDisposable
{
    // Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=92070B
    // Broiler-Human:        PENDING
    private static readonly BRenderOptions RenderOptions =
        new(Antialias: true, VSync: true, SubpixelText: true);

    private readonly LinuxOpenGlRenderer _renderer = new();
    private readonly LinuxOpenGlX11WindowSurface _surface;
    private readonly UiThreadDispatcher _dispatcher;
    private readonly EvdevInputRouter _router;
    private readonly LinuxCodeInput _input;
    private readonly TimeSpan _pollInterval;
    private readonly bool _ignoreFocus;

    /// <summary>
    /// The X11 selection, on its own connection. Null where there is no display
    /// to own one on, and then the clipboard commands report themselves
    /// unavailable rather than being backed by a buffer only this process sees.
    /// </summary>
    private readonly LinuxX11Clipboard? _clipboard = LinuxX11Clipboard.TryOpen();
    private CodeShell? _shell;
    private UiSession? _session;
    private UiElement? _editor;
    private long _frameIndex;
    private bool _invalidated = true;
    private bool _disposed;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=3DE714
    // Broiler-Falsified-If: a pollMilliseconds of zero or less reaches PeriodicTimer as a period below one millisecond
    // Broiler-Human:        PENDING
    public CodeWindow(BSize size, Action<string> log, bool ignoreFocus = false, int pollMilliseconds = 16)
    {
        ArgumentNullException.ThrowIfNull(log);

        _surface = _renderer.CreateX11WindowSurface(
            BSurfaceDescriptor.Default(size), "Broiler Code");
        _router = new EvdevInputRouter(size);

        // Constructed on the thread that will run the loop, so CheckAccess
        // answers for the thread that actually drains it.
        _dispatcher = new UiThreadDispatcher(() => _invalidated = true);
        _input = new LinuxCodeInput(_router, log);
        _pollInterval = TimeSpan.FromMilliseconds(Math.Max(1, pollMilliseconds));
        _ignoreFocus = ignoreFocus;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3B37C9
    // Broiler-Human:        PENDING
    public BSize ViewportSize => _surface.Size;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=537D40
    // Broiler-Human:        PENDING
    public double Scale => _surface.DpiScale;

    public UiThreadDispatcher Dispatcher => _dispatcher;

    public LinuxCodeInput Input => _input;

    /// <summary>Whether this host has a real clipboard to offer.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E985CC
    // Broiler-Falsified-If: HasClipboard is true while the X11 clipboard failed to open
    // Broiler-Human:        PENDING
    public bool HasClipboard => _clipboard is not null;

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=516BFD
    // Broiler-Human:        PENDING
    public BRenderList CreateRenderList(int capacity = 0) => new(capacity);

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=B03E24
    // Broiler-Falsified-If: with no X11 clipboard open, TryGetText returns true or a text other than the empty string
    // Broiler-Human:        PENDING
    public bool TryGetText(out string text)
    {
        if (_clipboard is not null)
            return _clipboard.TryGetText(out text);

        text = string.Empty;
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B5EF0D
    // Broiler-Falsified-If: on a host with no X11 clipboard, SetText throws instead of dropping the text
    // Broiler-Human:        PENDING
    public void SetText(string text) => _clipboard?.SetText(text);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7938F2
    // Broiler-Human:        PENDING
    public void Invalidate(UiInvalidation invalidation) => _invalidated = true;

    /// <summary>
    /// Presentation belongs to the loop, which renders as part of its own frame.
    /// Submitting here as well would draw the same frame twice.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F7DD0A
    // Broiler-Falsified-If: Present hands the render list to the renderer, so a frame is drawn twice
    // Broiler-Human:        PENDING
    public void Present(BRenderList renderList) => ArgumentNullException.ThrowIfNull(renderList);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=82DB69
    // Broiler-Falsified-If: a null session or shell is accepted, and RunAsync later fails with NullReferenceException
    // Broiler-Human:        PENDING
    public void Attach(UiSession session, CodeShell shell)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(shell);

        _session = session;
        _shell = shell;
        _editor = shell.Editor;
    }

    /// <summary>Gives the editor keyboard focus, for the first frame.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=05D25C
    // Broiler-Falsified-If: after FocusEditor the session's focused element is not the shell's editor
    // Broiler-Human:        PENDING
    public void FocusEditor()
    {
        if (_editor is not null)
            SetFocus(_editor);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=B2CC6F
    // Broiler-Falsified-If: after WaitForNextTickAsync resumes on a thread-pool thread, the next Drain runs off the thread that constructed the dispatcher and throws InvalidOperationException
    // Broiler-Human:        PENDING
    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        if (_session is null)
            throw new InvalidOperationException("Attach a session before running the window.");

        await _input.StartAsync(cancellationToken).ConfigureAwait(false);

        using var timer = new PeriodicTimer(_pollInterval);
        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            bool windowEvents = _surface.ProcessPendingEvents();

            // An X11 copy is a promise to answer requests for as long as this
            // process owns the selection: between these calls, another
            // application pasting from Code sees an empty clipboard.
            _clipboard?.ProcessPendingEvents();
            _router.SetViewport(_surface.Size);

            // Devices are only read while the window has focus, so typing into
            // another application does not also type in here. evdev is a global
            // device stream; nothing scopes it to a window but this.
            await _input.SetActiveAsync(_ignoreFocus || _surface.IsFocused, cancellationToken)
                .ConfigureAwait(false);

            // X11 knows where the real cursor is, so its position wins over
            // accumulated relative motion where both exist.
            if (_surface.TryGetPointerPosition(out int pointerX, out int pointerY, out bool inside) && inside)
            {
                double scale = Scale <= 0 ? 1 : Scale;
                _input.SetAbsolutePointer(pointerX / scale, pointerY / scale);
            }

            _input.Drain(DispatchInput);
            _dispatcher.Drain();

            if (_invalidated || windowEvents)
                RenderFrame();
        }
        while (!_surface.IsCloseRequested &&
               !_input.QuitRequested &&
               await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false));

        await _input.SetActiveAsync(false, cancellationToken).ConfigureAwait(false);
        return 0;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3A89A2
    // Broiler-Falsified-If: a second DisposeAsync disposes the X11 surface or the clipboard connection again
    // Broiler-Human:        PENDING
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        await _input.DisposeAsync().ConfigureAwait(false);

        // Before the connection goes: releasing the selection tells other
        // applications the clipboard is gone now rather than leaving them to
        // find a dead owner at their next paste.
        _clipboard?.Dispose();
        _surface.Dispose();
        _renderer.Dispose();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=8; Fingerprint=19E6AD
    // Broiler-Falsified-If: a pointer press moves focus after the session has dispatched it, so the click is handled by the control that had focus before
    // Broiler-Human:        PENDING
    private void DispatchInput(UiInputEvent input)
    {
        // Focus follows the press, before the event is dispatched, so the click
        // that moves the caret is handled by the control that now has focus.
        if (input.Kind == UiInputEventKind.PointerButton && _session is not null && _shell is not null)
        {
            UiElement? target = _shell.ResolveFocusTarget(_session.HitTest(input.Position));
            if (target is not null)
                SetFocus(target);
        }

        _session?.DispatchInput(input);
        _invalidated = true;
    }

    /// <summary>
    /// Moves session focus, and tells the editor whether it has it. The session
    /// decides where keys go; the control only draws the caret, so both have to
    /// be told or the caret and the keystrokes disagree.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=ACF630
    // Broiler-Falsified-If: moving focus to a control other than the editor leaves the editor's HasFocus true, so its caret still draws
    // Broiler-Human:        PENDING
    private void SetFocus(UiElement target)
    {
        if (_session is null || ReferenceEquals(_session.FocusedElement, target))
            return;

        _session.SetFocus(target);
        if (_shell is not null)
            _shell.Editor.HasFocus = ReferenceEquals(target, _shell.Editor);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=7; Fingerprint=C748F5
    // Broiler-Falsified-If: RenderFrame clears the invalidation flag without having rendered, so a pending change is not painted
    // Broiler-Human:        PENDING
    private void RenderFrame()
    {
        if (_session is null)
            return;

        BRenderList renderList = _session.RenderFrame();
        _renderer.Render(
            _surface, renderList, new BFrameContext(BColor.White, _frameIndex++, RenderOptions));
        _invalidated = false;
    }
}
