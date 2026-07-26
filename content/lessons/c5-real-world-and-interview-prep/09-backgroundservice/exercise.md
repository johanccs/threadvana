Write `Solution.RunUntilCancelledAsync(CancellationToken token)`:

1. Loop while `!token.IsCancellationRequested`.
2. Inside the loop: `Interlocked.Increment(ref Solution.Counter)`, then `await Task.Delay(50, token)`.
3. When the token fires, exit the loop and return `"stopped"`.

## Hints
1. `while (!token.IsCancellationRequested) { Counter++; await Task.Delay(50, token); }`
2. The `Task.Delay` will throw when cancelled â   catch or check before the increment.
