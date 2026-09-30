// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   54
// Annotated:        54/54
// Exempt:           39
// Human-reviewed:   0/54
// IP risk:          Low
// Security risk:    Critical
// Criteria:         54/54
// Resource impact:  8/10 max
// Unverified:       54
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Broiler.UI;

namespace Broiler.App;

/// <summary>
/// The X11 CLIPBOARD selection, as the UI layer's clipboard port.
///
/// X11 has no clipboard daemon holding a string: the application that copied
/// last *owns* a selection and every paste is a request answered by that owner.
/// So a copy is `XSetSelectionOwner` plus a promise to answer `SelectionRequest`
/// for as long as the process lives, and a paste is `XConvertSelection` followed
/// by a wait for the owner's reply. Both halves need an X connection whose event
/// queue is pumped, which is why <see cref="ProcessPendingEvents"/> exists and
/// why the host loop has to call it.
///
/// The connection is this class's own rather than the renderer's: Xlib is not
/// thread-safe per connection, the renderer's queue belongs to the window, and
/// draining it here would swallow the focus, resize and close events the window
/// is waiting for. A private connection also keeps the whole thing inside the
/// application head, with no change to the graphics component.
///
/// There is deliberately no in-memory fallback. A private string that stands in
/// for the clipboard makes copy and paste appear to work while interoperating
/// with nothing else on the machine — the user copies from Broiler, pastes into
/// a terminal, and gets whatever they copied before. <see cref="TryOpen"/>
/// returns null when there is no X display, and the commands then report
/// themselves unavailable.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=8; Fingerprint=903946
// Broiler-Falsified-If: a property read copies more bytes than Xlib returned for the reported format and item count, reading past the native buffer that holds another X client's data
// Broiler-Human:        PENDING
internal sealed class LinuxX11Clipboard : IUiClipboardHost, IDisposable
{
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=A2346E
    // Broiler-Falsified-If: the value differs from 29, the SelectionClear event code in X.h, so losing a selection to another application is never noticed
    // Broiler-Human:        PENDING
    private const int SelectionClear = 29;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=72784A
    // Broiler-Falsified-If: the value differs from 30, the SelectionRequest event code in X.h, so pastes from other applications go unanswered or another event is answered as one
    // Broiler-Human:        PENDING
    private const int SelectionRequest = 30;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=8DD9B8
    // Broiler-Falsified-If: the value differs from 31, the SelectionNotify event code in X.h, so every paste waits out its timeout and replies reach requestors with the wrong event type
    // Broiler-Human:        PENDING
    private const int SelectionNotify = 31;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=6D775A
    // Broiler-Falsified-If: the value differs from 28, the PropertyNotify event code in X.h, so no INCR chunk is ever read and large pastes time out empty
    // Broiler-Human:        PENDING
    private const int PropertyNotify = 28;

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=3514A3
    // Broiler-Falsified-If: the value differs from 0, the PropertyNewValue state in X.h, so the INCR loop reacts to its own property deletions and ends a transfer on an empty read
    // Broiler-Human:        PENDING
    private const int PropertyNewValue = 0;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=E4CE20
    // Broiler-Falsified-If: the value differs from bit 22, the PropertyChangeMask in X.h, so the window receives no PropertyNotify events and every INCR paste times out
    // Broiler-Human:        PENDING
    private const long PropertyChangeMask = 1L << 22;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=AE7507
    // Broiler-Falsified-If: the value differs from 0, the PropModeReplace mode in X.h, so an answer is appended to whatever the requestor's property already held
    // Broiler-Human:        PENDING
    private const int PropModeReplace = 0;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=32BEDC
    // Broiler-Falsified-If: the value differs from 4, XA_ATOM in Xatom.h, so the TARGETS reply is typed as something other than an atom list and requestors ignore it
    // Broiler-Human:        PENDING
    private const int XaAtom = 4;
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=7BCC6A
    // Broiler-Falsified-If: the value differs from 31, XA_STRING in Xatom.h, so the Latin-1 fallback asks for and answers a different target
    // Broiler-Human:        PENDING
    private const int XaString = 31;

    /// <summary>
    /// How long a paste waits for the owning application to answer. A frozen or
    /// very busy owner must not freeze Broiler with it, and a paste that returns
    /// nothing is recoverable where a hung UI is not.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=92791A
    // Broiler-Falsified-If: a paste from an owner that never sends SelectionNotify holds the calling thread for longer than one second per requested target
    // Broiler-Human:        PENDING
    private static readonly TimeSpan ConvertTimeout = TimeSpan.FromSeconds(1);

    private readonly IntPtr _display;
    private readonly IntPtr _window;
    private readonly IntPtr _clipboard;
    private readonly IntPtr _primary;
    private readonly IntPtr _targets;
    private readonly IntPtr _utf8String;
    private readonly IntPtr _text;
    private readonly IntPtr _incr;
    private readonly IntPtr _transferProperty;
    private string? _ownedText;
    private bool _isDisposed;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=74BCB6
    // Broiler-Falsified-If: an atom field is interned from a different name than the one it stands for, such as the INCR atom from a name other than INCR, so chunked pastes are decoded as their size header
    // Broiler-Human:        PENDING
    private LinuxX11Clipboard(IntPtr display, IntPtr window)
    {
        _display = display;
        _window = window;
        _clipboard = InternAtom(display, "CLIPBOARD");
        _primary = InternAtom(display, "PRIMARY");
        _targets = InternAtom(display, "TARGETS");
        _utf8String = InternAtom(display, "UTF8_STRING");
        _text = InternAtom(display, "TEXT");
        _incr = InternAtom(display, "INCR");
        _transferProperty = InternAtom(display, "BROILER_CLIPBOARD");
    }

    /// <summary>
    /// Connects to the X display and creates the unmapped 1×1 window that owns
    /// the selection — X11 addresses a selection owner by window, so there has to
    /// be one, but it never needs to be seen. Returns null when there is no
    /// display to connect to, which is the signal that this machine has no
    /// clipboard to offer rather than a reason to invent one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=E08E02
    // Broiler-Falsified-If: on a machine without libX11.so.6 or without a reachable display, TryOpen throws instead of returning null
    // Broiler-Human:        PENDING
    public static LinuxX11Clipboard? TryOpen()
    {
        IntPtr display;
        try
        {
            display = XOpenDisplay(IntPtr.Zero);
        }
        catch (DllNotFoundException)
        {
            // No libX11 on the machine at all: a headless or framebuffer-only
            // system. Same answer as a missing display.
            return null;
        }

        if (display == IntPtr.Zero)
            return null;

        int screen = XDefaultScreen(display);
        IntPtr window = XCreateSimpleWindow(display, XRootWindow(display, screen), 0, 0, 1, 1, 0, IntPtr.Zero, IntPtr.Zero);
        if (window == IntPtr.Zero)
        {
            XCloseDisplay(display);
            return null;
        }

        // PropertyChangeMask is what makes an INCR paste possible: the owner
        // signals each chunk by replacing a property on this window.
        XSelectInput(display, window, PropertyChangeMask);
        XFlush(display);
        return new LinuxX11Clipboard(display, window);
    }

    /// <summary>Whether this process currently owns the CLIPBOARD selection.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=32C3E7
    // Broiler-Falsified-If: a disposed instance calls XGetSelectionOwner on its closed display instead of reporting false
    // Broiler-Human:        PENDING
    public bool OwnsClipboard => !_isDisposed && XGetSelectionOwner(_display, _clipboard) == _window;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=D1F473
    // Broiler-Falsified-If: an owner that never answers keeps TryGetText from returning for longer than two ConvertTimeout periods
    // Broiler-Human:        PENDING
    public bool TryGetText(out string text)
    {
        text = string.Empty;
        if (_isDisposed)
            return false;

        IntPtr owner = XGetSelectionOwner(_display, _clipboard);
        if (owner == IntPtr.Zero)
            return false;

        // Asking ourselves would work, but only by round-tripping through the
        // server and our own request handler for a string we are already holding.
        if (owner == _window)
        {
            text = _ownedText ?? string.Empty;
            return text.Length > 0;
        }

        // UTF8_STRING is what everything modern publishes; STRING is Latin-1 and
        // is what an older owner offers instead, so an owner that cannot produce
        // the first is asked for the second before giving up.
        if (TryConvert(_utf8String, Encoding.UTF8, out text) && text.Length > 0)
            return true;
        if (TryConvert(new IntPtr(XaString), Encoding.Latin1, out text) && text.Length > 0)
            return true;

        text = string.Empty;
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=21F32A
    // Broiler-Falsified-If: a copy whose XSetSelectionOwner call does not take ownership leaves the text in place for TryGetText to return as the clipboard
    // Broiler-Human:        PENDING
    public void SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (_isDisposed)
            return;

        _ownedText = text;
        XSetSelectionOwner(_display, _clipboard, _window, IntPtr.Zero);

        // Ownership can be refused, and a copy that did not take must not leave a
        // string behind that TryGetText would serve as though it had.
        if (XGetSelectionOwner(_display, _clipboard) != _window)
        {
            _ownedText = null;
            return;
        }

        // PRIMARY too, so a middle-click paste into a terminal gets the same text
        // that Ctrl+V does. Losing PRIMARY is not worth failing a copy over.
        XSetSelectionOwner(_display, _primary, _window, IntPtr.Zero);
        XFlush(_display);
    }

    /// <summary>
    /// Answers whatever the server has queued for this connection. The host loop
    /// must call this regularly: between calls, another application's paste from
    /// Broiler goes unanswered and appears to it as an empty clipboard.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=494FC7
    // Broiler-Falsified-If: a SelectionRequest already queued when the call starts is still unanswered when it returns
    // Broiler-Human:        PENDING
    public void ProcessPendingEvents()
    {
        if (_isDisposed)
            return;

        while (XPending(_display) > 0)
        {
            XNextEvent(_display, out XEvent nextEvent);
            HandleEvent(ref nextEvent);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CFF845
    // Broiler-Falsified-If: a second Dispose calls XDestroyWindow or XCloseDisplay again on the closed connection
    // Broiler-Human:        PENDING
    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        _ownedText = null;

        // Releasing ownership before the connection closes tells the server the
        // selection is gone now, rather than leaving other applications to
        // discover it when their next paste finds a dead owner.
        if (XGetSelectionOwner(_display, _clipboard) == _window)
            XSetSelectionOwner(_display, _clipboard, IntPtr.Zero, IntPtr.Zero);
        if (XGetSelectionOwner(_display, _primary) == _window)
            XSetSelectionOwner(_display, _primary, IntPtr.Zero, IntPtr.Zero);

        XDestroyWindow(_display, _window);
        XCloseDisplay(_display);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=CD9D0C
    // Broiler-Falsified-If: a SelectionClear for PRIMARY alone discards the text this process still serves as the CLIPBOARD owner
    // Broiler-Human:        PENDING
    private void HandleEvent(ref XEvent nextEvent)
    {
        switch (nextEvent.Type)
        {
            case SelectionRequest:
                AnswerSelectionRequest(ref Unsafe.As<XEvent, XSelectionRequestEvent>(ref nextEvent));
                break;

            case SelectionClear:
                // Another application copied. Ours is no longer the clipboard's
                // content, and holding the string would make a later paste serve
                // a stale one.
                _ownedText = null;
                break;
        }
    }

    // Broiler-AI:           Origin=AI; Spec=ICCCM s2.2; IP=Low; Security=Critical; Resources=4; Fingerprint=51F355
    // Broiler-Falsified-If: a SelectionRequest for a target this process refuses gets no SelectionNotify, leaving the requesting application waiting
    // Broiler-Human:        PENDING
    private void AnswerSelectionRequest(ref XSelectionRequestEvent request)
    {
        // A requestor from before ICCCM names no property and expects the answer
        // under the target's own name.
        IntPtr property = request.Property == IntPtr.Zero ? request.Target : request.Property;
        bool answered = _ownedText is string owned && TryWriteRequestedTarget(ref request, property, owned);

        var reply = new XSelectionEvent
        {
            Type = SelectionNotify,
            Display = request.Display,
            Requestor = request.Requestor,
            Selection = request.Selection,
            Target = request.Target,
            Property = answered ? property : IntPtr.Zero,
            Time = request.Time,
        };

        // Sent through a full-size XEvent rather than the selection event alone:
        // XSendEvent takes the union, and handing it the smaller struct would
        // have it reading past the end of this frame.
        XEvent sendEvent = default;
        Unsafe.As<XEvent, XSelectionEvent>(ref sendEvent) = reply;

        // Every request is answered, refusals included: a requestor blocks until
        // the SelectionNotify arrives, so staying silent hangs the other
        // application rather than telling it we have nothing.
        XSendEvent(_display, request.Requestor, 0, 0, ref sendEvent);
        XFlush(_display);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=D3A55C
    // Broiler-Falsified-If: a text whose encoded form exceeds MaxPropertyBytes is written with one XChangeProperty instead of being refused
    // Broiler-Human:        PENDING
    private bool TryWriteRequestedTarget(ref XSelectionRequestEvent request, IntPtr property, string owned)
    {
        if (request.Target == _targets)
        {
            // What we can convert to, so a requestor can pick before asking.
            IntPtr[] supported = [_targets, _utf8String, new IntPtr(XaString), _text];
            XChangeProperty(_display, request.Requestor, property, new IntPtr(XaAtom), 32, PropModeReplace, supported, supported.Length);
            return true;
        }

        Encoding? encoding = null;
        if (request.Target == _utf8String || request.Target == _text)
            encoding = Encoding.UTF8;
        else if (request.Target == new IntPtr(XaString))
            encoding = Encoding.Latin1;

        if (encoding is null)
            return false;

        byte[] bytes = encoding.GetBytes(owned);

        // Everything goes in one property. A selection too large for a single
        // request would need the chunked INCR protocol on this side too, which
        // is not implemented; refusing is the honest answer, where sending it
        // anyway would be a protocol error and a truncated paste.
        if (bytes.Length > MaxPropertyBytes())
            return false;

        XChangeProperty(_display, request.Requestor, property, request.Target, 8, PropModeReplace, bytes, bytes.Length);
        return true;
    }

    /// <summary>
    /// What one request can carry, less the room the request header itself
    /// needs. The server reports it in 4-byte units, and the extended limit is
    /// the far larger one negotiated by the BIG-REQUESTS extension.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CA31DB
    // Broiler-Falsified-If: the returned limit plus the XChangeProperty request header exceeds the server's maximum request length, so an answer of exactly that size draws a BadLength error
    // Broiler-Human:        PENDING
    private long MaxPropertyBytes()
    {
        long units = XExtendedMaxRequestSize(_display);
        if (units <= 0)
            units = XMaxRequestSize(_display);

        return Math.Max(0, (units * 4) - 1024);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=8; Fingerprint=1A03B7
    // Broiler-Falsified-If: an INCR reply is returned as the text of its size header instead of being read in chunks
    // Broiler-Human:        PENDING
    private bool TryConvert(IntPtr target, Encoding encoding, out string text)
    {
        text = string.Empty;
        XDeleteProperty(_display, _window, _transferProperty);
        XConvertSelection(_display, _clipboard, target, _transferProperty, _window, IntPtr.Zero);
        XFlush(_display);

        if (!TryWaitForSelectionNotify(target, out bool refused) || refused)
            return false;

        if (!TryReadProperty(delete: true, out byte[] data, out IntPtr type))
            return false;

        // A payload too large for one property arrives as a byte count and a
        // promise, with the data following in chunks.
        if (type == _incr)
            return TryReadIncrementally(encoding, out text);

        text = encoding.GetString(data);
        return true;
    }

    /// <summary>
    /// Pumps this connection until the owner answers or the timeout expires.
    /// Other events keep being handled meanwhile — an application we are pasting
    /// from may be pasting from us at the same moment, and ignoring its request
    /// until ours completes would deadlock the pair.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=BAC71B
    // Broiler-Falsified-If: a SelectionNotify addressed to another requestor window or for another target ends the wait as though the owner had answered this conversion
    // Broiler-Human:        PENDING
    private bool TryWaitForSelectionNotify(IntPtr target, out bool refused)
    {
        refused = false;
        long deadline = Deadline();
        while (Stopwatch.GetTimestamp() < deadline)
        {
            if (XPending(_display) == 0)
            {
                Thread.Sleep(1);
                continue;
            }

            XNextEvent(_display, out XEvent nextEvent);
            if (nextEvent.Type != SelectionNotify)
            {
                HandleEvent(ref nextEvent);
                continue;
            }

            ref XSelectionEvent notify = ref Unsafe.As<XEvent, XSelectionEvent>(ref nextEvent);
            if (notify.Requestor != _window || notify.Target != target)
                continue;

            // A property of None is the owner saying it cannot produce this
            // target, which is a refusal rather than a failure to answer.
            refused = notify.Property == IntPtr.Zero;
            return true;
        }

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=8; Fingerprint=E0C149
    // Broiler-Falsified-If: an owner that sends a one-byte chunk every 900 ms keeps the loop running on the calling thread with no limit on total time or buffered bytes
    // Broiler-Human:        PENDING
    private bool TryReadIncrementally(Encoding encoding, out string text)
    {
        text = string.Empty;
        using var buffer = new MemoryStream();

        // The deadline guards against an owner that stopped answering, not
        // against a large payload, so every chunk that arrives extends it.
        long deadline = Deadline();
        while (Stopwatch.GetTimestamp() < deadline)
        {
            if (XPending(_display) == 0)
            {
                Thread.Sleep(1);
                continue;
            }

            XNextEvent(_display, out XEvent nextEvent);
            if (nextEvent.Type != PropertyNotify)
            {
                HandleEvent(ref nextEvent);
                continue;
            }

            ref XPropertyEvent property = ref Unsafe.As<XEvent, XPropertyEvent>(ref nextEvent);
            if (property.Window != _window || property.Atom != _transferProperty || property.State != PropertyNewValue)
                continue;

            // Deleting the property is the acknowledgement that moves the owner
            // on to the next chunk; a zero-length one ends the transfer.
            if (!TryReadProperty(delete: true, out byte[] chunk, out _))
                return false;
            if (chunk.Length == 0)
            {
                text = encoding.GetString(buffer.ToArray());
                return true;
            }

            buffer.Write(chunk, 0, chunk.Length);
            deadline = Deadline();
        }

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=535EA5
    // Broiler-Falsified-If: the returned timestamp lies further than ConvertTimeout from the current Stopwatch timestamp, for example because TimeSpan ticks are mixed with Stopwatch ticks
    // Broiler-Human:        PENDING
    private static long Deadline() =>
        Stopwatch.GetTimestamp() + (long)(ConvertTimeout.TotalSeconds * Stopwatch.Frequency);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=7; Fingerprint=02D2D8
    // Broiler-Falsified-If: Marshal.Copy copies more bytes than the item count times the element size Xlib allocates for the returned format, which is 1, 2 or sizeof(long) bytes
    // Broiler-Human:        PENDING
    private bool TryReadProperty(bool delete, out byte[] data, out IntPtr type)
    {
        data = [];
        type = IntPtr.Zero;

        // Read with a zero length first: that reports the size without
        // transferring anything, so the real read asks for exactly what is there.
        if (XGetWindowProperty(
                _display,
                _window,
                _transferProperty,
                IntPtr.Zero,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                out type,
                out int format,
                out IntPtr items,
                out IntPtr remaining,
                out IntPtr peek) != 0)
        {
            return false;
        }

        if (peek != IntPtr.Zero)
            XFree(peek);
        if (type == IntPtr.Zero)
            return false;

        long bytes = (long)remaining;
        // XGetWindowProperty counts its length in 32-bit units.
        var length = new IntPtr((bytes + 3) / 4);
        if (XGetWindowProperty(
                _display,
                _window,
                _transferProperty,
                IntPtr.Zero,
                length,
                delete ? 1 : 0,
                IntPtr.Zero,
                out type,
                out format,
                out items,
                out remaining,
                out IntPtr value) != 0)
        {
            return false;
        }

        if (value == IntPtr.Zero)
            return type != IntPtr.Zero;

        try
        {
            // Xlib's "32-bit format" hands back an array of C long, which is 64
            // bits here — the one place the wire format and the client array
            // disagree. Text arrives as format 8, so this only guards the
            // property reads that are not text.
            long count = format switch
            {
                32 => (long)items * IntPtr.Size,
                16 => (long)items * 2,
                _ => (long)items,
            };
            if (count <= 0)
                return true;

            data = new byte[count];
            Marshal.Copy(value, data, 0, data.Length);
            return true;
        }
        finally
        {
            XFree(value);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=049CB1
    // Broiler-Falsified-If: an atom is interned with only-if-exists set, so a name the server has not seen before comes back as None
    // Broiler-Human:        PENDING
    private static IntPtr InternAtom(IntPtr display, string name) => XInternAtom(display, name, 0);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=0; Fingerprint=B2D302
    // Broiler-Falsified-If: the struct is smaller than the 192-byte XEvent union Xlib uses on linux-x64, so XNextEvent writes past it
    // Broiler-Human:        PENDING
    [StructLayout(LayoutKind.Sequential, Size = 192)]
    private struct XEvent
    {
        public int Type;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=76C4EB
    // Broiler-Falsified-If: a field offset differs from Xlib's XSelectionRequestEvent on linux-x64, for example Requestor not at byte 40, so replies are sent to the wrong window
    // Broiler-Human:        PENDING
    [StructLayout(LayoutKind.Sequential)]
    private struct XSelectionRequestEvent
    {
        public int Type;
        public IntPtr Serial;
        public int SendEvent;
        public IntPtr Display;
        public IntPtr Owner;
        public IntPtr Requestor;
        public IntPtr Selection;
        public IntPtr Target;
        public IntPtr Property;
        public IntPtr Time;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=5D0165
    // Broiler-Falsified-If: a field offset differs from Xlib's XSelectionEvent on linux-x64, for example Property not at byte 56, so a refusal is read as an answer
    // Broiler-Human:        PENDING
    [StructLayout(LayoutKind.Sequential)]
    private struct XSelectionEvent
    {
        public int Type;
        public IntPtr Serial;
        public int SendEvent;
        public IntPtr Display;
        public IntPtr Requestor;
        public IntPtr Selection;
        public IntPtr Target;
        public IntPtr Property;
        public IntPtr Time;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=F95034
    // Broiler-Falsified-If: a field offset differs from Xlib's XPropertyEvent on linux-x64, for example State not at byte 56, so INCR chunk notifications are misread
    // Broiler-Human:        PENDING
    [StructLayout(LayoutKind.Sequential)]
    private struct XPropertyEvent
    {
        public int Type;
        public IntPtr Serial;
        public int SendEvent;
        public IntPtr Display;
        public IntPtr Window;
        public IntPtr Atom;
        public IntPtr Time;
        public int State;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=C11F35
    // Broiler-Falsified-If: the Display pointer is returned through a type narrower than IntPtr, so a 64-bit connection address is truncated
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XOpenDisplay")]
    private static extern IntPtr XOpenDisplay(IntPtr name);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=09A14C
    // Broiler-Falsified-If: the display parameter is declared narrower than IntPtr, so Dispose closes a truncated pointer instead of its connection
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XCloseDisplay")]
    private static extern int XCloseDisplay(IntPtr display);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=AA3524
    // Broiler-Falsified-If: the screen number is returned through a type other than a 32-bit int, so XRootWindow is asked for a screen that does not exist
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XDefaultScreen")]
    private static extern int XDefaultScreen(IntPtr display);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=7A72F9
    // Broiler-Falsified-If: the Window result is declared narrower than the 64-bit XID Xlib returns on linux-x64, so the selection window is created under a truncated parent
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XRootWindow")]
    private static extern IntPtr XRootWindow(IntPtr display, int screen);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=2926C1
    // Broiler-Falsified-If: a parameter's type or position differs from the XCreateSimpleWindow prototype in Xlib.h, for example border and background declared narrower than unsigned long
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XCreateSimpleWindow")]
    private static extern IntPtr XCreateSimpleWindow(
        IntPtr display,
        IntPtr parent,
        int x,
        int y,
        uint width,
        uint height,
        uint borderWidth,
        IntPtr border,
        IntPtr background);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=1F41FA
    // Broiler-Falsified-If: the window parameter is declared narrower than IntPtr, so Dispose destroys a different window id
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XDestroyWindow")]
    private static extern int XDestroyWindow(IntPtr display, IntPtr window);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=E92C2A
    // Broiler-Falsified-If: the event mask is declared as a type other than the 64-bit C long Xlib takes on linux-x64, so PropertyChangeMask does not reach the server
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XSelectInput")]
    private static extern int XSelectInput(IntPtr display, IntPtr window, long mask);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=927623
    // Broiler-Falsified-If: the atom name is marshalled as UTF-16 rather than a NUL-terminated byte string, so every atom is interned under a truncated name
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XInternAtom")]
    private static extern IntPtr XInternAtom(IntPtr display, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int onlyIfExists);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=54D4C2
    // Broiler-Falsified-If: a parameter is declared in a different order from Xlib's selection, owner, time, so ownership is claimed for the wrong selection
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XSetSelectionOwner")]
    private static extern int XSetSelectionOwner(IntPtr display, IntPtr selection, IntPtr owner, IntPtr time);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=83913D
    // Broiler-Falsified-If: the returned Window is declared narrower than IntPtr, so the comparison with this process's own window never matches
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XGetSelectionOwner")]
    private static extern IntPtr XGetSelectionOwner(IntPtr display, IntPtr selection);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=59DB00
    // Broiler-Falsified-If: a parameter is declared in a different order from Xlib's selection, target, property, requestor, time, so the owner writes its answer to the wrong property
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XConvertSelection")]
    private static extern int XConvertSelection(IntPtr display, IntPtr selection, IntPtr target, IntPtr property, IntPtr requestor, IntPtr time);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=A69735
    // Broiler-Falsified-If: the element count is declared as a type other than the 32-bit int Xlib takes, so the byte array is sent with a length it does not have and Xlib reads past it
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XChangeProperty")]
    private static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, int mode, byte[] data, int elements);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=AAF4A5
    // Broiler-Falsified-If: the array's elements are narrower than a C long, so a format-32 write of the TARGETS list reads past the array
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XChangeProperty")]
    private static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, int mode, IntPtr[] data, int elements);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=93C844
    // Broiler-Falsified-If: the property atom is declared narrower than IntPtr, so a different property is deleted and a stale transfer is read as the answer
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XDeleteProperty")]
    private static extern int XDeleteProperty(IntPtr display, IntPtr window, IntPtr property);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=7; Fingerprint=F503CE
    // Broiler-Falsified-If: an out parameter is declared narrower than what Xlib writes through it, for example nitems or bytes_after as int, so Xlib overwrites the caller's stack
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XGetWindowProperty")]
    private static extern int XGetWindowProperty(
        IntPtr display,
        IntPtr window,
        IntPtr property,
        IntPtr offset,
        IntPtr length,
        int delete,
        IntPtr requestedType,
        out IntPtr actualType,
        out int actualFormat,
        out IntPtr items,
        out IntPtr bytesAfter,
        out IntPtr value);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=B581CA
    // Broiler-Falsified-If: the event is passed by value or as a struct smaller than XEvent, so Xlib reads past the caller's event
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XSendEvent")]
    private static extern int XSendEvent(IntPtr display, IntPtr window, int propagate, long mask, ref XEvent sendEvent);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=A35B94
    // Broiler-Falsified-If: the return is declared as a type other than the 64-bit C long Xlib returns on linux-x64, so the request limit used to refuse oversized answers is garbage
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XMaxRequestSize")]
    private static extern long XMaxRequestSize(IntPtr display);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=48608A
    // Broiler-Falsified-If: the return is declared as a type other than the 64-bit C long Xlib returns on linux-x64, so the BIG-REQUESTS limit used to refuse oversized answers is garbage
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XExtendedMaxRequestSize")]
    private static extern long XExtendedMaxRequestSize(IntPtr display);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=C5AF79
    // Broiler-Falsified-If: the import binds to an entry other than XPending, so the event loops run on a count that does not reflect the queue
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XPending")]
    private static extern int XPending(IntPtr display);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=0AFABC
    // Broiler-Falsified-If: the event is received by value or into a struct smaller than the 192-byte XEvent union, so Xlib writes past the caller's storage
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XNextEvent")]
    private static extern int XNextEvent(IntPtr display, out XEvent nextEvent);

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=2B1605
    // Broiler-Falsified-If: the import binds to an entry other than XFlush, so replies and ownership changes stay in the output buffer and other applications wait on them
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XFlush")]
    private static extern int XFlush(IntPtr display);

    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=09B78A
    // Broiler-Falsified-If: the import binds to an entry other than the XFree of libX11, so buffers Xlib allocated are released by a different allocator
    // Broiler-Human:        PENDING
    [DllImport("libX11.so.6", EntryPoint = "XFree")]
    private static extern int XFree(IntPtr data);
}
