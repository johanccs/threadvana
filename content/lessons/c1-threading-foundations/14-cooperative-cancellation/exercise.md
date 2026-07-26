Teach a worker to stop when asked.

The starter starts a loop that counts from 1 to 1,000,000,000 â   it runs forever. Your
job is to add a stop flag so it can be told to stop cleanly.

1. Add a `volatile bool` flag at the top of the `Solution` class (call it
   `StopRequested`).
2. Change the loop condition to check `!Solution.StopRequested`.
3. In the loop body, call `Solution.Increment()` for every iteration (it records
   the count in `Solution.Count`).
4. From the main thread (after `worker.Start()`): sleep 100 ms, set
   `StopRequested = true`, then `Join`.

The harness checks that the worker stopped EARLY (Count < 200,000,000 â   way before
a billion) AND that it stopped cleanly (the thread finished, the test does not
time out).

## Hints
1. The flag declaration: `public static volatile bool StopRequested;`
2. The loop: `for (var i = 0; i < 1_000_000_000 && !Solution.StopRequested; i++)`
3. After starting the worker, call `Thread.Sleep(100)` then set the flag, then `Join`.
