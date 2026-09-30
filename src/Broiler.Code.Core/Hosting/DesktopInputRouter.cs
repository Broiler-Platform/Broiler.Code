// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           4
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    High
// Criteria:         14/2
// Resource impact:  1/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Threading;
using Broiler.Graphics;
using Broiler.Graphics.Windowing;
using Broiler.Input;
using Broiler.Input.Keyboard;
using Broiler.Input.Mouse;
using Broiler.Input.Text;
using Broiler.UI;

namespace Broiler.Code.Core.Hosting;

/// <summary>
/// Turns a graphics window's events into explicit Broiler.Input events.
///
/// The Writer heads use <c>StandardLegacyGraphicsInputAdapter</c>, which is
/// marked obsolete and which they suppress the warning for. Code routes
/// explicitly instead, and the difference is not cosmetic: this assigns a real
/// device identity, a monotonic sequence, and a named clock to every event, so
/// a consumer can order events and tell devices apart. Pen and touch work later
/// in the roadmap needs exactly that.
///
/// It lives in Core rather than in each head so the two heads share one
/// translation. A copy in each is what the Phase 2 architecture test forbids.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=062099
// Broiler-Falsified-If: two events built concurrently by one router carry the same sequence number
// Broiler-Human:        PENDING
public sealed class DesktopInputRouter
{
    private readonly InputDeviceId _pointerDevice;
    private readonly InputDeviceId _keyboardDevice;
    private readonly string _clockName;
    private long _sequence;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F88592
    // Broiler-Falsified-If: the pointer and keyboard device identities built from one host name compare equal
    // Broiler-Human:        PENDING
    public DesktopInputRouter(string hostName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hostName);
        _pointerDevice = InputDeviceId.FromOpaqueValue($"{hostName}:pointer");
        _keyboardDevice = InputDeviceId.FromOpaqueValue($"{hostName}:keyboard");
        _clockName = hostName;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=278214
    // Broiler-Falsified-If: a button release is emitted with MouseButtonTransition.Down
    // Broiler-Human:        PENDING
    public UiInputEvent FromPointerButton(BPointerEventArgs args, bool pressed) =>
        UiInputEvent.FromMouseButton(new MouseButtonEvent(
            Header(_pointerDevice),
            InputPoint.ClientDeviceIndependentPixels(args.Position.X, args.Position.Y),
            MapButtons(args.Buttons),
            MapButton(args.ChangedButton),
            pressed ? MouseButtonTransition.Down : MouseButtonTransition.Up,
            InputEventSource.Raw));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3FDFE9
    // Broiler-Falsified-If: a move is emitted at a position other than the window event's position
    // Broiler-Human:        PENDING
    public UiInputEvent FromPointerMove(BPointerEventArgs args) =>
        UiInputEvent.FromMouseMove(new MouseMoveEvent(
            Header(_pointerDevice),
            InputPoint.ClientDeviceIndependentPixels(args.Position.X, args.Position.Y),
            MapButtons(args.Buttons),
            InputEventSource.Raw));

    /// <summary>
    /// A wheel notch, with the axis it turned on and the modifiers held while it
    /// did.
    ///
    /// Both matter and both used to be dropped here. A control cannot tell a
    /// sideways scroll from a downwards one without the axis, and cannot honour
    /// shift-and-wheel — which is how every editor scrolls sideways on a mouse
    /// with one wheel — without the modifiers.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=391CB6
    // Broiler-Falsified-If: a horizontal wheel notch is emitted on MouseWheelAxis.Vertical
    // Broiler-Human:        PENDING
    public UiInputEvent FromWheel(BMouseWheelEventArgs args) =>
        UiInputEvent.FromMouseWheel(new MouseWheelEvent(
            Header(_pointerDevice),
            InputPoint.ClientDeviceIndependentPixels(args.Position.X, args.Position.Y),
            MapButtons(args.Buttons),
            args.IsHorizontal ? MouseWheelAxis.Horizontal : MouseWheelAxis.Vertical,
            args.Delta,
            InputEventSource.Raw,
            WheelModifiers(args)));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4B61A5
    // Broiler-Falsified-If: a wheel notch turned with Shift held produces modifiers without Shift
    // Broiler-Human:        PENDING
    private static InputModifiers WheelModifiers(BMouseWheelEventArgs args)
    {
        InputModifiers modifiers = InputModifiers.None;
        if (args.Control)
            modifiers |= InputModifiers.Control;
        if (args.Shift)
            modifiers |= InputModifiers.Shift;
        if (args.Alt)
            modifiers |= InputModifiers.Alt;

        return modifiers;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5F378B
    // Broiler-Falsified-If: a key release is emitted with KeyboardKeyTransition.Down
    // Broiler-Human:        PENDING
    public UiInputEvent FromKey(BKeyEventArgs args, bool pressed) =>
        UiInputEvent.FromKeyboardKey(new KeyboardKeyEvent(
            Header(_keyboardDevice),
            KeyboardKey.FromName(MapVirtualKey(args.VirtualKey)),
            pressed ? KeyboardKeyTransition.Down : KeyboardKeyTransition.Up,
            MapModifiers(args),
            args.VirtualKey,
            0,
            0,
            false,
            false,
            Source: InputEventSource.Raw));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=50108C
    // Broiler-Falsified-If: the emitted text input carries the pointer device identity instead of the keyboard one
    // Broiler-Human:        PENDING
    public UiInputEvent FromText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return UiInputEvent.FromTextInput(
            new TextInputEvent(Header(_keyboardDevice), text, InputEventSource.Raw));
    }

    /// <summary>
    /// An IME composition update. A null <paramref name="text"/> means the
    /// composition ended without a commit, which the editor has to treat as a
    /// cancellation rather than as committing an empty string.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D4E10B
    // Broiler-Falsified-If: a null composition text is emitted as Committed or Updated rather than Cancelled
    // Broiler-Human:        PENDING
    public UiInputEvent FromComposition(string? text, bool committed) =>
        UiInputEvent.FromTextComposition(new TextCompositionEvent(
            Header(_keyboardDevice),
            text ?? string.Empty,
            text is null
                ? TextCompositionState.Cancelled
                : committed ? TextCompositionState.Committed : TextCompositionState.Updated,
            Source: InputEventSource.Raw));

    /// <summary>
    /// Maps a platform virtual-key code to the neutral key name the controls
    /// switch on. Explicit and small on purpose: the legacy adapter's mapping is
    /// opaque, and a key that silently arrives under an unexpected name is a
    /// shortcut that does nothing with no way to see why.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C69D01
    // Broiler-Falsified-If: a virtual-key code between 0x70 and 0x87 maps to a name other than F1 through F24
    // Broiler-Human:        PENDING
    public static string MapVirtualKey(int virtualKey) => virtualKey switch
    {
        BVirtualKey.Back => "Backspace",
        BVirtualKey.Tab => "Tab",
        BVirtualKey.Enter => "Enter",
        BVirtualKey.Escape => "Escape",
        BVirtualKey.Space => "Space",
        BVirtualKey.PageUp => "PageUp",
        BVirtualKey.PageDown => "PageDown",
        BVirtualKey.End => "End",
        BVirtualKey.Home => "Home",
        BVirtualKey.Left => "ArrowLeft",
        BVirtualKey.Up => "ArrowUp",
        BVirtualKey.Right => "ArrowRight",
        BVirtualKey.Down => "ArrowDown",
        0x2E => "Delete",
        >= 0x30 and <= 0x39 => ((char)virtualKey).ToString(),
        >= 0x41 and <= 0x5A => ((char)virtualKey).ToString(),
        >= 0x70 and <= 0x87 => $"F{virtualKey - 0x6F}",
        _ => $"VK{virtualKey}",
    };

    /// <summary>
    /// A monotonic header per event. The sequence is what lets a consumer order
    /// two events from one device; without it they are indistinguishable.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=695AF6
    // Broiler-Falsified-If: two events built concurrently by one router receive the same sequence number, or a later event a smaller one
    // Broiler-Human:        PENDING
    private InputEventHeader Header(InputDeviceId device) => new(
        device,
        new InputTimestamp(Environment.TickCount64, 1000, _clockName),
        Interlocked.Increment(ref _sequence));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D7F0BC
    // Broiler-Falsified-If: a change of the middle button maps to a button other than MouseButton.Middle
    // Broiler-Human:        PENDING
    private static MouseButton MapButton(BMouseButtons button) => button switch
    {
        BMouseButtons.Right => MouseButton.Right,
        BMouseButtons.Middle => MouseButton.Middle,
        BMouseButtons.Left => MouseButton.Left,
        _ => MouseButton.None,
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E57B7B
    // Broiler-Falsified-If: the left and right buttons held together map to a set missing one of them
    // Broiler-Human:        PENDING
    private static MouseButtons MapButtons(BMouseButtons buttons)
    {
        MouseButtons mapped = MouseButtons.None;
        if ((buttons & BMouseButtons.Left) != 0)
            mapped |= MouseButtons.Left;
        if ((buttons & BMouseButtons.Right) != 0)
            mapped |= MouseButtons.Right;
        if ((buttons & BMouseButtons.Middle) != 0)
            mapped |= MouseButtons.Middle;
        return mapped;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=090AD1
    // Broiler-Falsified-If: a key pressed with Control held produces modifiers without Control
    // Broiler-Human:        PENDING
    private static KeyboardModifierState MapModifiers(BKeyEventArgs args)
    {
        KeyboardModifierState modifiers = KeyboardModifierState.None;
        if (args.Shift)
            modifiers |= KeyboardModifierState.Shift;
        if (args.Control)
            modifiers |= KeyboardModifierState.Control;
        if (args.Alt)
            modifiers |= KeyboardModifierState.Alt;
        return modifiers;
    }
}
