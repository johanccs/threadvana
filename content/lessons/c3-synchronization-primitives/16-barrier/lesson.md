---
id: c3-l16-barrier
category: c3-synchronization-primitives
order: 16
title: "Barrier Ã Â¢Ã¢â  Â¬ Phased Parallel Work"
difficulty: advanced
description: "Synchronize multiple threads at a rendezvous point with Barrier: everyone waits until all participants have arrived."
explainer: event-gate
interview:
  - q: "What is the Barrier class?"
    a: "Barrier synchronises a fixed number of participants into phases. All participants call SignalAndWait() Ã Â¢Ã¢â  Â¬ when the last one calls it, the barrier fires an optional post-phase action, then releases everyone to start the next phase. It is like a meeting where everyone must arrive before the next agenda item starts."
  - q: "What happens if a participant never calls SignalAndWait?"
    a: "The barrier deadlocks Ã Â¢Ã¢â  Â¬ all other participants wait forever. The phase never completes because the expected participant count never arrived. Barrier supports timeouts (SignalAndWait(timeout)) to prevent infinite hangs."
---

## What is it?

A `Barrier` coordinates N threads into synchronised phases. Each thread calls `SignalAndWait()` Ã Â¢Ã¢â  Â¬ the barrier blocks until ALL N have done so. Then it optionally fires a post-phase callback (e.g., "phase 2 complete"), and releases all threads again.

## See it move

Press **Run demo** Ã Â¢Ã¢â  Â¬ 3 workers complete 3 phases each. Each phase: work Ã Â¢Ã¢â ¬Â  SignalAndWait Ã Â¢Ã¢â ¬Â  barrier fires when all 3 arrive Ã Â¢Ã¢â ¬Â  next phase.

## Key takeaways

- `new Barrier(N, action)` Ã Â¢Ã¢â ¬Â  N participants, optional per-phase action.
- `SignalAndWait()` Ã Â¢Ã¢â ¬Â  blocks until all participants arrive at the barrier.
- Use for phased parallel algorithms (image processing, parallel matrix operations).
