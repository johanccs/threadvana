Write `Solution.RetryWithBackoffAsync(Func<Task<string>> operation, int maxRetries)`:

1. Loop up to `maxRetries + 1` times.
2. Try the operation; on success return the result.
3. On exception, if last attempt, return `"failed"`.
4. Otherwise, delay with backoff: `100ms Ã  2^attempt` and retry.

## Hints
1. `for (var i = 0; i <= maxRetries; i++)`
2. `catch { if (i == maxRetries) return "failed"; await Task.Delay(100 * (int)Math.Pow(2, i)); }`
