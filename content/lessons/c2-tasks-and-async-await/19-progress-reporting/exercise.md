Write `Solution.RunWithProgressAsync(IProgress<int> progress)`:

1. Loop from 0 to 5, pausing 100ms each iteration.
2. Call `progress.Report(i * 20)` at each step (so it reports 0, 20, 40, 60, 80, 100).
3. Return `"done"` when finished.

The harness wraps a counter, then calls your method and checks that all six progress
values were reported.

## Hints
1. `IProgress<int>.Report(value)` is a void method — call it, don't await it.
2. Loop 6 times (0..5), report `i * 20`, await 100ms delay.
3. Return `"done"` after the loop — the harness awaits your Task<string>.
