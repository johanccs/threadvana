---
id: c4-l19-lock-free-cas
category: c4-concurrent-collections-and-parallelism
order: 19
title: "Lock-Free Basics - Interlocked.CompareExchange and the CAS Loop"
difficulty: advanced
description: "Go under the hood of lock-free programming with CAS (Compare-And-Swap). Learn the CAS loop pattern, when it beats a lock, and when it makes things worse. This is how ConcurrentQueue and ConcurrentDictionary work internally."
explainer: race-interleaving
interview:
  - q: "What is a CAS loop?"
    a: "CAS = Compare-And-Swap. Interlocked.CompareExchange atomically compares a memory location with an expected value - if they match, it replaces with the new value. The CAS loop pattern: read current, compute new, attempt CAS, retry on failure. This is how ConcurrentQueue and ConcurrentDictionary are built."
  - q: "When would you write your own CAS loop instead of using lock?"
    a: "When the critical work is a single Interlocked operation or a simple transformation. A lock is simpler and more maintainable 95% of the time - only use CAS when profiling shows lock contention is a bottleneck."
---

## What is it?

Lock-free programming uses atomic CPU instructions instead of OS-level locks. The central primitive in .NET is `Interlocked.CompareExchange` - the hardware-level Compare-And-Swap (CAS). It atomically reads a value, compares it to what you expected, and if they match, writes a new value. If not (someone else changed it), it tells you the actual current value. The CAS loop wraps this in a retry loop: read, compute, try to swap, retry on failure.

## The real-world picture

A bank has a shared account balance on a screen. Two tellers process deposits at once. Both read $100. Teller A computes $150. Teller B computes $130. Without protection, whoever writes last wins - $50 disappears. With CAS: Teller A walks up - "Swap $100 with $150?" Balance is $100, swap succeeds (now $150). Teller B walks up - "Swap $100 with $130?" Balance is $150, not $100 - swap fails! Teller B rereads ($150), recomputes ($180), tries again. Both deposits counted. No lock held, yet no updates lost. Cost: B had to retry.

## How it works in C#

```csharp
using System.Threading;

int sharedValue = 0;

// Manual CAS loop
void IncrementSafely() {
    int current = Volatile.Read(ref sharedValue);
    while (true) {
        int next = current + 1;
        int original = Interlocked.CompareExchange(
            ref sharedValue, next, current);
        if (original == current) break;  // success
        current = original;              // retry
    }
}

// Built-in: Interlocked.Increment does the CAS loop for you
Interlocked.Increment(ref sharedValue);

// Custom: multiply by 2
int MultiplyByTwo() {
    int current = Volatile.Read(ref sharedValue);
    while (true) {
        int next = current * 2;
        int original = Interlocked.CompareExchange(
            ref sharedValue, next, current);
        if (original == current) return next;
        current = original;
    }
}
```

## See it move

Press **Run demo**. Watch multiple threads attempt CAS on the same variable. When two threads collide (both try to swap from the same expected value), one succeeds and the other retries. The retry count shows contention - more retries = more threads fighting for the same variable.

## Watch out

> **CAS loops + high contention = retry storms.** If 20 threads CAS at once, one succeeds and 19 retry. They retry, 18 retry again, burning CPU. A lock can be faster than a retry storm. Measure.

> **The CAS loop body must be cheap.** Your transformation runs every retry. If it allocates or takes more than nanoseconds, retry cost explodes. CAS is for arithmetic, not business logic.

> **Volatile.Read ensures fresh values.** Without it, the JIT might cache the variable in a register and never see updates from other threads.

## Key takeaways

- `Interlocked.CompareExchange(ref t, newVal, comparand)` swaps if target equals comparand.
- CAS loop pattern: read, compute, CAS, retry on failure.
- `Interlocked.Increment/Decrement/Add` are built-in CAS wrappers - use them directly.
- Lock-free does NOT mean always faster - measure contention.
- CAS is the foundation of all concurrent collections.