Count the couriers.

`Solution.DoTask(int id)` is provided: it works ~100ms, then records WHICH
thread did it in `Solution.TaskThreadIds` (a `ConcurrentDictionary`: task id
-> thread id) and signals `Solution.Done` (a `CountdownEvent` starting at 8).

Inside `Solution.Run()`:

1. Queue 8 tasks on the thread pool - one per id, 1 to 8:
   `ThreadPool.QueueUserWorkItem(_ => DoTask(id));`
   (A loop is perfect here - but remember the capture trap and give each
   task its OWN copy of the loop variable!)
2. Wait until all 8 are done: `Solution.Done.Wait();`

The checker counts how many DISTINCT threads did your 8 tasks. Borrowed the
on-call team? 1-4 distinct ids. Hired privately? One new face per task.

## Hints
1. `for (int i = 1; i <= 8; i++)` with `int mine = i;` inside - then `DoTask(mine)`.
2. Do not forget `Done.Wait();` at the end - pool threads cannot be Joined.
3. If the checker says ids are missing and mentions the number 9, your tasks all grabbed the same loop variable - the capture trap again.
