Write `Solution.SumSquaresAsync()`:

1. Given an `int[]` â   call `Solution.Data` (already set to `[1,2,3,4,5]`).
2. Use `Parallel.ForEach` to compute the sum of squares of each item.
3. Return the sum as an `int`.

## Hints
1. `Parallel.ForEach(Data, item => { /* ... */ });`
2. Use `Interlocked.Add(ref sum, item * item)` inside the loop body.
3. Return the sum after the parallel loop completes.
