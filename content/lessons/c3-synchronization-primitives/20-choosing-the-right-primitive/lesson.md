---
id: c3-l20-choosing-the-right-primitive
category: c3-synchronization-primitives
order: 20
title: "Choosing the Right Primitive  -  the Decision Map"
difficulty: advanced
description: "Decision guide: which synchronization primitive for which scenario? Lock vs semaphore vs event vs barrier - a flowchart."
explainer: lock-key
interview:
  - q: "You have a shared counter incremented from many threads. What primitive do you use and why?"
    a: "Interlocked.Increment  -  it is lock-free, the fastest option, and semantically exact for a counter. No lock object needed, no kernel transitions. If the counter is part of a larger invariant (e.g., Balance -= amount), fall back to lock or SpinLock for a short critical section."
  - q: "You need to limit concurrent I/O calls to 5. What primitive?"
    a: "SemaphoreSlim(5) with WaitAsync. It blocks cooperatively (async-friendly), the count matches the limit naturally, and no kernel transitions for the common case. Semaphore (OS-level) is overkill unless cross-process."
---

## What is it?

You now know 10+ synchronisation primitives. The interview test is choosing the RIGHT one fast. This lesson maps use case to primitive.

## The decision map

| Scenario | Primitive | Why |
|----------|-----------|-----|
| Short exclusive access, one thread | `lock` | Simple, safe, fast enough |
| Ultra-short hold, high frequency | `SpinLock` | Avoids kernel overhead |
| Limit concurrency to N | `SemaphoreSlim` | Built-in count, async-friendly |
| Many readers, rare writes | `ReaderWriterLockSlim` | Reads run in parallel |
| Wait for N signals | `CountdownEvent` | Exactly N countdowns |
| Sync phases across threads | `Barrier` | All arrive → continue |
| Simple true/false gate | `ManualResetEventSlim` | Set/Reset, multiple waiters |
| One-shot per signal | `AutoResetEvent` | Turnstile pattern |
| Cross-process lock | `Mutex` | Kernel-level, named |
| Atomic counter | `Interlocked` | Lock-free, fastest |
| Visibility flag | `volatile` | No caching, read/write barrier |
| Per-thread isolation | `ThreadLocal<T>` | No locking at all |
| Per-request context | `AsyncLocal<T>` | Flows across awaits |
| Async critical section | `SemaphoreSlim(1,1)` | lock + await don't mix |

## Key takeaways

- Map the PROBLEM to the primitive — don't reach for lock by default.
- Interlocked for counters; SemaphoreSlim for concurrency limits.
- lock is fine 95% of the time — the map is for the other 5%.
