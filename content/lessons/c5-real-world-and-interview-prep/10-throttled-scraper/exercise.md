Write `Solution.ScrapeUrlsAsync(string[] urls)`:

1. Use a `SemaphoreSlim(2)` to cap concurrency.
2. Start one `Task.Run` per URL that awaits the semaphore, then calls `FetchAsync(url)`, then releases.
3. `FetchAsync` simulates work with a 100ms delay — use the provided method.
4. Increment `Solution.Completed` via `Interlocked.Increment` for each finished fetch.
5. Return `"done"`.

## Hints
1. `var throttle = new SemaphoreSlim(2);`
2. `await throttle.WaitAsync(); try { await FetchAsync(url); Interlocked.Increment(ref Completed); } finally { throttle.Release(); }`
