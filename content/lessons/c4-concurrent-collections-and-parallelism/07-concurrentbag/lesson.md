---
id: c4-l07-concurrentbag
category: c4-concurrent-collections-and-parallelism
order: 7
title: "ConcurrentBag  -  When Order Doesn't Matter"
difficulty: beginner
description: "Understand ConcurrentBag: a thread-local-backed bag optimized for same-thread produce-and-consume patterns."
explainer: lock-key
interview:
  - q: "What is ConcurrentBag and when is it useful?"
    a: "It is an unordered, thread-safe collection optimised for the producer-consumer pattern where the SAME thread both adds and removes items. Internally, each thread gets a local list  -  adding uses ThreadLocal storage (fast, no contention), and taking first checks the local list before stealing from other threads' lists. It is fastest when each thread adds and then takes its own items; slowest when a single consumer takes from many producers (lots of stealing)."
  - q: "How is ConcurrentBag different from ConcurrentQueue for a producer-consumer scenario?"
    a: "ConcurrentQueue has predictable FIFO ordering and performs well with mixed producers/consumers. ConcurrentBag has no ordering guarantee and is optimised for when producers and consumers are the SAME threads (e.g., a pool where workers queue up work items and then process them). For a classic multi-producer, multi-consumer queue, use ConcurrentQueue or Channel."
---

## What is it?

`ConcurrentBag<T>` is the "I don't care about order" collection. Threads doing both enqueue and dequeue get thread-local speed (zero contention). Items interleaved by different threads are stored in per-thread lists; taking steals from others' lists when the local list is empty.

## Watch out

> **No ordering guaranteed.** Two calls to `Add(1)` then `Add(2)` from the same thread do NOT guarantee 1 comes out before 2 under heavy stealing.

## Key takeaways

- Per-thread local storage Ã¢â€ â€™ fast for add+take on the same thread.
- Steals from other threads when local is empty Ã¢â‚¬â€ best for work-stealing patterns.
- No FIFO/LIFO guarantee Ã¢â‚¬â€ use `ConcurrentQueue` when order matters.
