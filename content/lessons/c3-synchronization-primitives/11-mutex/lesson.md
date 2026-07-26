---
id: c3-l11-mutex
category: c3-synchronization-primitives
order: 11
title: "Mutex Ã¢â‚¬â€ the Cross-Process Key"
difficulty: intermediate
description: "Use Mutex for system-wide locking: ensure only one instance of your application runs at a time, even across processes."
explainer: semaphore
interview:
  - q: "What is a Mutex and how is it different from a lock?"
    a: "A Mutex is a kernel-level mutual-exclusion primitive that works ACROSS PROCESSES (like a named lock). lock is thread-only, in-process, and fast. Mutex can ensure only one instance of an application runs at a time (the classic `bool createdNew; new Mutex(true, \"MyApp\", out createdNew)` pattern). Mutex is slower (kernel transitions) and must be explicitly Released Ã¢â‚¬â€ missing a Release leaves the mutex permanently abandoned (the next waiter gets an AbandonedMutexException)."
  - q: "What is AbandonedMutexException?"
    a: "When a thread that owns a Mutex exits without calling ReleaseMutex, the OS marks the mutex as abandoned. The next thread that WaitOne succeeds will receive an AbandonedMutexException Ã¢â‚¬â€ the OS is warning you that the previous owner's work may have been interrupted mid-operation (e.g., a crash), and the guarded resource may be in a corrupted state."
---

## What is it?

A `Mutex` is the cross-process version of `lock`. It ensures only ONE thread across ALL processes can enter a critical section. The OS owns it, so if your process dies, the mutex is released (with an "abandoned" marker to warn the next owner).

## The classic pattern

```csharp
// Single-instance app guard.
using var mutex = new Mutex(true, @"Global\MyAppSingleInstance", out var createdNew);
if (!createdNew)
{
    Console.WriteLine("Another instance is already running.");
    return;
}
// ... run the app ...
```

## Watch out

> **Always Release the mutex.** Unlike lock (Monitor), the CLR does not auto-release on thread exit Ã¢â‚¬â€ you must call ReleaseMutex explicitly, ideally in a try/finally block.

> **Named mutex naming rules.** On Windows, prefix with "Global\" to make it accessible across sessions (e.g., services), or "Local\" for the current session only. Names are case-sensitive.

## Key takeaways

- `Mutex` Ã¢â€ â€™ kernel-level, cross-process mutual exclusion.
- Single-instance guard: `new Mutex(true, "name", out createdNew)`.
- Must always Release; abandoned mutexes throw `AbandonedMutexException`.
