Write `Solution.SignalAndWaitAsync()`:

1. Get the `AutoResetEvent` from `Solution.Evt`.
2. Call `Signal()` on the event (hint: `Set()`).
3. Wait 100ms.
4. Return `"signalled"`.

The harness has a waiter blocking on WaitOne — your Set() should release it.

## Hints
1. `Evt.Set()` signals exactly one waiting thread.
2. No Wait/Reset needed for this exercise — just Set() and return.
