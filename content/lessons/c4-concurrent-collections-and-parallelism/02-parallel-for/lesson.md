---
id: c4-l02-parallel-for
category: c4-concurrent-collections-and-parallelism
order: 2
title: Parallel.For Ã Â¢Ã¢â  Â¬ Split Work Across Cores
difficulty: intermediate
description: "Replace your for loop with Parallel.For and let .NET split iterations across all CPU cores automatically."
visualization: thread-pool
interview:
  - q: When is Parallel.For a win and when should you avoid it?
    a: A win when iterations are CPU-heavy and independent (image processing, number crunching, parsing large documents). Avoid when iterations are tiny (< 1ms each Ã Â¢Ã¢â  Â¬ overhead dominates), depend on each other (need sequential order), or are I/O-bound (use async).
  - q: How does Parallel.For partition the work?
    a: It divides the iteration range into chunks and assigns chunks to pool threads. The chunk size and number of threads are tuned automatically based on cores and workload. You can control it with ParallelOptions but rarely need to.
---

Parallel.For splits a loop across multiple pool workers. The demo squares 5 million
numbers both ways Ã Â¢Ã¢â  Â¬ sequential and parallel Ã Â¢Ã¢â  Â¬ and compares the elapsed time on screen.
The exercise has you convert a sequential loop to Parallel.For and observe the speedup.
