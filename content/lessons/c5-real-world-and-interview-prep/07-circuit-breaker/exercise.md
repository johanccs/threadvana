Write `Solution.CallWithCircuitBreakerAsync(Func<Task<string>> operation, int threshold)`:

1. Track consecutive failures in `_failures`.
2. If failures >= threshold, return `"open"` immediately (fast fail).
3. Otherwise, try the operation. On success: reset failures, return result.
   On failure: increment failures, return `"failed"`.

## Hints
1. `if (_failures >= threshold) return Task.FromResult("open");`
2. Use `Interlocked.Increment`/`Exchange` to update `_failures` safely.
