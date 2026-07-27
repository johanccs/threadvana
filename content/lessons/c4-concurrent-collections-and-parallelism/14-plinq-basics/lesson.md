---
id: c4-l14-plinq-basics
category: c4-concurrent-collections-and-parallelism
order: 14
title: "PLINQ Basics - AsParallel on Your LINQ"
difficulty: intermediate
description: "Learn how to turn any LINQ query into a parallel one by adding .AsParallel(). See how PLINQ splits a list across all your CPU cores, processes each chunk at the same time, and merges the results back - all with zero changes to your Where/Select logic."
explainer: thread-pool
interview:
  - q: "How do you turn a LINQ query into a parallel one?"
    a: "Add .AsParallel() before the query operators. PLINQ partitions the source collection across cores, processes in parallel, and merges results. The query syntax is identical - just the execution model changes. For CPU-bound work on large collections, AsParallel can give near-linear speedups."
  - q: "When does PLINQ fall back to sequential execution?"
    a: "When the query is too small to justify parallel overhead, when the operators are not parallel-safe (e.g., Take, SkipWhile, or custom operators that rely on order), or when parallelism would likely be slower than sequential. PLINQ uses heuristics and you can force parallelism with .WithExecutionMode(ParallelExecutionMode.ForceParallelism)."
---

## What is it?

PLINQ (Parallel LINQ) is the parallel twin of LINQ-to-Objects. Add .AsParallel() to any IEnumerable<T> and the operators after it - Where, Select, Aggregate - run across all your CPU cores at the same time. You write the same LINQ you already know. PLINQ handles splitting the data, handing chunks to different threads, and merging the results.

Think of it like this: normal LINQ is one chef cooking one dish at a time. PLINQ is a kitchen with four chefs, each cooking a quarter of the meal. The recipe (your lambda) has not changed - but dinner arrives faster because the work was shared.

## The real-world picture

You have a folder with 10,000 images. You need to resize every one of them. A normal foreach loop resizes image 1, then image 2, then image 3 - one at a time, one CPU core doing all the work while the other seven cores sit idle. With PLINQ, the 10,000 images are split into chunks - maybe 2,500 per core on a 4-core machine. All four cores resize images at the same time. The work finishes in roughly a quarter of the time.

But there is a catch: if you only have 10 images, the time spent splitting the list and merging results might be longer than just doing it one at a time. PLINQ is for **big, CPU-heavy work**, not tiny collections.

## How it works in C#

`csharp
using System;
using System.Linq;
using System.Diagnostics;

// A list of numbers we want to process
var numbers = Enumerable.Range(1, 10_000_000).ToList();

// Normal LINQ - one thread, sequential
var sw = Stopwatch.StartNew();
var evens = numbers.Where(n => n % 2 == 0).ToList();
sw.Stop();
Console.WriteLine($"Sequential: {sw.ElapsedMilliseconds} ms");

// PLINQ - all cores, parallel (just add .AsParallel()!)
sw.Restart();
var evensParallel = numbers.AsParallel()
    .Where(n => n % 2 == 0)
    .ToList();
sw.Stop();
Console.WriteLine($"PLINQ:       {sw.ElapsedMilliseconds} ms");
`

Three things to notice:
- The only change is .AsParallel() - the Where logic stayed exactly the same.
- Results are **unordered by default** (items may come back in any order). Add .AsOrdered() if you need the original sequence.
- PLINQ decides for itself how many threads to use. You can override it with .WithDegreeOfParallelism(N).

## See it move

Press **Run demo**. The thread-pool visualization lights up: watch multiple worker threads pick up chunks of the list at the same time. Compare that to a sequential run - only one thread works while the others stay dark. The total time shrinks when threads share the load.

## Watch out

> **Side effects in PLINQ are danger.** A Select that increments a shared counter without a lock will race. PLINQ is for **pure** (side-effect-free) transformations. If you must aggregate, use the thread-safe overloads like .Aggregate(seed, func, resultSelector) - never modify shared state inside a PLINQ lambda.

> **PLINQ can be SLOWER for small collections.** The partitioning overhead dwarfs tiny per-item work. If your list has 100 items and each item takes a microsecond to process, the cost of splitting the list across threads is higher than the work itself. Always measure.

> **Not every LINQ operator plays nice with parallel.** Take() and SkipWhile() rely on order, which parallel execution does not guarantee. PLINQ handles this internally but may fall back to sequential for those operators. Trust it - or force parallelism with .WithExecutionMode(ParallelExecutionMode.ForceParallelism) if you know what you are doing.

## Key takeaways

- .AsParallel() turns any LINQ-to-Objects query into a parallel one - same syntax, parallel execution.
- PLINQ splits your collection into chunks; each chunk runs on a different thread.
- Results are **unordered by default**; use .AsOrdered() only when you need the original order.
- Pure functions only - no side effects inside PLINQ lambdas.
- PLINQ is for **big, CPU-bound** collections. For small or I/O-bound work, use sync/wait + Task.WhenAll instead.