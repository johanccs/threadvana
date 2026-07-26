Build the worker loop.

A shared `Queue<int>` and a `volatile bool` stop flag are already provided. Two
worker threads exist but their loop body is empty (just a TODO).

Complete the worker loop inside each worker thread so that:
1. Under `lock (Solution.Gate)`: if the queue has items, Dequeue one and set a local
   `hasItem = true`. If the queue is empty AND `Solution.ShouldStop` is true, break
   out of the loop (the work is done).
2. After the lock: if `hasItem`, process it by calling `ProcessItem(item)` (provided).
   Otherwise sleep 1 ms so you don't burn CPU.

## Hints
1. The lock pattern: `lock (Gate) { if (queue.Count > 0) { item = queue.Dequeue(); hasItem = true; } else if (ShouldStop) break; }`
2. After the lock: `if (hasItem) ProcessItem(item); else Thread.Sleep(1);`
3. The starter already creates the threads and starts the producer â   you just fill in the worker loop body.
