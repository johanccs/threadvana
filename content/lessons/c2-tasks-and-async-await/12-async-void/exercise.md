The starter gives you an `async void` method `Solution.LaunchWorkers()` that calls a helper
`Solution.WorkerAsync(int id)`. This is broken — the caller cannot track the work, and if
`WorkerAsync` somehow throws, it will crash the process.

Your job:

1. Change `LaunchWorkers` from `async void` to `async Task`.
2. Start two workers inside it (id 1 and id 2) and await both.
3. To prove the fix: change the return statement to `return Task.WhenAll(w1, w2)`.

The harness will `await Solution.LaunchWorkers()` and verify it completes — something
impossible with the original `async void` signature.

## Hints
1. `async void` → `async Task` is a one-word change; change the return type and the harness can await it.
2. `Task.WhenAll` returns a Task — you can await it or return it directly.
3. If you changed the return type, also change any `return 0;` statement.
