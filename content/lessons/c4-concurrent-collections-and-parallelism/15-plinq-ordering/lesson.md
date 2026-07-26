---
id: c4-l15-plinq-ordering
category: c4-concurrent-collections-and-parallelism
order: 15
title: "PLINQ Ordering - AsOrdered Costs, ForAll Wins"
difficulty: intermediate
description: "Understand the trade-off between ordered and unordered PLINQ results. Learn when AsOrdered is worth its performance cost, and when ForAll gives you the fastest parallel output by skipping the merge step entirely."
explainer: thread-pool
interview:
  - q: "What does AsOrdered() do in PLINQ?"
    a: "It forces the output to preserve the original input order - items come out in the same sequence they went in. This requires buffering and reordering, which adds overhead. Use it only when the consumer genuinely needs ordered results."
  - q: "What is ForAll?"
    a: "It runs an action on each output element in parallel without merging results into a single enumerator. It is faster than foreach on the PLINQ result because it avoids the merge/ordering overhead entirely."
---

## What is it?

PLINQ gives you two choices for how results come back: **ordered** (.AsOrdered()) or **unordered** (the default). Ordered means items appear in the same sequence as the original list. Unordered means whichever chunk finishes first outputs first. The difference is speed: ordering requires PLINQ to buffer results and sort them back into position before you see them, which costs time and memory.

.ForAll() takes this one step further: it runs your action on each result *as soon as it is ready*, in whatever thread processed it. No merging, no enumerator, no buffering - just fire and go.

## The real-world picture

Imagine you are a teacher grading 100 exams. You hand stacks of 25 to four teaching assistants. **Unordered (default PLINQ):** each TA grades their stack and hands papers back as they finish. You get paper 73 before paper 2 - who cares, they are all graded. **Ordered (AsOrdered):** each TA grades, but then someone has to collect all 100 papers and sort them back into alphabetical order before you see them. That sorting step takes extra time. **ForAll:** each TA grades AND enters the score directly into the gradebook. No one collects or sorts anything.

## How it works in C#

`csharp
using System;
using System.Linq;

var items = Enumerable.Range(1, 10).ToList();

// UNORDERED (default) - fastest
var unordered = items.AsParallel().Select(n => n * 10).ToList();
// Output may be: 30, 10, 50, 20, 40, 70, 90, 60, 80, 100

// ORDERED - costs buffering
var ordered = items.AsParallel().AsOrdered().Select(n => n * 10).ToList();
// Output: 10, 20, 30, 40, 50, 60, 70, 80, 90, 100

// ForAll - no merge, runs directly on worker threads
items.AsParallel().ForAll(n =>
    Console.WriteLine($"Processing {n} on thread {Environment.CurrentManagedThreadId}"));
`

## See it move

Press **Run demo**. Watch the thread-pool visualization: when unordered, results stream back as each thread finishes. With AsOrdered, notice the pause at the end - PLINQ is buffering and sorting. With ForAll, results appear immediately on the worker threads themselves with no merge step.

## Watch out

> **AsOrdered + ForAll = no effect.** ForAll ignores ordering entirely. If you need ordered output, use oreach on the final IEnumerable, not ForAll.

> **AsOrdered on giant collections can eat memory.** PLINQ must hold all results in a buffer to reorder them. If you are processing millions of items, the buffer can grow large.

## Key takeaways

- AsOrdered() forces output to preserve input order (costs buffering and time).
- ForAll() runs an action on each result in parallel, no merge overhead.
- Default PLINQ is UNordered - fastest path.
- For aggregation (Sum, Average), skip ordering - the final number does not depend on order.