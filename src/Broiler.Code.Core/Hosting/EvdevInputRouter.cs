// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           7
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    High
// Criteria:         13/7
// Resource impact:  1/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.Graphics;
using Broiler.Graphics.Geometry;
using Broiler.Input;
using Broiler.Input.Keyboard;
using Broiler.Input.Mouse;
using Broiler.Input.Text;
using Broiler.UI;

namespace Broiler.Code.Core.Hosting;

/// <summary>
/// Turns raw evdev device events into UI input.
///
/// evdev is not a windowing input stack. It reports what a device did — this key
/// went down, the pointer moved this far — with no notion of a window, a cursor
/// position, or a character. Three things therefore have to happen here that the
/// Windows head gets from the OS:
///
/// <list type="bullet">
/// <item>relative motion is accumulated into an absolute position and clamped to
/// the viewport, because nothing else is tracking where the pointer is;</item>
/// <item>key names are normalised to the names the controls switch on, since the
/// Linux providers spell them <c>KeyA</c> and <c>ArrowLeft</c>;</item>
/// <item>characters are derived from keys, because evdev carries no text.</item>
/// </list>
///
/// That last one is also the boundary of what this can do: a US-layout mapping
/// is not an input method. It handles typing Latin text and nothing else, which
/// is exactly why the Linux head reports its IME as unavailable rather than
/// claiming this is one.
///
/// It lives in Core rather than the head so it can be tested on any operating
/// system — none of it needs a device, a display, or Linux.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=54F756
// Broiler-Falsified-If: pointer updates from two threads interleave so that PointerPosition reports a coordinate outside the current viewport
// Broiler-Human:        PENDING
public sealed class EvdevInputRouter
{
    private readonly object _gate = new();
    private BSize _viewport;
    private double _pointerX;
    private double _pointerY;
    private bool _pointerPlaced;
    private MouseButtons _buttons;

    public EvdevInputRouter(BSize viewport) => _viewport = viewport;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=58D12E
    // Broiler-Falsified-If: a read during a concurrent move returns the X of one update together with the Y of another
    // Broiler-Human:        PENDING
    public BPoint PointerPosition
    {
        get
        {
            lock (_gate)
                return new BPoint(_pointerX, _pointerY);
        }
    }

    /// <summary>
    /// The viewport the pointer is confined to. The pointer starts centred, and
    /// afterwards a resize only clamps it — recentring on every resize would
    /// move the cursor out from under the user's hand.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=1420B9
    // Broiler-Falsified-If: after the pointer has been placed, a resize to a smaller viewport leaves it beyond the new width or height minus one
    // Broiler-Human:        PENDING
    public void SetViewport(BSize viewport)
    {
        if (viewport.Width <= 0 || viewport.Height <= 0)
            return;

        lock (_gate)
        {
            _viewport = viewport;
            if (!_pointerPlaced)
            {
                _pointerX = viewport.Width / 2;
                _pointerY = viewport.Height / 2;
                _pointerPlaced = true;
                return;
            }

            _pointerX = Clamp(_pointerX, viewport.Width);
            _pointerY = Clamp(_pointerY, viewport.Height);
        }
    }

    /// <summary>
    /// An absolute position from the window server, which knows where the real
    /// cursor is. Preferred over accumulated motion when it is available: it
    /// keeps this pointer and the X11 cursor the user can see in agreement.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AD971D
    // Broiler-Falsified-If: an absolute position outside the viewport is emitted without being clamped to it
    // Broiler-Human:        PENDING
    public UiInputEvent? SetAbsolutePointer(double x, double y, InputDeviceId device, long sequence)
    {
        lock (_gate)
        {
            double nextX = Clamp(x, _viewport.Width);
            double nextY = Clamp(y, _viewport.Height);

            // A sub-pixel move is not worth an event; at 60 Hz the queue would
            // fill with motion nobody can see.
            bool moved = !_pointerPlaced ||
                Math.Abs(nextX - _pointerX) >= 0.5 ||
                Math.Abs(nextY - _pointerY) >= 0.5;

            _pointerX = nextX;
            _pointerY = nextY;
            _pointerPlaced = true;

            return moved
                ? UiInputEvent.FromMouseMove(new MouseMoveEvent(
                    new InputEventHeader(device, StopwatchInputClock.Shared.GetTimestamp(), sequence),
                    InputPoint.ClientDeviceIndependentPixels(_pointerX, _pointerY),
                    _buttons,
                    InputEventSource.Semantic))
                : null;
        }
    }

    /// <summary>Relative device motion, accumulated into the tracked position.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BD2522
    // Broiler-Falsified-If: relative motion past the viewport edge yields a position below zero or above the extent minus one
    // Broiler-Human:        PENDING
    public UiInputEvent FromMouseMove(MouseMoveEvent motion)
    {
        lock (_gate)
        {
            Place();
            _pointerX = Clamp(_pointerX + motion.Position.X, _viewport.Width);
            _pointerY = Clamp(_pointerY + motion.Position.Y, _viewport.Height);
            return UiInputEvent.FromMouseMove(new MouseMoveEvent(
                motion.Header,
                InputPoint.ClientDeviceIndependentPixels(_pointerX, _pointerY),
                motion.Buttons,
                InputEventSource.Semantic));
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=11A5E4
    // Broiler-Falsified-If: a button event that arrives before any motion is emitted at a position other than the viewport centre
    // Broiler-Human:        PENDING
    public UiInputEvent FromMouseButton(MouseButtonEvent button)
    {
        lock (_gate)
        {
            Place();
            _buttons = button.Buttons;
            return UiInputEvent.FromMouseButton(new MouseButtonEvent(
                button.Header,
                InputPoint.ClientDeviceIndependentPixels(_pointerX, _pointerY),
                button.Buttons,
                button.Button,
                button.Transition,
                InputEventSource.Semantic));
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=43AFDC
    // Broiler-Falsified-If: a wheel event carrying Shift in its Modifiers is emitted with no modifiers
    // Broiler-Human:        PENDING
    public UiInputEvent FromWheel(MouseWheelEvent wheel)
    {
        lock (_gate)
        {
            Place();
            return UiInputEvent.FromMouseWheel(new MouseWheelEvent(
                wheel.Header,
                InputPoint.ClientDeviceIndependentPixels(_pointerX, _pointerY),
                wheel.Buttons,
                wheel.Axis,
                wheel.DeltaNotches,
                InputEventSource.Semantic));
        }
    }

    /// <summary>
    /// A key event, and the character it produces if it produces one. Both are
    /// returned together because the controls need both: the key drives
    /// shortcuts and caret movement, the text drives insertion, and a head that
    /// sent only one of them would have either no typing or no shortcuts.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=26C213
    // Broiler-Falsified-If: a KeyA press with Control held returns a text event alongside the key event
    // Broiler-Human:        PENDING
    public (UiInputEvent Key, UiInputEvent? Text) FromKey(KeyboardKeyEvent key)
    {
        KeyboardKeyEvent normalized = key with
        {
            Key = KeyboardKey.FromName(NormalizeKeyName(key.Key.Name)),
            Source = InputEventSource.Semantic,
        };

        UiInputEvent keyEvent = UiInputEvent.FromKeyboardKey(normalized);
        return TryComposeText(normalized, out string text)
            ? (keyEvent, UiInputEvent.FromTextInput(
                new TextInputEvent(normalized.Header, text, InputEventSource.Semantic)))
            : (keyEvent, null);
    }

    /// <summary>
    /// The names the Linux providers use, mapped to the ones the controls switch
    /// on. <c>DesktopInputRouter.MapVirtualKey</c> does the same job for Win32
    /// codes; a key arriving under an unexpected name is a shortcut that does
    /// nothing with no way to see why.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3C89C9
    // Broiler-Falsified-If: a name longer than four characters that starts with Key, such as Keypad, is shortened to a single character
    // Broiler-Human:        PENDING
    public static string NormalizeKeyName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (name.Length == 4 && name.StartsWith("Key", StringComparison.Ordinal))
            return char.ToUpperInvariant(name[3]).ToString();
        if (name.Length == 6 && name.StartsWith("Digit", StringComparison.Ordinal))
            return name[5].ToString();

        return name;
    }

    /// <summary>
    /// The character a key produces on a US layout, or none.
    ///
    /// Deliberately narrow. It covers unmodified and shifted Latin typing so the
    /// editor is usable, and refuses anything with Control, Alt, or a Windows
    /// key held, which are shortcuts rather than text. Everything beyond that —
    /// other layouts, dead keys, composition — needs a real input method, which
    /// is the gap this head reports rather than papers over.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=244F02
    // Broiler-Falsified-If: a key pressed with Control, Alt or a Windows key held produces text
    // Broiler-Human:        PENDING
    public static bool TryComposeText(KeyboardKeyEvent key, out string text)
    {
        text = string.Empty;

        if (key.Transition != KeyboardKeyTransition.Down)
            return false;

        const KeyboardModifierState Blocked =
            KeyboardModifierState.Control |
            KeyboardModifierState.Alt |
            KeyboardModifierState.LeftWindows |
            KeyboardModifierState.RightWindows;
        if ((key.Modifiers & Blocked) != 0)
            return false;

        bool shift = key.Modifiers.HasFlag(KeyboardModifierState.Shift);
        string name = NormalizeKeyName(key.Key.Name);

        if (name.Length == 1)
        {
            char character = name[0];
            if (character is >= 'A' and <= 'Z')
            {
                text = shift ? character.ToString() : char.ToLowerInvariant(character).ToString();
                return true;
            }

            if (character is >= '0' and <= '9')
            {
                text = (shift ? ShiftedDigit(character) : character).ToString();
                return true;
            }
        }

        text = name switch
        {
            "Space" => " ",
            "Tab" => "\t",
            "Minus" => shift ? "_" : "-",
            "Equal" => shift ? "+" : "=",
            "BracketLeft" => shift ? "{" : "[",
            "BracketRight" => shift ? "}" : "]",
            "Semicolon" => shift ? ":" : ";",
            "Quote" => shift ? "\"" : "'",
            "Backquote" => shift ? "~" : "`",
            "Backslash" => shift ? "|" : "\\",
            "Comma" => shift ? "<" : ",",
            "Period" => shift ? ">" : ".",
            "Slash" => shift ? "?" : "/",
            _ => string.Empty,
        };

        return text.Length > 0;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3E4419
    // Broiler-Falsified-If: Place moves a pointer that has already been placed
    // Broiler-Human:        PENDING
    private void Place()
    {
        if (_pointerPlaced)
            return;

        _pointerX = _viewport.Width / 2;
        _pointerY = _viewport.Height / 2;
        _pointerPlaced = true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6D2F1D
    // Broiler-Falsified-If: a value greater than extent minus one is returned above that bound
    // Broiler-Human:        PENDING
    private static double Clamp(double value, double extent)
    {
        double max = Math.Max(0, extent - 1);
        return value < 0 ? 0 : value > max ? max : value;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1AC1FD
    // Broiler-Falsified-If: a shifted digit maps to a character other than the US-layout symbol on that key
    // Broiler-Human:        PENDING
    private static char ShiftedDigit(char digit) => digit switch
    {
        '1' => '!',
        '2' => '@',
        '3' => '#',
        '4' => '$',
        '5' => '%',
        '6' => '^',
        '7' => '&',
        '8' => '*',
        '9' => '(',
        '0' => ')',
        _ => digit,
    };
}
