---
id: c4-l16-plinq-pitfalls
category: c4-concurrent-collections-and-parallelism
order: 16
title: "PLINQ Pitfalls - Side Effects and AggregateException"
difficulty: advanced
description: "Avoid the two biggest PLINQ mistakes: side effects that cause data races, and exceptions that get wrapped in AggregateException. Learn the safe patterns for aggregation and error handling in parallel queries."
explainer: race-interleaving
interview:
  - q: "What happens when a PLINQ query throws an exception in one partition?"
    a: "PLINQ wraps all exceptions from all partitions in an AggregateException. Even if 9 threads succeed and 1 fails, you get a single AggregateException containing all thrown exceptions. Catch AggregateException and inspect .InnerExceptions."
  - q: "Why is Thread.Sleep inside PLINQ a bad idea?"
    a: "PLINQ uses a limited number of worker threads (typically ProcessorCount). If every lambda calls Thread.Sleep(1000), you are holding all workers idle doing nothing. PLINQ is for CPU-bound work, not waiting. For I/O, use async + Task.WhenAll."
---

## What is it?

PLINQ is powerful but has two traps that catch almost everyone: **side effects** (mutating shared state inside a parallel lambda) and **AggregateException** (when partitions throw, all exceptions are bundled together). This lesson shows you how to spot these traps and the safe patterns that keep your parallel code correct.

## The real-world picture

**Side effects trap:** Four chefs cook different parts of a meal. They all add their portion to a *single shared scoreboard* by erasing the number and writing a new one. Chef A writes 42. Chef B writes 55 at the same moment. The number ends up 55 but should be 97. That is a race condition - two parallel operations overwrote each other.

**AggregateException trap:** Chef C sets the kitchen on fire. PLINQ does not stop at the first fire - it collects every exception from every partition and bundles them into one `AggregateException`. You need to check `.InnerExceptions` (plural!) to see everything that went wrong.

## How it works in C#

```csharp
using System;
using System.Linq;

// PITFALL 1: Side effect - shared counter races
var counter = 0;
var items = Enumerable.Range(1, 1000).ToList();
items.AsParallel().ForAll(n => { counter++; }); // RACE!
Console.WriteLine($"Counter: {counter}"); // Usually LESS than 1000!

// SAFE: Interlocked for atomic operations
var safeCounter = 0;
items.AsParallel().ForAll(n => { Interlocked.Increment(ref safeCounter); });
Console.WriteLine($"Safe: {safeCounter}"); // Always 1000

// PITFALL 2: AggregateException
try { items.AsParallel().Select(n => {
    if (n == 500) throw new InvalidOperationException("Bad!");
    return n * 2;
}).ToList(); }
catch (AggregateException agg) {
    foreach (var ex in agg.InnerExceptions)
        Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
}
```

## See it move

Press **Run demo**. The interleaving visualization shows shared state races: watch the counter jump as threads overwrite each other. Then see `Interlocked.Increment` - each increment is atomic, the counter always reaches 1000. When an exception fires, watch PLINQ continue processing other partitions before throwing the bundled `AggregateException`.

## Watch out

> **Shared state inside PLINQ without a lock = race.** Use `Interlocked`, `ConcurrentDictionary`, or PLINQ's `.Aggregate()` with thread-local accumulators.

> **AggregateException contains ALL inner exceptions.** Always inspect `.InnerExceptions` (plural!), not `.InnerException`. You could miss a second, different bug.

> **Do not PLINQ I/O-bound work.** Thread.Sleep or HttpClient calls inside PLINQ burn pool threads doing nothing. For I/O, use `async` + `Task.WhenAll`.

## Key takeaways

- No side effects in PLINQ lambdas. Pure transformations only.
- Use `Interlocked` or `ConcurrentDictionary` when you must share state.
- Exceptions become `AggregateException` - check `.InnerExceptions`.
- PLINQ is for CPU-bound work; do not put I/O inside it.