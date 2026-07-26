Write `Solution.RunWithTimeoutAsync(int timeoutMs)`:

1. Start `Solution.BackgroundJobAsync(token)` — a slow job that honours cancellation.
2. Race it against `Task.Delay(timeoutMs)` using `Task.WhenAny`.
3. If the job finishes first: return `"completed"`.
4. If the timeout expires first: cancel the source, then return `"timeout"`.

`BackgroundJobAsync` is provided — it loops 10 times with a 200ms delay between steps,
honouring the cancellation token each iteration.

## Hints
1. `using var cts = new CancellationTokenSource(timeoutMs);`
2. `var winner = await Task.WhenAny(job, Task.Delay(timeoutMs));`
3. If timeout won, call `cts.Cancel()` before returning `"timeout"`.
