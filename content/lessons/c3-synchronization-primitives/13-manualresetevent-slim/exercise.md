`Solution.Gate` is a `ManualResetEventSlim` — call `Set()` and `Reset()` on it.

Write `Solution.OpenAndCloseGateAsync()`:
1. Call `Set()` on the gate — waiters unblock.
2. Wait 100ms.
3. Call `Reset()` on the gate — new callers block again.
4. Return `"toggled"`.

The harness has two waiters already waiting before calling your method.

## Hints
1. `Gate.Set()` opens; `Gate.Reset()` closes.
2. No Wait/Release needed — just set and reset the gate.
