---
id: c3-l13-manualresetevent-slim
category: c3-synchronization-primitives
order: 13
title: "ManualResetEventSlim  -  the Gate That Stays Open"
difficulty: intermediate
description: "Signal between threads with ManualResetEventSlim: one thread turns on a green light, others wait for it."
explainer: event-gate
interview:
  - q: "What is ManualResetEventSlim and how is it different from AutoResetEvent?"
    a: "ManualResetEventSlim is a gate: it starts closed (threads calling Wait() block), then Set() opens it  -  and it STAYS open until Reset() is called. All waiting threads are released and all new callers pass through immediately. AutoResetEvent releases exactly ONE waiting thread and immediately closes again  -  like a turnstile, not a gate."
  - q: "When would you use ManualResetEventSlim over a TaskCompletionSource?"
    a: "TaskCompletionSource is single-shot  -  you can't reset it. ManualResetEventSlim supports Set/Reset cycles and can be waited on by multiple threads (not just awaited once). In modern code, TCS is usually preferred for one-shot signals; ManualResetEventSlim is better for reusable gates or legacy thread-based signalling."
---

## What is it?

`ManualResetEventSlim` is a reusable signal: a gate. `Set()` opens it; `Reset()` closes it. Any number of threads calling `Wait()` will block when closed and pass through when open.

## See it move

Press **Run demo** Ã¢â‚¬â€ two threads Wait on a gate. After 500ms, Set() opens it Ã¢â‚¬â€ both pass. Then Reset() closes it again, and a third thread blocks.

## Key takeaways

- `Set()` opens the gate; `Reset()` closes it.
- `Manual` = stays open until explicitly Reset; `Auto` = releases one and closes.
- `Wait()` blocks the calling thread Ã¢â‚¬â€ no async version. For async, use `TaskCompletionSource`.
