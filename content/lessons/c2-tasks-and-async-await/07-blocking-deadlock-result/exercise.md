The starter code for `Solution.FetchSumAsync()` uses `.Result` to wait for
two number-fetching tasks — this blocks the thread and wastes pool capacity.

Your job:

1. Rewrite `FetchSumAsync` to use `await` instead of `.Result` — the method
   should be `async` and return `Task<int>`.
2. The two fetchers are `Solution.FetchAAsync()` and `Solution.FetchBAsync()`
   (they are already written for you). Start both with `Task.Run`, then
   await both results, then return their sum.

## Hints

1. `var a = await taskA` replaces `var a = taskA.Result`.
2. If you add `async` to `FetchSumAsync`, change the return type to `Task<int>`.
3. Nothing else changes — the same two fetchers, the same sum. Just cooperative waiting.
