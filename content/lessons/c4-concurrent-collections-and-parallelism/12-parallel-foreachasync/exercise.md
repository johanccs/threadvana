Write `Solution.ProcessItemsAsync()`:

1. Use `Parallel.ForEachAsync` with `Solution.Items` (set to `new[] { 1, 2, 3, 4 }`).
2. Set `MaxDegreeOfParallelism = 2`.
3. In the body, increment `Solution.Processed` via `Interlocked.Increment`.
4. Return `"done"`.

## Hints
1. `await Parallel.ForEachAsync(Items, new ParallelOptions { MaxDegreeOfParallelism = 2 }, async (item, ct) => { ... });`
2. `Interlocked.Increment(ref Processed)` inside the async lambda.
