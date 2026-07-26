Teach the busy loop some manners.

`Solution.BusyWork()` is provided for reference: a tight loop, 2000 rounds, no
breaks - it hogs the CPU like someone who never lets another car merge.

Also provided: `Solution.Pause()`. It counts the pause (in `Solution.PauseCount`)
AND offers the CPU to other threads via `Thread.Yield()`.

Inside `Solution.PoliteWork()` (the same loop, already written for you):

1. Keep all 2000 rounds intact - `Solution.Count` must still reach 2000.
2. Every 100 iterations, call `Pause()` so other threads get a turn.
   About 20 pauses should happen in total; the checker wants at least 15.

## Hints
1. The modulo operator finds every 100th round: `if (i % 100 == 99) Pause();`
2. Do not touch the loop bounds - only ADD the pause inside the loop body.
3. Too few pauses means the condition fires too rarely - every 100 iterations, not every 500.
