// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           4
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  4/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Threading;
using Broiler.UI;

namespace Broiler.Code.Core.Hosting;

/// <summary>
/// A real UI-thread dispatcher: work posted from any thread is queued and run
/// on the thread that created it.
///
/// This exists because the Standard <c>ImmediateUiDispatcher</c> runs its
/// callback inline on the calling thread and answers true to every
/// <see cref="CheckAccess"/>. That is harmless for Writer, whose work already
/// originates on the UI thread, and wrong for Code: classification and build
/// results complete on worker threads, so an immediate dispatcher would mutate
/// control state from whichever thread happened to finish first. The roadmap is
/// explicit that Writer's dispatcher must not be carried into the Code support
/// claim, and this is why.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=08954B
// Broiler-Falsified-If: a callback passed to Post from a worker thread runs on that worker thread rather than on the thread that constructed the dispatcher
// Broiler-Human:        PENDING
public sealed class UiThreadDispatcher : IUiDispatcher
{
    private readonly Queue<Action> _pending = new();
    private readonly int _uiThreadId;
    private readonly Action? _wake;

    /// <param name="wake">
    /// Asks the host to drain soon. A windowed host posts a message; a polling
    /// host can leave it null and drain on its next tick.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=089C6F
    // Broiler-Falsified-If: UiThreadId differs from the managed thread id of the thread that ran the constructor
    // Broiler-Human:        PENDING
    public UiThreadDispatcher(Action? wake = null)
    {
        _uiThreadId = Environment.CurrentManagedThreadId;
        _wake = wake;
    }

    /// <summary>The thread this dispatcher considers the UI thread.</summary>
    public int UiThreadId => _uiThreadId;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=040E0B
    // Broiler-Falsified-If: a read concurrent with Post throws or returns a count the queue never held
    // Broiler-Human:        PENDING
    public int PendingCount
    {
        get
        {
            lock (_pending)
                return _pending.Count;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=BE395E
    // Broiler-Falsified-If: CheckAccess returns true on a thread other than the one that constructed the dispatcher
    // Broiler-Human:        PENDING
    public bool CheckAccess() => Environment.CurrentManagedThreadId == _uiThreadId;

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=EE0A23
    // Broiler-Falsified-If: a callback posted concurrently with another Post or with a Drain is lost and never runs
    // Broiler-Human:        PENDING
    public void Post(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        lock (_pending)
            _pending.Enqueue(callback);
        _wake?.Invoke();
    }

    /// <summary>
    /// Runs the work queued so far and returns how much ran. Called from the
    /// host's loop; throws if called from anywhere else, because running UI
    /// work off the UI thread is the failure this type exists to prevent.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=7BE455
    // Broiler-Falsified-If: a callback posted while Drain runs is executed within that same Drain call
    // Broiler-Human:        PENDING
    public int Drain()
    {
        if (!CheckAccess())
        {
            throw new InvalidOperationException(
                "The dispatcher may only be drained on the UI thread.");
        }

        // The batch is bounded to what was queued on entry. Draining until the
        // queue is empty lets a callback that posts more work starve the frame
        // it is running in.
        int budget;
        lock (_pending)
            budget = _pending.Count;

        int ran = 0;
        while (ran < budget)
        {
            Action next;
            lock (_pending)
            {
                if (_pending.Count == 0)
                    break;
                next = _pending.Dequeue();
            }

            next();
            ran++;
        }

        return ran;
    }
}
