Spot the starved thread.

Two threads share a lock. Thread A (`GreedyWorker`) grabs the lock and then does
5 chunks of slow work (50ms each) without ever letting go. Thread B
(`StarvingWorker`) attempts the lock ONCE but doesn't get it until A is
completely done.

After calling `Solution.Run()`:
- `GreedyWorker` sets `Solution.GreedyRuns` (counts how many times it got the lock).
- `StarvingWorker` sets `Solution.StarvingWaitedMs` (how long it waited from its start
  to getting the lock).

Your job: set `Solution.MinWaitMs` to the MINIMUM number of milliseconds the starving
worker MUST have waited (the greedy worker holds the lock for at least 5 × 50 = 250 ms).
Just write the number.

## Hints
1. The greedy worker holds the lock for 5 chunks × 50ms = 250ms, and the starving
   worker is already waiting before the first chunk begins.
2. The answer is 250 (minimum). Set `MinWaitMs = 250`.
3. The harness runs Run(), checks that `StarvingWaitedMs >= MinWaitMs`.
