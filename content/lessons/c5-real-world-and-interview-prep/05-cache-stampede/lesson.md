---
id: c5-l05-cache-stampede
category: c5-real-world-and-interview-prep
order: 5
title: "Cache Stampede Protection Ã Â¢Ã¢â  Â¬ the Single-Flight Pattern"
difficulty: advanced
description: "Prevent cache stampedes: when a cached value expires and 100 threads all try to recompute it at the same time."
visualization: async-activity
interview:
  - q: "What is a cache stampede and how do you prevent it?"
    a: "When a cache key expires, 100 concurrent requests all see the miss and EACH starts recomputing the value at the same time Ã Â¢Ã¢â  Â¬ hammering the backend. The fix is the single-flight pattern: use a Lazy<Task<T>> per key. The first request creates the Lazy and starts the fetch; all concurrent requests get the SAME Lazy. The caller awaits the Lazy's Task, which runs the fetch exactly once."
  - q: "Why Lazy<Task<T>> instead of a semaphore?"
    a: "Lazy with ExecutionAndPublication mode runs the factory once and serves the cached result to all concurrent callers. A semaphore serializes each caller Ã Â¢Ã¢â  Â¬ they all queue and wait their turn, but the fetch still runs for each one individually. Lazy gives true single-flight: one fetch, N awaiters."
---

## What is it?

A cache stampede is the thundering-herd problem for caches: expired key Ã Â¢Ã¢â ¬Â  100 simultaneous recomputes. The single-flight pattern wraps the value in `Lazy<Task<T>>` so the first request triggers the recompute and all others wait for the SAME lazy Task to complete.

## How it works

```csharp
var cache = new ConcurrentDictionary<string, Lazy<Task<Data>>>();
var lazy = cache.GetOrAdd(key, _ => new Lazy<Task<Data>>(() => FetchFromDbAsync(key)));
var data = await lazy.Value;
```

## Key takeaways

- `Lazy<Task<T>>` Ã Â¢Ã¢â ¬Â  one fetch, all concurrent callers share the same Task.
- `ConcurrentDictionary.GetOrAdd` with Lazy ensures key is created once.
- Protects backend from thundering-herd recomputation on cache expiry.
