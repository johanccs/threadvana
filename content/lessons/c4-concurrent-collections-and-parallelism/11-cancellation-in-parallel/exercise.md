Write `Solution.RunCancellableLoopAsync(CancellationToken token)`:

1. Use `Parallel.For(0, 100, new ParallelOptions { CancellationToken = token }, i => { ... })`.
2. Inside the body, call `token.ThrowIfCancellationRequested()` first.
3. Simulate work with `Thread.SpinWait(1000)`.
4. Catch `OperationCanceledException` and return `"cancelled"`. Otherwise return `"done"`.

## Hints
1. `var opts = new ParallelOptions { CancellationToken = token };`
2. Wrap the `Parallel.For` in try/catch for OCE.
3. Return the appropriate string.
