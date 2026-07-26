Build a race detector.

Yes, really — this exercise wants the BUG on purpose, so you can watch it
happen and measure it. (The next lessons hand you the fix.)

Inside `Solution.RunRace()`:

1. Reset `Solution.SharedCounter` to 0.
2. Start `Solution.ThreadCount` new threads (that is 6). Each thread loops
   `Solution.IncrementsPerThread` times (that is 100,000) and does the
   classic racy increment: `Solution.SharedCounter++;`
3. `Join()` EVERY thread, then return the final value of `SharedCounter`.

The checker runs your race up to 3 times and watches for the fingerprint:
a total BELOW the expected 600,000.

## Hints
1. Create the threads in a loop and keep them in a Thread[] so you can Join each one afterwards.
2. The increment must stay plain and racy: SharedCounter++ — no lock, no Interlocked. Breaking it IS the assignment!
3. Miss a Join and RunRace returns while workers are still running — the checker notices the counter moving after your return.
