---
id: c5-l14-pool-starvation
category: c5-real-world-and-interview-prep
order: 14
title: "Thread Pool Starvation Ã Â¢Ã¢â  Â¬ the Silent Slowdown"
difficulty: advanced
description: "Diagnose thread pool starvation: when all pool threads are blocked on async work and nothing can make progress."
explainer: thread-pool
interview:
  - q: "What is thread pool starvation?"
    a: "When all pool threads are blocked (sleeping, waiting on locks, or doing sync I/O), new work queued to the pool (via Task.Run, Timer callbacks, async continuations) cannot start Ã Â¢Ã¢â  Â¬ it sits in the queue until a thread frees up. Symptoms: requests taking longer and longer under load, timeouts at the load balancer, but CPU is idle. The pool grows slowly (roughly 1 thread/sec) so recovery is sluggish."
  - q: "How do you detect thread pool starvation?"
    a: "dotnet-counters monitor --refresh-interval 1 System.Runtime: ThreadPool Thread Count vs ThreadPool Completed Work Item Count. If Thread Count hits max but Completed Work Item Count flatlines, the pool is starved. Also check dotnet-stack for all thread stacks Ã Â¢Ã¢â  Â¬ many threads will be in Monitor.Enter, WaitOne, or synchronous Sleep/Wait calls."
---

Write `Solution.StarveThePoolAsync()` Ã Â¢Ã¢â  Â¬ fire 50 `Task.Run` calls that each do a synchronous `Thread.Sleep(500)`, then return `"starved"`.
