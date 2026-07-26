---
id: c4-l06-concurrent-queue-stack
category: c4-concurrent-collections-and-parallelism
order: 6
title: "ConcurrentQueue & ConcurrentStack  -  Ordering Guarantees"
difficulty: beginner
description: "Explore ConcurrentQueue and ConcurrentStack: lock-free FIFO and LIFO collections for high-throughput scenarios."
explainer: lock-key
interview:
  - q: "What ordering does ConcurrentQueue guarantee?"
    a: "First-In-First-Out (FIFO)  -  items are dequeued in the order they were enqueued. But this guarantee is PER PRODUCER  -  if two threads enqueue A then B simultaneously, the dequeue order may be A,B or B,A depending on which producer's CAS won first. Within a single producer, order is preserved."
  - q: "Why use ConcurrentStack over ConcurrentQueue?"
    a: "ConcurrentStack is Last-In-First-Out (LIFO)  -  like a push/pop stack. It can be faster because it only touches the head (no tail pointer). Use when order doesn't matter but throughput does  -  e.g., a work-stealing pool where workers pop their own queue (LIFO) and steal from others (FIFO-end)."
---

## What is it?

`ConcurrentQueue<T>` and `ConcurrentStack<T>` are lock-free, thread-safe collections with different ordering guarantees: queue = FIFO (line), stack = LIFO (pile). Both use lock-free algorithms (CAS loops) and avoid the global lock of a `lock`-guarded `List<T>`.

## Watch out

> **TryDequeue/TryPop return false when empty Ã¢â‚¬â€ never throw.** Always check the return value. A `while (q.TryDequeue(out var item))` loop is the standard consume pattern.

## Key takeaways

- `ConcurrentQueue` Ã¢â€ â€™ FIFO, lock-free, safe for multiple producers and consumers.
- `ConcurrentStack` Ã¢â€ â€™ LIFO, lock-free, slightly faster.
- Always use `TryDequeue`/`TryPop`/`TryPeek` Ã¢â‚¬â€ never index or enumerate while mutating.
