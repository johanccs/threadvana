Give every thread its own counting space.

The starter has a shared `static int _counter` that both threads fight over â   they
overwrite each other's work.

Your job: replace it with **thread-local storage** so each thread increments its OWN
copy of the counter. Use `[ThreadStatic]` on `Solution.Counter`.

The provided code starts two threads that call `Solution.Increment()` (increments the
counter) and then stores the value they saw into `Solution.Results[threadIndex]`.

After your fix: the harness runs from 2 threads. Each thread should see its own
independent count (each increments once â   each sees 1, not 2).

## Hints
1. Add `[ThreadStatic]` before the `public static int Counter` field. That's it.
2. With `[ThreadStatic]` the default value is 0 for every thread â   which is exactly
   what a counter starts at. No initialiser needed.
3. The starter code is the same as the solution â   you literally just add
   `[ThreadStatic]` and everything works.
