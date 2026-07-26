---
id: c3-l14-autoresetevent
category: c3-synchronization-primitives
order: 14
title: "AutoResetEvent Ã¢â‚¬â€ One Thread at a Time Through the Gate"
difficulty: intermediate
description: "Use AutoResetEvent for one-shot thread signals: the gate automatically closes after letting exactly one thread through."
explainer: event-gate
interview:
  - q: "Explain AutoResetEvent with a real-world analogy."
    a: "It's a turnstile at a stadium. Set() loads one ticket Ã¢â‚¬â€ exactly one person (thread) can pass through, and then it immediately locks again. If 10 threads are waiting and you call Set() 3 times, exactly 3 pass (one per Set). WaitOne() consumes a ticket; if none available, blocks. This is the classic 'producer-consumer with exactly-one-shot-per-signal' primitive."
  - q: "What is the difference between AutoResetEvent(true) and AutoResetEvent(false)?"
    a: "The bool parameter is initial state: true means the event starts signalled (first WaitOne passes immediately), false starts non-signalled (first WaitOne blocks). After the initial Set/Wait, it behaves identically."
---

## What is it?

`AutoResetEvent` is a turnstile: each `Set()` loads ONE ticket; exactly ONE waiting thread passes, then the gate locks again. It's the simplest form of producer-consumer signalling Ã¢â‚¬â€ every signal produces exactly one consumer pass.

## See it move

Press **Run demo** Ã¢â‚¬â€ a producer calls Set() 4 times, spaced 200ms apart. Eight consumers WaitOne. Exactly 4 pass through, one per Set(). The remaining 4 are still waiting.

## Key takeaways

- Turnstile: Set() Ã¢â€ â€™ one thread passes, then auto-closes.
- `WaitOne()` blocks the thread Ã¢â‚¬â€ no async.
- For async signalling, use `SemaphoreSlim(0, int.MaxValue)` with Release/WaitAsync.
