---
id: c1-l18-race-sneak-preview
category: c1-threading-foundations
order: 18
title: Race Conditions Ã¢â‚¬â€ a Sneak Preview
difficulty: intermediate
description: "A sneak preview of race conditions: the most common and sneaky bug in multithreaded code, where timing determines correctness."
visualization: thread-timeline
explainer: race-interleaving
interview:
  - q: What is a race condition?
    a: When two or more threads access shared data and the result depends on who gets there first (the timing "race"). A classic example is two threads doing counter++ on a shared variable Ã¢â‚¬â€ the increments can be lost because ++ is not one atomic operation.
  - q: How do you fix a race condition?
    a: "With synchronization Ã¢â‚¬â€ a lock, Interlocked operations, or thread-safe data structures like ConcurrentDictionary. Category 3 covers these in depth."
---

## What is it?

Two threads both grab the same shared counter, read "5", add one, write "6". Both
write 6. Execution order: both saw 5 before either wrote Ã¢â€ â€™ one increment is LOST.

The root cause: `counter++` is actually three steps Ã¢â‚¬â€
1. Read the value from memory.
2. Add 1 to it.
3. Write the new value back.
The OS can swap threads between ANY of those steps.

This lesson shows you the problem. Category 3 shows you the solutions (lock,
Interlocked, volatile, etc.).

## The real-world picture

Two waiters both grab the last reservation from the shared clipboard. Both read
"Table 7", cross it out, and lead their customers to the same table. Awkward.

## How it works in C#

```csharp
private static int _counter = 0;

// Two threads, each incrementing 50,000 times.
// Expected sum: 100,000. Actual: something less (and different every time!).
```

No lock, no Interlocked Ã¢â‚¬â€ just a raw, unprotected `_counter++`. Run the demo and
see the damage.

## See it move

Press **Run demo**. Two threads both hammer the same counter. The final total is
shown Ã¢â‚¬â€ it's always less than 100,000 (and the exact number changes every run).

## Watch out

- A race CAN pass every time in a small test and fail in production under load.
  That's what makes it the hardest bug to catch.
- `volatile` alone does NOT make `counter++` atomic. You need `lock` or
  `Interlocked.Increment` (Category 3).
- The debugger makes races appear to disappear Ã¢â‚¬â€ it pauses time.

## Key takeaways

- A race happens when two threads touch the same data without coordination.
- `counter++` is NOT one step Ã¢â‚¬â€ it's read, add, write.
- Races are non-deterministic: the result changes every run.
- Category 3 gives you all the tools to fix them.
