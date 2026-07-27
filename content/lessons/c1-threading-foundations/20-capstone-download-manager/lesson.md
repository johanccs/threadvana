---
id: c1-l20-capstone-download-manager
category: c1-threading-foundations
order: 20
title: Capstone  -  Mini Download Manager
difficulty: advanced
description: "Capstone project: build a multithreaded download manager that downloads multiple files in parallel and reports progress."
visualization: thread-pool
explainer: thread-pool
interview:
  - q: How would you design a system where N worker threads process items from a shared queue?
    a: "Use a shared queue (ConcurrentQueue or BlockingCollection) and start N dedicated threads. Each worker loops  -  dequeue, process, repeat. The main thread enqueues work and signals when done. Shut down cooperatively when the queue is empty and the producer is finished."
  - q: What happens if a worker thread crashes?
    a: The other workers keep running  -  they are independent. But the crashed worker's item may be lost unless you catch exceptions inside the worker loop and re-queue or log the failure. Use try/catch in the worker body.
---

## What is it?

You have built 19 lessons of threading skills. This capstone puts them together: a
mini "download manager" with a shared queue and a pool of worker threads.

One producer thread enqueues 12 work items. Four worker threads dequeue and
"process" them (represented by a short sleep). The system shuts down cleanly when
all work is done and the workers are told to stop.

## The design

```
Main thread ──▶ enqueue(12 items) ──▶ set stop flag ──▶ Join all workers
                    │
                    ▼
[shared queue] ──▶ Worker 1 ──▶ Worker 2 ──▶ Worker 3 ──▶ Worker 4
```

Each worker: `while (items to process OR stop flag not set) { dequeue; process; }`

This uses: `new Thread` (dedicated workers), a shared queue, a `lock` for the
dequeue, and a cooperative stop flag — almost everything from Category 1.

## How it works in C#

```csharp
var queue = new Queue<int>();
var shouldStop = false;
var gate = new object();

// Worker loop
new Thread(() =>
{
    while (true)
    {
        int item;
        lock (gate)
        {
            if (queue.Count == 0 && shouldStop) break;
            if (queue.Count == 0) continue;
            item = queue.Dequeue();
        }
        Process(item);
    }
});
```

## See it move

Press **Run demo**. Watch 4 workers consuming 12 items from the shared queue.
Notice how workers overlap — while one processes, another dequeues the next item.

## Watch out

- Without the stop flag, workers loop forever even after the queue is empty.
- `queue.Count == 0` followed by `Dequeue` must be inside the SAME lock. If you
  check outside and dequeue inside, another worker might grab it in between.
- Worker loops burn CPU if they busy-wait. Adding a short `Thread.Sleep(1)` when
  the queue is empty saves CPU while still being responsive.

## Key takeaways

- Multiple workers sharing a queue is the classic multithreading pattern.
- A lock around enqueue/dequeue keeps the queue safe.
- A cooperative stop flag lets workers exit cleanly.
- You just built a real, working thread system end-to-end.
