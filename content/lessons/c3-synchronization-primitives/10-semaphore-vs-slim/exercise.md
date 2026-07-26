`Solution.NumWorkers` is set to 3. You have a `SemaphoreSlim` field that limits concurrency.

Write `Solution.ProcessAsync(int id)` that:
1. Awaits the semaphore to enter.
2. Sleeps 200ms (simulating work).
3. Releases the semaphore.

The harness calls ProcessAsync from 6 tasks and checks that at most `NumWorkers` (3)
were ever inside the critical section at once.

## Hints
1. `await _semaphore.WaitAsync()` to enter.
2. `await Task.Delay(200)` is the work.
3. `_semaphore.Release()` in a try/finally â   or use a helper pattern.
