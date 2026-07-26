---
id: c4-l13-parallel-vs-async
category: c4-concurrent-collections-and-parallelism
order: 13
title: "Parallel vs Async Ã Â¢Ã¢â  Â¬ CPU-Bound vs I/O-Bound"
difficulty: intermediate
description: "Understand when to use Parallel.ForEach vs Task.WhenAll: CPU-bound parallelism vs I/O-bound concurrency - different tools."
explainer: thread-pool
interview:
  - q: "How do you decide between Parallel.ForEach and Task.WhenAll with async?"
    a: "Answer this question: is each unit of work waiting on the CPU or on external I/O? CPU Ã Â¢Ã¢â ¬Â  Parallel.ForEach (keeps cores busy, synchronous lambdas, threads don't yield). I/O Ã Â¢Ã¢â ¬Â  Task.WhenAll or Parallel.ForEachAsync (threads yield while waiting, other work runs in the gap). Using Parallel.ForEach for I/O starves the pool by blocking threads on network/disk waits."
  - q: "Can you mix Parallel.ForEach with async inside?"
    a: "No Ã Â¢Ã¢â  Â¬ Parallel.ForEach expects an Action<T>, not a Func<T, Task>. Using async void or Task.Run inside it creates unobserved tasks and breaks the parallel barrier. Use Parallel.ForEachAsync instead."
---

## What is it?

The single most important decision in parallel .NET code: is this CPU-bound or I/O-bound?

- **CPU-bound**: the work hammers the CPU (math, sorting, compression) Ã Â¢Ã¢â ¬Â  use `Parallel.For`/`ForEach`, PLINQ, or `Task.WhenAll` with truly CPU-bound `Task.Run` calls.
- **I/O-bound**: the work waits on disk, network, or a database Ã Â¢Ã¢â ¬Â  use `async`/`await` with `Task.WhenAll` or `Parallel.ForEachAsync`.

Getting this wrong is the #1 cause of thread-pool starvation in production.

## Key takeaways

- CPU Ã Â¢Ã¢â ¬Â  synchronous parallelism (`Parallel.ForEach`, PLINQ).
- I/O Ã Â¢Ã¢â ¬Â  async concurrency (`Task.WhenAll`, `Parallel.ForEachAsync`).
- Never mix: `Parallel.ForEach` + `async`. The loop ignores the returned Task.
