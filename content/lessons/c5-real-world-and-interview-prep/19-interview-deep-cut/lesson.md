---
id: c5-l19-interview-deep-cut
category: c5-real-world-and-interview-prep
order: 19
title: "Interview: SemaphoreSlim & Lock Deep-Cut Questions"
difficulty: advanced
description: "Tackle advanced interview questions: ExecutionContext flow, TaskScheduler internals, and async state machine details."
explainer: async-state-machine
interview:
  - q: "What happens when you call Release() on a SemaphoreSlim more times than the max count?"
    a: "SemaphoreReleaseException is thrown. The semaphore tracks current count  -  it cannot exceed the maxCount passed to the constructor. Unlike the OS Semaphore, which silently ignores extra releases, SemaphoreSlim enforces the upper bound."
  - q: "Can you await inside a lock? What are the alternatives?"
    a: "No  -  lock is syntactic sugar for Monitor.Enter/Exit, which are thread-affine. After an await, the continuation may run on a different thread, and Monitor.Exit would throw SynchronizationLockException. Use SemaphoreSlim(1,1).WaitAsync() + Release() instead. This is the standard async-compatible mutual exclusion pattern."
---

Review-only. Return `"ok"`.
