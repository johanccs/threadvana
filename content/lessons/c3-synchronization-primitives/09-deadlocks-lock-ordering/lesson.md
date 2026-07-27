---
id: c3-l09-deadlocks-lock-ordering
category: c3-synchronization-primitives
order: 9
title: "Deadlocks and Lock Ordering  -  Prevention by Convention"
difficulty: advanced
description: "Diagnose and prevent deadlocks: when two threads each hold a lock the other needs, and both freeze forever."
visualization: thread-timeline
explainer: deadlock
interview:
  - q: "What causes a deadlock with locks?"
    a: "Two or more threads each hold a lock the other needs  -  a circular wait. Thread A locks X then tries Y; Thread B locks Y then tries X. Both wait forever. The fix is a consistent lock ORDERING: both threads must always acquire X first, then Y. If the order must differ, acquire both under one larger-scope lock, or use Monitor.TryEnter with a timeout as a back-out mechanism."
  - q: "How do you detect a deadlock in production?"
    a: "dotnet-dump or PerfView can capture the thread stacks of a live process  -  you'll see threads waiting on Monitor.Enter or WaitOne, and you can match lock addresses to find the circular dependency. Visual Studio's Parallel Stacks window visualises this. The best defence is consistent lock ordering + keeping the number of locks small."
---

## What is it?

A deadlock is the ultimate circular dependency: thread A holds lock #1 and wants lock #2; thread B holds lock #2 and wants lock #1. Neither can progress. The process freezes — no exceptions, no crashes, just a silent hang.

## The classic deadlock scenario

```csharp
// Thread A
lock (lockA) { lock (lockB) { Transfer(); } }

// Thread B — opposite order!
lock (lockB) { lock (lockA) { Transfer(); } }

// If both start at the same moment:
// A holds lockA, waits for lockB.
// B holds lockB, waits for lockA.
// DEADLOCK.
```

The fix: **consistent ordering**. Decide a rule (e.g., "always lock `lockA` before `lockB`") and follow it everywhere.

## How to defend

```
Rule 1: Keep lock count small — every extra lock multiplies deadlock risk.
Rule 2: Always acquire locks in the same order across all code paths.
Rule 3: If order CANNOT be fixed, use Monitor.TryEnter with a timeout:
         if (!Monitor.TryEnter(lockB, 100)) { Monitor.Exit(lockA); retry; }
Rule 4: Never call external/unknown code while holding a lock (it might lock back).
```

## See it move

Press **Run demo** — two threads lock in opposite orders. The timeline shows both entering their first lock, then both stuck forever waiting for the second. After 2 seconds the sandbox timeouts and reports the hang.

## Watch out

> **The Thread.Join deadlock.** If thread A calls threadB.Join() while threadB calls threadA.Join(), same deadlock — just using threads instead of locks. Always Join in a known, consistent order.

> **SemaphoreSlim can deadlock too.** If thread A holds a semaphore slot and then awaits another semaphore that thread B holds — while B is waiting on A's semaphore — same circular wait. Same prevention: consistent ordering.

## Key takeaways

- Deadlock = circular wait: A holds X wants Y; B holds Y wants X.
- Prevent: lock ordering convention, TryEnter timeouts, minimal lock count.
- Detect: thread stacks in a dump show waiting threads at Monitor.Enter.
