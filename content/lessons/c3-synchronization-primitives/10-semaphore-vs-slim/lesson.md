---
id: c3-l10-semaphore-vs-slim
category: c3-synchronization-primitives
order: 10
title: "Semaphore vs SemaphoreSlim  -  the OS Heavyweight and the Async-Friendly Twin"
difficulty: intermediate
description: "Compare Semaphore (OS-level, cross-process) with SemaphoreSlim (in-process, async-friendly): when to use each."
visualization: semaphore
interview:
  - q: "What is the difference between Semaphore and SemaphoreSlim?"
    a: "Semaphore is an OS-level kernel object  -  it works across processes, supports named instances, and is heavyweight (every WaitOne enters the kernel). SemaphoreSlim is a lightweight, user-mode-only implementation inside the CLR  -  it cannot cross process boundaries but is much faster and supports WaitAsync for truly async waiting. 99% of in-process throttling should use SemaphoreSlim."
  - q: "When should you still use the heavyweight Semaphore?"
    a: "When you need cross-process coordination  -  e.g., limiting the total number of instances of an application across multiple processes, or gating access to a system-wide resource like a hardware device. Named semaphores are the only way to do this without a separate IPC mechanism."
---

## What is it?

You already used `SemaphoreSlim` in c3-l03. But .NET has TWO semaphores — `Semaphore` (the kernel-backed original) and `SemaphoreSlim` (the lightweight, modern one). The `Slim` suffix tells you everything: same API surface, zero kernel trips.

`Semaphore` wraps a Windows kernel semaphore object (or a POSIX semaphore on Linux). Every call goes through the OS — fast enough for occasional use, but for a tight loop it burns a kernel transition. `SemaphoreSlim` lives entirely in the CLR, using atomic operations and managed wait queues.

## How it works

```csharp
// Heavyweight — OS kernel object, cross-process, named.
using var osSem = new Semaphore(3, 3, "Global\\MyAppLimit");
osSem.WaitOne(); // kernel transition
// ... work ...
osSem.Release();

// Lightweight — pure CLR, async, fast.
using var slim = new SemaphoreSlim(3, 3);
await slim.WaitAsync(); // no kernel transition
// ... work ...
slim.Release();
```

## Watch out

> **Named semaphores can leak.** The OS persists them until the last handle is closed. If your app crashes, the named semaphore may exist forever with a count of 0, preventing new instances from starting. Always pass a timeout to WaitOne for named semaphores.

> **SemaphoreSlim is disposable.** It allocates a small internal wait-handle on first contention — dispose it when you are done.

## Key takeaways

- `SemaphoreSlim` → fast, in-process, supports `WaitAsync`. Use this.
- `Semaphore` → kernel-backed, cross-process, supports named instances. Niche.
- Both limit concurrency to N simultaneous entries.
