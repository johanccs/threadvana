Write `Solution.SetAndReadContextAsync()`:

1. Set `Solution.Context.Value` to `"hello"`.
2. Call `await Task.Yield()` (forces a thread switch).
3. Read `Solution.Context.Value` â   it should still be `"hello"`.
4. Return the value.

`Solution.Context` is already defined as `AsyncLocal<string>`.

## Hints
1. `Context.Value = "hello";`
2. `await Task.Yield();` â   the current thread yields.
3. Return `Context.Value` â   AsyncLocal should carry it across the yield.
