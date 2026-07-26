Start a named thread and log its work.

Inside `Solution.Run()`:
1. Create a new Thread that calls `DoWork()` (provided — it increments a counter).
2. Name it `"data-worker"`.
3. Start it and Join it.

We check that the thread's name was set and that it completed its work.

## Hints
1. Set `.Name` BEFORE calling `.Start()`. Setting it after Start throws.
2. `worker.Name = "data-worker";` — plain assignment, no special method.
3. The `DoWork()` method is already provided in the starter.
