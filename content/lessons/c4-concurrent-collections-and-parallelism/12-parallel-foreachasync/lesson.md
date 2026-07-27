---
id: c4-l12-parallel-foreachasync
category: c4-concurrent-collections-and-parallelism
order: 12
title: "Parallel.ForEachAsync  -  the Async-Native Loop"
difficulty: advanced
description: "Combine async work with parallelism: Parallel.ForEachAsync for I/O-bound concurrent processing with controlled concurrency."
explainer: async-state-machine
interview:
  - q: "What is the difference between Parallel.ForEach and Parallel.ForEachAsync?"
    a: "Parallel.ForEach is for synchronous (CPU-bound) work  -  each body runs synchronously on a pool thread. Parallel.ForEachAsync (introduced in .NET 6) is for async (I/O-bound) work  -  each body is an async lambda returning Task, and the loop yields threads while awaiting. Both use MaxDegreeOfParallelism and CancellationToken. Use ForEach for CPU, ForEachAsync for I/O (calling HttpClient, databases, etc.)."
  - q: "When would Parallel.ForEachAsync be a better choice than Task.WhenAll?"
    a: "When you have a huge collection (thousands) and don't want to create thousands of Tasks at once. ForEachAsync processes up to MaxDegreeOfParallelism items concurrently, throttling the in-flight count. Task.WhenAll with 10,000 Task.Runs creates 10,000 Tasks and saturates the pool. ForEachAsync with MaxDegreeOfParallelism=10 keeps exactly 10 in flight."
---

## What is it?

`.NET 6` finally gave us the async-native parallel loop: `Parallel.ForEachAsync`. Each iteration is an `async` lambda, threads yield when awaiting, and `MaxDegreeOfParallelism` caps concurrency. It's the right tool when you need to process thousands of items with async I/O without exploding the task count.

## Watch out

> **It does NOT run iterations in parallel on different threads.** It runs them concurrently — when one iteration awaits, another can start. The actual parallelism depends on the I/O.

## Key takeaways

- `await Parallel.ForEachAsync(items, opts, async (item, ct) => { ... });`
- Caps concurrency properly for async I/O workloads.
- Use for I/O; use sync `Parallel.ForEach` for CPU.
