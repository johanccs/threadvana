Put a bouncer in front of the API.

`Solution.CallApiAsync()` is provided: it pretends to be a slow API (~200 ms)
and reports every enter/exit to the checker's counters. The starter's
`CallApiLimitedAsync()` just calls it directly â   no limit at all â   so when the
checker starts 6 calls at once, all 6 pile in together.

Your job â   give the API a parking lot with 2 spaces:

1. Add the semaphore: `public static SemaphoreSlim Lot = new SemaphoreSlim(2);`
2. Rewrite `CallApiLimitedAsync()`:
   - `await Lot.WaitAsync();` â   drive in (or queue at the entrance).
   - `try { await CallApiAsync(); }`
   - `finally { Lot.Release(); }` â   ALWAYS drive out, even on a crash.

The checker starts 6 calls at once and watches: never more than 2 inside, all
6 finish, and NOT serialized to one-at-a-time either â   that would be a lock,
not a semaphore!

## Hints
1. `new SemaphoreSlim(2)` creates 2 permits; `WaitAsync()` takes one, `Release()` gives it back.
2. The `finally` block is the whole trick: if `Release()` is skipped or lost to an exception, a space is gone forever and the queue never moves again.
3. The full pattern is: `await Lot.WaitAsync(); try { await CallApiAsync(); } finally { Lot.Release(); }`