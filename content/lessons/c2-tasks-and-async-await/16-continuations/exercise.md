`Solution.ChainWithContinueWithAsync()` does exactly the same work as `ChainWithAwaitAsync`,
but using `ContinueWith` instead of `await`:

1. Start `Task.Run(() => 10)`.
2. Use `.ContinueWith(t => t.Result * 2)` to chain a doubling operation.
3. **Return** the resulting chained `Task<int>` — your method does NOT need `async`.
4. The harness calls it and expects `20`.

`ChainWithAwaitAsync` is provided as a reference — your `ContinueWith` version
should produce the same result without using `await`.

## Hints
1. `Task.Run(() => 10).ContinueWith(t => t.Result * 2)` returns a `Task<int>` — return it directly.
2. No `async` keyword on the method — it returns a `Task<int>` without being async.
3. If you see `Unwrap` in the return type, you are chaining too deeply — `ContinueWith` on a `Task<int>` returns `Task<int>`, not `Task<Task<int>>`.
