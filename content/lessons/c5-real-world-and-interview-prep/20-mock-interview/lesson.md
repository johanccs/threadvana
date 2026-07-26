---
id: c5-l20-mock-interview
category: c5-real-world-and-interview-prep
order: 20
title: "Mock Interview: Design a Concurrent System"
difficulty: advanced
description: "Full mock interview: answer 10 real multithreading questions under time pressure, then compare with model answers."
explainer: thread-pool
interview:
  - q: "Design a concurrent rate-limited URL shortener. Walk me through the threading decisions."
    a: "(1) URL-to-short-code mapping: ConcurrentDictionary<string,string> for read-heavy O(1) lookups. (2) ID generation: Interlocked.Increment on a counter, base62-encoded. (3) Rate limiting: a ConcurrentDictionary<string, SemaphoreSlim> per caller IP + a background timer pruning expired entries. (4) Async all the way: ASP.NET Core controllers return Task<IActionResult>, no .Result. (5) Shutdown: CancellationToken propagated from the host, backing services honour it. This is what senior-level design answers sound like."
  - q: "What if the service is read-heavy Ã¢â‚¬â€ 99% reads, 1% writes? Does your design change?"
    a: "ReaderWriterLockSlim for the mapping, or a cache-first architecture with a stale-reads TTL. The ConcurrentDictionary already handles reads without locking, so it's fine as-is for the lookup path. The write path is the bottleneck  -  batch DB writes on a background Channel."
---

Final review lesson. Return `"ready"` from `Solution.ReadyForInterview()`.
