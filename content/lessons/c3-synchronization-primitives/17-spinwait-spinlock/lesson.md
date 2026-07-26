---
id: c3-l17-spinwait-spinlock
category: c3-synchronization-primitives
order: 17
title: "SpinWait and SpinLock Ã¢â‚¬â€ When Spinning Beats Sleeping"
difficulty: advanced
description: "Learn SpinWait and SpinLock: when a very short wait makes spinning faster than blocking, and when it backfires badly."
explainer: lock-key
interview:
  - q: "When would you use SpinLock instead of lock?"
    a: "When the critical section is extremely short (a few nanoseconds) and contention is low. lock involves a kernel transition (Monitor.Enter eventually calls the OS) Ã¢â‚¬â€ for a one-line atomic swap, the kernel overhead dwarfs the work. SpinLock spins in a tight loop ('voluntary busy-wait') that avoids the kernel Ã¢â‚¬â€ faster for sub-microsecond holds. But if contention is high or the hold time is longer than a few microseconds, SpinLock burns CPU Ã¢â‚¬â€ fall back to lock."
  - q: "What does Thread.SpinWait(int iterations) do?"
    a: "It burns CPU for the specified number of iterations Ã¢â‚¬â€ useful as a tiny delay in lock-free code. Unlike Thread.Sleep(0) (which yields but may immediately reschedule the same thread), SpinWait advances a spin counter and may eventually yield the CPU for longer waits, giving you an adaptive back-off."
---

## What is it?

When locking for 50 nanoseconds, you waste more time entering the kernel than doing the work. `SpinLock` skips the kernel: it sits in a tight CPU loop (`while (locked) { }`) until the lock frees. The gamble: the lock holder finishes in microseconds, so spinning wastes less CPU than the kernel transition would.

`SpinWait` is the yielding spin Ã¢â‚¬â€ it starts with a pure spin, but after a few tries it begins yielding the CPU (`Thread.Sleep(0)`, then `Thread.Yield`, then `Thread.Sleep(1)`), so a long-spinning waiter doesn't starve the system.

## Watch out

> **Never hold a SpinLock across an await or long operation.** Spinning burns a full CPU core Ã¢â‚¬â€ if the holder blocks, other cores spin uselessly. Use SpinLock only for sub-microsecond critical sections.

## Key takeaways

- `SpinLock` Ã¢â€ â€™ busy-waits; faster than lock for ultra-short holds, low contention.
- `SpinWait` Ã¢â€ â€™ adaptive: spin Ã¢â€ â€™ yield Ã¢â€ â€™ sleep as wait time grows.
- In modern .NET, `lock` is already optimised with a spin phase before falling into the kernel Ã¢â‚¬â€ SpinLock is for fine-tuned hot paths.
