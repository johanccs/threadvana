`Solution.IsRunning` is a volatile `bool` flag. The `Solution.WaitUntilStarted()` method
should spin-read it in a loop until it sees `true`, then return `"started"`.

`Solution.SignalStart()` sets `IsRunning` to `true`.

The harness calls `WaitUntilStarted()` from one thread and `SignalStart()` from another
100ms later. Fix the loop so it actually reads the updated flag.

## Hints
1. `while (!IsRunning) { }` â   but this won't work without volatile or explicit Volatile.Read.
2. Since `IsRunning` is already `volatile`, the simple `while` loop works.
3. The challenge is understanding WHY volatile is needed here â   the starter has it, your code just needs the loop.
