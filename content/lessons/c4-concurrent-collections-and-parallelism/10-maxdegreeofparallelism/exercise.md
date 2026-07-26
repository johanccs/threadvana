Write `Solution.RunWithMaxParallelismAsync()`:

1. Use `Parallel.For(0, 10, new ParallelOptions { MaxDegreeOfParallelism = 2 }, i => ...)`.
2. Inside the body, call `Interlocked.Increment(ref Solution.Counter)`.
3. Return `"done"`.

The point is to use `Parallel.For` WITH `MaxDegreeOfParallelism` set to 2.

## Hints
1. `var opts = new ParallelOptions { MaxDegreeOfParallelism = 2 };`
2. `Parallel.For(0, 10, opts, i => Interlocked.Increment(ref Counter));`
3. Return `"done"`.
