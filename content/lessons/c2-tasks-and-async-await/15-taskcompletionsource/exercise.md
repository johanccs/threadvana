Write `Solution.WaitForSignalAsync()` that returns a `Task` that completes only when you
call `Solution.Trigger()` from another thread.

- `WaitForSignalAsync` should create a `TaskCompletionSource`, store it in a static field,
  and return its `.Task`.
- `Trigger()` should call `TrySetResult()` on the stored TCS to complete it.

The harness calls `WaitForSignalAsync`, verifies it hasn't finished yet, then calls `Trigger`
and verifies the task completes.

## Hints
1. `new TaskCompletionSource().Task` gives you a plain `Task` (no result — use non-generic TCS).
2. `TrySetResult()` succeeds once — use `null` or a default value if needed (for non-generic).
3. Store the TCS in a `static TaskCompletionSource?` field so both methods can access it.
