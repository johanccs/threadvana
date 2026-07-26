`Solution.ProcessWithTimeoutAsync(int timeoutMs)` should:

1. Start a slow, cancellable task by calling `Solution.SlowWorkAsync(CancellationToken token)` â  
   a helper that loops 20 times with a 100ms delay, honouring the token each iteration.
2. Create a `CancellationTokenSource` set to cancel after `timeoutMs` milliseconds.
3. Pass the token to `SlowWorkAsync` inside a `try/catch`:
   - If `OperationCanceledException` is thrown, return `"cancelled"`.
   - If no exception, return `"finished"`.
4. Always `Dispose()` the `CancellationTokenSource` â   the best way is a `using` block.

## Hints
1. `new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutMs))` or `new CancellationTokenSource(timeoutMs)`.
2. `using var cts = ...;` Dispose is automatic â   otherwise `try/finally { cts.Dispose(); }`.
3. `OperationCanceledException` â   a using for `System` adds it; catch that specific exception.
