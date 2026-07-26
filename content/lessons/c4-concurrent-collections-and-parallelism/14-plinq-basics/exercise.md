Write `Solution.ComputeParallelSumAsync()`:

1. Use PLINQ on `Solution.Data` (an `int[]` of 1..10) to compute the sum of squares.
2. Chain `.AsParallel().Select(n => n * n).Sum()`.
3. Return the sum.

## Hints
1. `Data.AsParallel().Select(n => n * n).Sum()` does it all.
2. No `Interlocked` needed â   `.Sum()` is thread-safe in PLINQ.
