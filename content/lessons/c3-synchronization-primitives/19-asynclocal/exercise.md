Write `Solution.SetAndReadContextAsync()`:

1. Set `Solution.Context.Value` to `"hello"`.
2. Call `await Task.Yield()` (forces a thread switch).
3. Read `Solution.Context.Value` — it should still be `"hello"`.
4. Return the value.

`Solution.Context` is already defined as `AsyncLocal<string>`.

## Hints
1. `Context.Value = "hello";`
2. `await Task.Yield();` — the current thread yields.
3. Return `Context.Value` — AsyncLocal should carry it across the yield.
