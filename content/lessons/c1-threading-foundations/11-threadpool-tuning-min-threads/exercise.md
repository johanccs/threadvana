Tell the pool to get ready for a burst.

Inside `Solution.Run()`:
1. Set `Workers = 8` (the number of workers you need ready).
2. Call `ThreadPool.SetMinThreads(Workers, Workers)`.
3. Set `UsedSetMinThreads = true` so the harness knows you did it.

The starter already has the reset line `ThreadPool.SetMinThreads(1, 1)` at the end
of `Run()` to be polite to other lessons. You don't need to change that.

## Hints
1. The exact call is `ThreadPool.SetMinThreads(Workers, Workers)` â   both arguments
   need the number of workers.
2. The reset line is already in the starter â   just leave it.
3. Set `Workers = 8` at the top of the class, or right before the `SetMinThreads` call.
