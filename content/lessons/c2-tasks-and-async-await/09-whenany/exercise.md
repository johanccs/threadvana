Write `Solution.RaceWithTimeoutAsync(int timeoutMs)` that:

1. Starts `Solution.RealWorkAsync()` (a slow method that takes 2 seconds).
2. Starts a timeout using `Task.Delay(timeoutMs)`.
3. Races both with `Task.WhenAny`.
4. If the real work wins: return its result (a string).
5. If the timeout wins: cancel the real work and return `"timeout"`.

`RealWorkAsync` is already written for you — it takes a `CancellationToken` and returns `"done"`.

## Hints
1. `Task.Delay(timeoutMs)` returns a `Task` — race it directly, no `Task.Run`.
2. `var winner = await Task.WhenAny(work, delay);` — then check which one it is.
3. If timeout won, throw or token-source-cancel the `RealWorkAsync` call so it stops cleanly.
