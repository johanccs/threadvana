---
id: c3-l18-threadlocal
category: c3-synchronization-primitives
order: 18
title: "ThreadLocal<T> Ã Æ Ã â  Ã â  Ã Â¢Ã Æ Ã Â¢Ã Â¢Ã¢â  Â¬Ã Â¡Ã â  Ã Â¬Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã â  Ã Â  a Private Copy per Thread"
difficulty: intermediate
description: "Mark fields with [ThreadStatic] so each thread gets its own independent copy of the value with no locking needed."
explainer: thread-local
interview:
  - q: "What is ThreadLocal<T> and when would you use it?"
    a: "It gives each thread its own private copy of a value Ã Æ Ã â  Ã â  Ã Â¢Ã Æ Ã Â¢Ã Â¢Ã¢â  Â¬Ã Â¡Ã â  Ã Â¬Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã â  Ã Â  no locking needed because no thread touches another thread's copy. Classic use: a per-thread Random instance (Random is not thread-safe). Create it with a factory: new ThreadLocal<Random>(() => new Random()), then access .Value from any thread Ã Æ Ã â  Ã â  Ã Â¢Ã Æ Ã Â¢Ã Â¢Ã¢â  Â¬Ã Â¡Ã â  Ã Â¬Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã â  Ã Â  each gets its own instance. The per-thread data is isolated, so there are no race conditions."
  - q: "What's the difference between [ThreadStatic] and ThreadLocal<T>?"
    a: "[ThreadStatic] is an attribute on a static field Ã Æ Ã â  Ã â  Ã Â¢Ã Æ Ã Â¢Ã Â¢Ã¢â  Â¬Ã Â¡Ã â  Ã Â¬Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã â  Ã Â  each thread gets its own copy, but there is NO initialisation per thread (it is null/default each time). ThreadLocal<T> accepts a factory delegate that runs once per thread to seed the value. ThreadLocal<T> is the modern, safer choice."
---

## What is it?

`ThreadLocal<T>` gives each thread its own private slot Ã Æ Ã â  Ã â  Ã Â¢Ã Æ Ã Â¢Ã Â¢Ã¢â  Â¬Ã Â¡Ã â  Ã Â¬Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã â  Ã Â  like a post-office box. One thread writes a value; another thread reading the SAME property sees its own value. No locking needed because the data never crosses threads.

## See it move

Press **Run demo** Ã Æ Ã â  Ã â  Ã Â¢Ã Æ Ã Â¢Ã Â¢Ã¢â  Â¬Ã Â¡Ã â  Ã Â¬Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã â  Ã Â  4 threads, each incrementing their own ThreadLocal<int> 100 times. The main thread sums all thread-local values and reports 400 (100 Ã Æ Ã â  Ã â  'Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬" 4). No race because each thread has its own counter.

## Key takeaways

- `new ThreadLocal<T>(factory)` Ã Æ Ã â  Ã â  Ã Â¢Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã â  Ã Â Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â ¬Å¾Ã Â¢ factory runs once per thread.
- `.Value` accesses the calling thread's private copy.
- Thread-safe by isolation, not by locking.
