`Solution.GetGreetingAsync(bool cached)` simulates a high-frequency greeting service.

If `cached` is `true`: return a pre-computed greeting immediately without any allocation
— use `new ValueTask<string>(greeting)`.
If `cached` is `false`: fetch from a slow source via `Solution.FetchGreetingAsync()`
and return that (wrapped in a `ValueTask<string>`).

You can await a `Task<string>` and pass it directly to `new ValueTask<string>(task)`.

## Hints
1. `new ValueTask<string>("Hello, ThreadCraft!")` is the synchronous path.
2. `new ValueTask<string>(Solution.FetchGreetingAsync())` wraps the async Task.
3. The consumer (harness) awaits exactly once — no need for `.Preserve()`.
