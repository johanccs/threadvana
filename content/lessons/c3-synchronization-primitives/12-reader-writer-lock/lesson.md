---
id: c3-l12-reader-writer-lock
category: c3-synchronization-primitives
order: 12
title: "ReaderWriterLockSlim  -  Many Readers, One Writer"
difficulty: advanced
description: "Optimize read-heavy workloads with ReaderWriterLockSlim: allow many readers at once but only one writer at a time."
explainer: lock-key
interview:
  - q: "When should you use ReaderWriterLockSlim instead of lock?"
    a: "When reads vastly outnumber writes. lock serialises everything  -  even pure reads that don't mutate state. ReaderWriterLockSlim lets any number of concurrent readers in, but only one writer (with no readers active). This dramatically increases throughput for read-heavy caches and configuration stores. The Slim suffix means it's user-mode, not kernel-backed."
  - q: "What is the danger of an upgradeable read lock?"
    a: "ReaderWriterLockSlim supports an upgradeable read lock that can later be promoted to a write lock. But only ONE thread can hold the upgradeable read at a time  -  if many threads request it, they serialise. Plus, upgrading is not atomic  -  between checking a read and upgrading to write, the data may have changed. Always re-check after upgrading."
---

## What is it?

`lock` is a single bathroom Ã¢â‚¬â€ one person at a time, period. `ReaderWriterLockSlim` is a library reading room: as many people as you want can read quietly, but only one person can write (and they kick everyone else out first).

The three lock modes: `EnterReadLock` (any number, concurrent), `EnterWriteLock` (exclusive), `EnterUpgradeableReadLock` (read now, may upgrade to write later Ã¢â‚¬â€ only one holder at a time).

## Watch out

> **ReaderWriterLockSlim is NOT async-friendly.** All Enter methods block Ã¢â‚¬â€ there is no WaitAsync. For async scenarios, use SemaphoreSlim or standard lock (brief, then offload to async).

> **Writer starvation.** If readers keep arriving, the writer may wait forever. ReaderWriterLockSlim prefers fairness on .NET Core, but on .NET Framework it prefers readers.

## Key takeaways

- Many concurrent readers, one writer at a time.
- Three modes: Read, Write, UpgradeableRead.
- Always `try/finally { ExitReadLock() }` Ã¢â‚¬â€ these locks are not auto-released.
