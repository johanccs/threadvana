Save the vanishing counter.

`Solution.CountToTen()` is provided: it counts from 1 to 10, one tick every
50ms (about 500ms in total), storing the latest number in `Solution.Counter`.

`Solution.Run()` already contains code - but it is BROKEN on purpose. It starts
the counting on a **background** thread and returns immediately. In a real
program, that worker would be cut off mid-count the moment the program exits.

Your fix:

1. Change `Run()` so the counting is GUARANTEED finished (`Counter == 10`)
   before `Run()` returns.
2. One well-placed line is enough. (You may also delete the `IsBackground`
   line - but notice that on its own it does NOT make `Run()` wait!)

## Hints
1. Foreground vs background decides who keeps the PROCESS alive - it never makes a METHOD wait. Waiting is Join's job.
2. Add `worker.Join();` as the last line of `Run()`.
3. If `Counter` reaches 10 "eventually" but not when `Run()` returns, you started the work but still are not waiting for it.
