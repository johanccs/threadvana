---
id: c4-l09-parallel-foreach
category: c4-concurrent-collections-and-parallelism
order: 9
title: "Parallel.ForEach Ã¢â‚¬â€ Beyond Parallel.For"
difficulty: intermediate
description: "Process collections in parallel with Parallel.ForEach: the easiest way to speed up CPU-bound loops on large data sets."
explainer: thread-pool
interview:
  - q: "When is Parallel.ForEach faster than a regular foreach with Task.Run?"
    a: "When you have an in-memory collection (array, list) and the per-item work is CPU-bound and independent. Parallel.ForEach partitions the input into chunks, assigns one chunk per worker thread (avoiding per-item task overhead), and uses work-stealing to balance load across cores. For I/O-bound work, use Task.WhenAll + async instead."
  - q: "Does Parallel.ForEach guarantee ordering of results?"
    a: "No Ã¢â‚¬â€ items are processed in parallel and may complete in any order. If you need ordered results, use PLINQ's AsOrdered() or collect results with an index-based array. ForEach itself provides no ordering."
---

## What is it?

`Parallel.ForEach` splits a collection across all CPU cores, processing items in parallel without you managing threads, tasks, or partitioning. It's the simplest way to say "do this lambda for every item, using all cores."

## Watch out

> **Don't use Parallel.ForEach for I/O.** It blocks pool threads waiting for I/O Ã¢â‚¬â€ use `Task.WhenAll` with async instead.

## Key takeaways

- Partitions input, work-steals, balances across cores.
- Much faster than `foreach` with per-item `Task.Run` for CPU-bound work.
- Use `ParallelOptions { MaxDegreeOfParallelism = N }` to cap concurrency.
