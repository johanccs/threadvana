Write `CallAsyncFromSync()` that calls `FetchAsync()` (async) from a synchronous method. Use `Task.Run(() => FetchAsync()).GetAwaiter().GetResult()` to avoid deadlocks. Return the fetched string.

## Hints
1. `Task.Run(() => FetchAsync()).GetAwaiter().GetResult()` runs the async method on a pool thread and blocks synchronously for the result.
