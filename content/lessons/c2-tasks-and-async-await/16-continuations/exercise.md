`Solution.ChainWithContinueWithAsync()` does exactly the same work as `ChainWithAwaitAsync`,
but using `ContinueWith` instead of `await`:

1. Start `Task.Run(() => 10)`.
2. Use `.ContinueWith(t => t.Result * 2)` to chain a doubling operation.
3. **Return** the resulting chained `Task<int>` â   your method does NOT need `async`.
4. The harness calls it and expects `20`.

`ChainWithAwaitAsync` is provided as a reference â   your `ContinueWith` version
should produce the same result without using `await`.

## Hints
1. `Task.Run(() => 10).ContinueWith(t => t.Result * 2)` returns a `Task<int>` â   return it directly.
2. No `async` keyword on the method â   it returns a `Task<int>` without being async.
3. If you see `Unwrap` in the return type, you are chaining too deeply â   `ContinueWith` on a `Task<int>` returns `Task<int>`, not `Task<Task<int>>`.
