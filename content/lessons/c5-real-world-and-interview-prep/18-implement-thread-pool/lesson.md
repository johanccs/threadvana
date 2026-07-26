---
id: c5-l18-implement-thread-pool
category: c5-real-world-and-interview-prep
order: 18
title: "Interview: Implement a Tiny Thread Pool"
difficulty: advanced
description: "Build a miniature thread pool: understand work queues, worker threads, the dispatch loop, and how tasks get scheduled."
explainer: thread-pool
interview:
  - q: "Design a simple thread pool from scratch."
    a: "Ingredients: a BlockingCollection<Action> as the work queue, N threads (new Thread) each looping on queue.Take() and executing the action. AddWork(Action action) enqueues. Shutdown: call CompleteAdding and Join all threads. This is essentially what the real ThreadPool does, minus work-stealing, hill-climbing, and async support."
---

Write `Solution.TinyPool` with `Start(int workerCount)` (starts N threads draining `BlockingCollection`) and `QueueWork(Action work)`. The harness starts 2 workers, queues 4 actions incrementing a counter, waits for completion.
