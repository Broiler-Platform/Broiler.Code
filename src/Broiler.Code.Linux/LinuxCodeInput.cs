// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           15
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    High
// Criteria:         13/7
// Resource impact:  4/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broiler.Code.Core.Hosting;
using Broiler.Input;
using Broiler.Input.Keyboard;
using Broiler.Input.Keyboard.Linux;
using Broiler.Input.Linux;
using Broiler.Input.Mouse;
using Broiler.Input.Mouse.Linux;
using Broiler.UI;

namespace Broiler.Code.Linux;

/// <summary>
/// Opens the evdev keyboard and mouse and queues what they report.
///
/// Only the device half lives here. The translation into UI input — pointer
/// tracking, key names, characters — is <see cref="EvdevInputRouter"/>'s, in
/// Core, so it can be tested without a device and so the head stays thin.
///
/// Events arrive on the providers' own threads and are queued rather than
/// dispatched, because a control touched from a device thread is a race. The
/// window's loop drains the queue on the UI thread.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=52B01F
// Broiler-Falsified-If: a key typed while another application has focus is dispatched to the editor although the window never reported focus to SetActiveAsync
// Broiler-Human:        PENDING
internal sealed class LinuxCodeInput(EvdevInputRouter router, Action<string> log) : IAsyncDisposable
{
    private readonly EvdevInputRouter _router = router ?? throw new ArgumentNullException(nameof(router));
    private readonly Action<string> _log = log ?? throw new ArgumentNullException(nameof(log));
    private readonly ConcurrentQueue<UiInputEvent> _pending = new();
    private readonly InputDeviceId _pointerId = InputDeviceId.FromOpaqueValue("broiler-code-linux:x11-pointer");

    private LinuxKeyboardProvider? _keyboardProvider;
    private LinuxMouseProvider? _mouseProvider;
    private KeyboardInputDevice? _keyboard;
    private MouseInputDevice? _mouse;
    private long _pointerSequence;
    private bool _started;
    private bool _active;
    private bool _disposed;

    public bool QuitRequested { get; private set; }

    public string KeyboardDevice { get; private set; } = "none";

    public string MouseDevice { get; private set; } = "none";

    /// <summary>True once at least one device is open and readable.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=380C67
    // Broiler-Falsified-If: IsAvailable reads false while a keyboard or mouse device is open, so SetActiveAsync(false) returns without stopping it
    // Broiler-Human:        PENDING
    public bool IsAvailable => _keyboard is not null || _mouse is not null;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=5A21D5
    // Broiler-Falsified-If: an opened keyboard delivers key events to the pending queue before SetActiveAsync(true) has started it
    // Broiler-Human:        PENDING
    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (_started)
            return;

        _started = true;

        if (!OperatingSystem.IsLinux())
        {
            _log("evdev devices are only opened on Linux; input is disabled.");
            return;
        }

        LinuxEventDeviceAccessStatus access = LinuxEventDeviceAccessProbe.CheckEventDeviceAccess();
        _log("evdev device access: " + access.Diagnostic);

        var options = new LinuxEvdevProviderOptions(AcknowledgeRawBackgroundInput: true);
        _keyboardProvider = new LinuxKeyboardProvider(options);
        _mouseProvider = new LinuxMouseProvider(options);

        _keyboard = await OpenKeyboardAsync(cancellationToken).ConfigureAwait(false);
        _mouse = await OpenMouseAsync(cancellationToken).ConfigureAwait(false);

        if (!IsAvailable)
        {
            _log("no readable keyboard or mouse event device was opened; the window will not accept input.");
            Explain(access);
            return;
        }

        if (_keyboard is not null)
            _keyboard.KeyChanged += OnKeyChanged;
        if (_mouse is not null)
        {
            _mouse.Moved += OnMouseMoved;
            _mouse.ButtonChanged += OnMouseButtonChanged;
            _mouse.WheelChanged += OnMouseWheelChanged;
        }
    }

    /// <summary>
    /// Starts or stops reading devices. evdev is a global stream with no notion
    /// of a window, so this is the only thing stopping keystrokes meant for
    /// another application from being typed into the editor.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=14B20F
    // Broiler-Falsified-If: SetActiveAsync(false) returns normally while the keyboard device is still started, so keys typed into another application are queued for the editor
    // Broiler-Human:        PENDING
    public async ValueTask SetActiveAsync(bool active, CancellationToken cancellationToken = default)
    {
        if (!IsAvailable || _active == active)
            return;

        if (active)
        {
            if (_keyboard is not null)
                await _keyboard.StartAsync(cancellationToken).ConfigureAwait(false);
            if (_mouse is not null)
                await _mouse.StartAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            if (_keyboard is not null)
                await _keyboard.StopAsync(cancellationToken).ConfigureAwait(false);
            if (_mouse is not null)
                await _mouse.StopAsync(cancellationToken).ConfigureAwait(false);
        }

        _active = active;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6EBB00
    // Broiler-Falsified-If: two pointer updates are given the same pointer sequence number
    // Broiler-Human:        PENDING
    public void SetAbsolutePointer(double x, double y)
    {
        if (_router.SetAbsolutePointer(x, y, _pointerId, Interlocked.Increment(ref _pointerSequence))
            is { } moved)
        {
            _pending.Enqueue(moved);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0B12C6
    // Broiler-Falsified-If: an event enqueued by a device thread while a drain runs is lost rather than dispatched in this drain or the next
    // Broiler-Human:        PENDING
    public int Drain(Action<UiInputEvent> dispatch)
    {
        ArgumentNullException.ThrowIfNull(dispatch);

        int count = 0;
        while (_pending.TryDequeue(out UiInputEvent? input))
        {
            dispatch(input);
            count++;
        }

        return count;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=724DAA
    // Broiler-Falsified-If: after DisposeAsync completes, the keyboard or mouse device is still open or its events still reach the pending queue
    // Broiler-Human:        PENDING
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        if (_active)
            await SetActiveAsync(false).ConfigureAwait(false);

        if (_keyboard is not null)
        {
            _keyboard.KeyChanged -= OnKeyChanged;
            await _keyboard.DisposeAsync().ConfigureAwait(false);
        }

        if (_mouse is not null)
        {
            _mouse.Moved -= OnMouseMoved;
            _mouse.ButtonChanged -= OnMouseButtonChanged;
            _mouse.WheelChanged -= OnMouseWheelChanged;
            await _mouse.DisposeAsync().ConfigureAwait(false);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=602E8F
    // Broiler-Falsified-If: a key event whose router result carries text queues the text before the key, so the editor sees the character before its key-down
    // Broiler-Human:        PENDING
    private void OnKeyChanged(KeyboardKeyEvent inputEvent)
    {
        (UiInputEvent key, UiInputEvent? text) = _router.FromKey(inputEvent);
        _pending.Enqueue(key);
        if (text is not null)
            _pending.Enqueue(text);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2A1D43
    // Broiler-Falsified-If: a mouse move is dispatched on the device thread instead of being queued for the UI thread
    // Broiler-Human:        PENDING
    private void OnMouseMoved(MouseMoveEvent inputEvent) =>
        _pending.Enqueue(_router.FromMouseMove(inputEvent));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=ED9D85
    // Broiler-Falsified-If: a mouse button change is dispatched on the device thread instead of being queued for the UI thread
    // Broiler-Human:        PENDING
    private void OnMouseButtonChanged(MouseButtonEvent inputEvent) =>
        _pending.Enqueue(_router.FromMouseButton(inputEvent));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=1E543E
    // Broiler-Falsified-If: a wheel change is dispatched on the device thread instead of being queued for the UI thread
    // Broiler-Human:        PENDING
    private void OnMouseWheelChanged(MouseWheelEvent inputEvent) =>
        _pending.Enqueue(_router.FromWheel(inputEvent));

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=5E3814
    // Broiler-Falsified-If: when opening the first available keyboard throws, the next available keyboard is not tried and keyboard input stays disabled
    // Broiler-Human:        PENDING
    private async ValueTask<KeyboardInputDevice?> OpenKeyboardAsync(CancellationToken cancellationToken)
    {
        if (_keyboardProvider is null)
            return null;

        IReadOnlyList<InputDeviceDescriptor> devices =
            await _keyboardProvider.GetDevicesAsync(cancellationToken).ConfigureAwait(false);

        foreach (InputDeviceDescriptor descriptor in Available(devices))
        {
            try
            {
                // ReceiveText: false — evdev carries no text, and asking for it
                // would let a provider synthesise characters this head has
                // already decided to derive itself, in one place.
                KeyboardInputDevice device = await _keyboardProvider
                    .OpenAsync(descriptor, new KeyboardOpenOptions(ReceiveText: false), cancellationToken)
                    .ConfigureAwait(false);
                KeyboardDevice = Describe(descriptor);
                _log("keyboard: " + KeyboardDevice);
                return device;
            }
            catch (Exception exception)
            {
                _log("keyboard open failed for " + descriptor.DisplayName + ": " + exception.Message);
            }
        }

        if (devices.Any(static device => device.Availability == InputDeviceAvailability.PermissionDenied))
            _log("keyboard: event devices exist but permission was denied.");

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D690DE
    // Broiler-Falsified-If: when opening the first available mouse throws, the next available mouse is not tried and mouse input stays disabled
    // Broiler-Human:        PENDING
    private async ValueTask<MouseInputDevice?> OpenMouseAsync(CancellationToken cancellationToken)
    {
        if (_mouseProvider is null)
            return null;

        IReadOnlyList<InputDeviceDescriptor> devices =
            await _mouseProvider.GetDevicesAsync(cancellationToken).ConfigureAwait(false);

        foreach (InputDeviceDescriptor descriptor in Available(devices))
        {
            try
            {
                MouseInputDevice device = await _mouseProvider
                    .OpenAsync(descriptor, new MouseOpenOptions(), cancellationToken)
                    .ConfigureAwait(false);
                MouseDevice = Describe(descriptor);
                _log("mouse: " + MouseDevice);
                return device;
            }
            catch (Exception exception)
            {
                _log("mouse open failed for " + descriptor.DisplayName + ": " + exception.Message);
            }
        }

        if (devices.Any(static device => device.Availability == InputDeviceAvailability.PermissionDenied))
            _log("mouse: event devices exist but permission was denied.");

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D5E0BA
    // Broiler-Human:        PENDING
    private void Explain(LinuxEventDeviceAccessStatus access)
    {
        if (!access.DirectoryExists || access.EventDeviceCount == 0)
        {
            _log("  reason: no /dev/input/event* devices are visible (headless session, or " +
                "/dev/input was not passed into the container).");
            return;
        }

        if (access.ReadableEventDeviceCount == 0)
        {
            _log("  reason: " + access.EventDeviceCount.ToString(CultureInfo.InvariantCulture) +
                " /dev/input/event* device(s) exist but are not readable by this user.");
            _log("  fix: sudo usermod -aG input \"$USER\", then log out and back in.");
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=2; Fingerprint=1589B8
    // Broiler-Human:        PENDING
    private static IEnumerable<InputDeviceDescriptor> Available(
        IReadOnlyList<InputDeviceDescriptor> devices) =>
        devices.Where(static device => device.Availability == InputDeviceAvailability.Available);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=DEA113
    // Broiler-Human:        PENDING
    private static string Describe(InputDeviceDescriptor descriptor)
    {
        string? node = descriptor.Capabilities
            .Where(static capability => capability.Name == "event-device")
            .Select(static capability => capability.Value)
            .FirstOrDefault();

        return string.IsNullOrWhiteSpace(node)
            ? descriptor.DisplayName
            : descriptor.DisplayName + " (" + node + ")";
    }
}
