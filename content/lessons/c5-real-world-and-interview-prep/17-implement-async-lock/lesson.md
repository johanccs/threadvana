---
id: c5-l17-implement-async-lock
category: c5-real-world-and-interview-prep
order: 17
title: "Interview: Implement Your Own Async Lock"
difficulty: advanced
description: "Implement a fully async lock from scratch: understand the internals that make SemaphoreSlim work under the hood."
explainer: semaphore
interview:
  - q: "Implement an async lock using SemaphoreSlim."
    a: "class AsyncLock { private readonly SemaphoreSlim _sem = new(1,1); public async Task<IDisposable> LockAsync() { await _sem.WaitAsync(); return new Releaser(_sem); } private struct Releaser : IDisposable { public void Dispose() => _sem.Release(); } }. Usage: using (await myLock.LockAsync()) { ... }. The Releaser struct avoids heap allocations for the common case."
---

Write `Solution.AsyncLock` class with a `LockAsync()` method that returns something that can be disposed to release. Use `SemaphoreSlim(1,1)`. Return `"locked"` from `AcquireAndReleaseAsync()`.
