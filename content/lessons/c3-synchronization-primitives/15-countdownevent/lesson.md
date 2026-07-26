---
id: c3-l15-countdownevent
category: c3-synchronization-primitives
order: 15
title: "CountdownEvent Ã¢â‚¬â€ Waiting for N Signals"
difficulty: intermediate
description: "Count down to zero with CountdownEvent: wait until N threads have all signaled they are done with their work."
explainer: event-gate
interview:
  - q: "What is CountdownEvent and when would you use it?"
    a: "CountdownEvent is a countdown latch: create it with a count, and call Signal() to count down. Wait() blocks until the count hits zero. It is perfect for 'wait until all N workers have finished their piece' without manually managing a counter and a lock. Once zero, it cannot be reset Ã¢â‚¬â€ unlike ManualResetEventSlim, it is single-use."
  - q: "How is CountdownEvent different from Task.WhenAll?"
    a: "CountdownEvent works with raw threads (Thread class), while WhenAll works with Tasks. Use CountdownEvent when you must coordinate Thread instances without Tasks; use WhenAll with Task-based code. In modern code, you almost never need CountdownEvent Ã¢â‚¬â€ Tasks and WhenAll replaced it."
---

## What is it?

`CountdownEvent` counts DOWN from N to 0. Workers call `Signal()` as they finish. The coordinator calls `Wait()` Ã¢â‚¬â€ which blocks until the count reaches zero. It's the barrier you lower behind the last runner.

## See it move

Press **Run demo** Ã¢â‚¬â€ 5 workers each do some work, then Signal. The coordinator Waits and prints "All done!" after the fifth Signal.

## Key takeaways

- `new CountdownEvent(N)` Ã¢â€ â€™ Signal N times to release Wait().
- Wait() blocks the calling thread; no async version.
- Single-use Ã¢â‚¬â€ once zero, it stays zero.
