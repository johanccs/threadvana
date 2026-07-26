Replace the sequential loop with Parallel.For.

`SlowSquare(int n)` does 2ms of work simulating a CPU-heavy operation. The starter runs
it sequentially for 0..99 (200+ ms). Convert the `for` loop to `Parallel.For` so it
finishes faster.

After the loop, set `Solution.ElapsedMs` to the elapsed milliseconds (use a Stopwatch
around the loop). The harness checks ElapsedMs < 150 — parallel should be faster.

## Hints
1. `Parallel.For(0, 100, i => { ... });` replaces `for (var i = 0; i < 100; i++) { ... }`.
2. `using System.Diagnostics;` for Stopwatch: `var sw = Stopwatch.StartNew(); ... ElapsedMs = sw.ElapsedMilliseconds;`
3. The body calls `SlowSquare(i)` but that doesn't compute a result — it just burns CPU. That's intentional.
