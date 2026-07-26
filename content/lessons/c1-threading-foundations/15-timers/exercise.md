Make your first timer.

Inside `Solution.Run()`:
1. Create a `new Timer(...)` that sets `Solution.Done = true` after 300 milliseconds.
2. Wait until `Done` is true before returning from `Run()` (a simple
   `while (!Done) Thread.Sleep(10)` loop works for this).

Use a one-shot timer: `dueTime: 300, period: Timeout.Infinite`. The callback is a
lambda: `_ => { Solution.Done = true; }`.

## Hints
1. The timer constructor: `new Timer(callback, state, dueTime, period)`. For `state`,
   use `null` since you don't need to pass extra data.
2. `TimeSpan.Infinite`? No — use `Timeout.Infinite` (from `System.Threading`).
3. After starting the timer, wait in a loop until `Done` is true, then `Dispose` the
   timer (or use `using var`).
