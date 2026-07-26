---
id: c4-l04-concurrentdictionary-deep-dive
category: c4-concurrent-collections-and-parallelism
order: 4
title: ConcurrentDictionary Deep Dive Ã Â¢Ã¢â  Â¬ GetOrAdd Is Not Atomic
difficulty: intermediate
description: "Go deep on ConcurrentDictionary: every method, every overload, and the performance characteristics of each."
visualization: thread-timeline
explainer: lock-key
interview:
  - q: If two threads call GetOrAdd with the same missing key at the same moment, what happens?
    a: Both value factories RUN Ã Â¢Ã¢â  Â¬ GetOrAdd does NOT lock; it calls the factory lazily, then uses TryAdd-like CAS to pick a winner. One thread's value is kept, the other's is discarded without being stored. If your factory has side effects (like incrementing a counter), that side effect may happen twice. Use AddOrUpdate or a Lazy<T> wrapper when this matters.
  - q: When should you use ConcurrentDictionary over a regular Dictionary with a lock?
    a: When reads far outnumber writes. ConcurrentDictionary uses fine-grained locking (buckets) so many readers can run in parallel without blocking each other. A global lock around a Dictionary blocks everyone.
---

## What is it?

`ConcurrentDictionary<K,V>` is .NET's built-in thread-safe dictionary. You can call `TryAdd`, `TryGetValue`, `TryUpdate`, `TryRemove` from any thread without an external `lock`. But the *composed* operations Ã Â¢Ã¢â  Â¬ especially `GetOrAdd` Ã Â¢Ã¢â  Â¬ hide a subtle trap that interviewers love.

## The real-world picture

Imagine a phone book shared by 10 receptionists. If Alice asks "what is Bob's extension?" and nobody has written it yet, Alice's manager says "I'll look it up, give me a second." Meanwhile Bob asks "what is Bob's extension?" Ã Â¢Ã¢â  Â¬ another manager also starts looking it up. Both managers do the work. Only one extension ends up in the book. If "looking it up" also sent an email, the email goes out twice.

`GetOrAdd` is exactly like that phone book: the factory (the "look it up") may run more than once, but the dictionary guarantees only one value is stored.

## How it works in C#

```csharp
var dict = new ConcurrentDictionary<string, int>();
int emailsSent = 0;

// 10 threads, each calling GetOrAdd for the same missing key:
Parallel.For(0, 10, _ =>
{
    dict.GetOrAdd("total", key =>
    {
        Interlocked.Increment(ref emailsSent); // side effect!
        return 42;
    });
});

// emailsSent may be > 1 Ã Â¢Ã¢â  Â¬ the factory ran more than once!
Console.WriteLine($"Factory ran {emailsSent} time(s)");
```

The fix for side-effect-heavy factories: wrap the value in `Lazy<T>`:

```csharp
var dict = new ConcurrentDictionary<string, Lazy<int>>();
var lazy = dict.GetOrAdd("total", _ => new Lazy<int>(() => HeavyCompute()));
int result = lazy.Value; // HeavyCompute runs at most once
```

## See it move

Press **Run demo**. Ten threads race to `GetOrAdd` the same key, each factory pings a shared counter. Watch the trace Ã Â¢Ã¢â  Â¬ some factories run, some don't. Then we show the `Lazy<T>` wrapper fixing it.

## Watch out

> **GetOrAdd is thread-safe per-key, not across keys.** Two different keys can run their factories in parallel Ã Â¢Ã¢â  Â¬ that's the point. But if those factories share mutable state, you have a race.

> **Don't put side effects in the factory.** Database calls, logging, incrementing counters Ã Â¢Ã¢â  Â¬ all can happen more than once.

> **AddOrUpdate runs the factory under a lock per key.** If you need exactly-one semantics without Lazy<T>, AddOrUpdate is single-invocation Ã Â¢Ã¢â  Â¬ it uses internal locking per key so the factory runs exactly once per key.

## Key takeaways

- `GetOrAdd` may call the factory multiple times Ã Â¢Ã¢â  Â¬ use `Lazy<T>` to guard side effects.
- `ConcurrentDictionary` uses internal locking per bucket, not a single global lock Ã Â¢Ã¢â  Â¬ great for read-heavy workloads.
- `TryAdd`, `TryGetValue`, `TryUpdate`, `TryRemove` are individually atomic; composed operations need care.
