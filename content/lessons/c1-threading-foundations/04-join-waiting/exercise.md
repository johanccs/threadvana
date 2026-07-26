Make "done" mean done.

`Solution.Work()` is provided: it pretends to work for 300ms and then sets
`Solution.Flag = true`.

Inside `Solution.Run()`:

1. Create a `new Thread(...)` that runs `Work`.
2. Store it in `Solution.Worker` so the checker can inspect your thread.
3. `Start()` it.
4. `Join()` it - so `Flag` is GUARANTEED to be set before `Run()` returns.

The checker reads `Flag` at the exact moment `Run()` returns. No grace period.
The checker is strict on purpose - that strictness IS the lesson: without a
`Join`, `Run()` wins the race every time (returning takes ~1ms, the work
needs 300ms).

## Hints
1. `Worker = new Thread(Work);` then `Worker.Start();` - the Join goes last.
2. Join means "pause MY thread until THAT thread is done" - one line: `Worker.Join();`
3. If the flag is only set "later" or "sometimes", you started the work but never waited for it.
