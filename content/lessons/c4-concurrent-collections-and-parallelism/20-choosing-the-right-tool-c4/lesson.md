---
id: c4-l20-choosing-the-right-tool-c4
category: c4-concurrent-collections-and-parallelism
order: 20
title: "Choosing the Right Tool  -  Collections vs Channels vs Parallel"
difficulty: advanced
description: "Decision guide for concurrent collections and parallelism: which tool for which scenario? Collection, channel, parallel loop, or PLINQ?"
explainer: channel
interview:
  - q: "You need to process 10,000 items, each taking 5ms of CPU. ConcurrentQueue + Task.Run or Parallel.ForEach?"
    a: "Parallel.ForEach  -  it partitions the work, avoids per-item task overhead, and auto-balances across cores. ConcurrentQueue + Task.Run creates 10,000 Tasks (or fewer if throttled) but doesn't partition and has queue contention. For pure CPU work, Parallel.ForEach or PLINQ is the answer."
  - q: "What about 10,000 I/O-bound items (each calling an API)?"
    a: "Parallel.ForEachAsync with MaxDegreeOfParallelism=N, or Task.WhenAll with a throttler (SemaphoreSlim). Never Parallel.ForEach for I/O  -  it blocks pool threads. Channels also work well as the producer-consumer fabric for streaming I/O work."
---

## What is it?

The c4 decision map Ã¢â‚¬â€ when to use which concurrent data structure and processing pattern.

## The decision map

| Scenario | Tool |
|----------|------|
| Thread-safe dictionary, many reads | `ConcurrentDictionary` |
| Producer-consumer, async, bounded | `Channel<T>` (bounded) |
| Producer-consumer, sync, bounded | `BlockingCollection<T>` |
| FIFO order, async | `Channel<T>` or `ConcurrentQueue` |
| LIFO order | `ConcurrentStack<T>` |
| No order, same-thread add/take | `ConcurrentBag<T>` |
| CPU-bound bulk processing | `Parallel.ForEach` or PLINQ |
| I/O-bound bulk processing | `Parallel.ForEachAsync` or `Task.WhenAll` |
| Lock-free hot path | CAS loop (`Interlocked.CompareExchange`) |

## Key takeaways

- CPU Ã¢â€ â€™ partition; I/O Ã¢â€ â€™ async + throttle.
- Order matters Ã¢â€ â€™ queue/stack; order doesn't Ã¢â€ â€™ bag/channel.
- Dictionary Ã¢â€ â€™ ConcurrentDictionary. Simple Ã¢â€ â€™ start with lock.
