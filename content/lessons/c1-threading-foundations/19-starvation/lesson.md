---
id: c1-l19-starvation-and-fairness
category: c1-threading-foundations
order: 19
title: Starvation Ã Â¢Ã¢â  Â¬ When Greedy Threads Hog the Key
difficulty: advanced
description: "Understand thread pool starvation: when all pool threads are blocked and your application grinds to a halt."
visualization: thread-timeline
explainer: thread-pool
interview:
  - q: What is thread starvation?
    a: When one or more threads cannot get access to a shared resource (like a lock) because other threads keep taking it first. The starved threads make no progress, sometimes forever.
  - q: How do you prevent starvation?
    a: Keep locked sections SHORT. Avoid nesting locks. Use fair primitives when available (e.g. SemaphoreSlim with a queue in Category 3). And never hold a lock while doing slow work like I/O.
---

## What is it?

Three threads share one lock. Thread A grabs it, Thread B waits. Thread A finishes
and calls `Re-enterImmediately()`, grabbing the lock again before Thread B even has
a chance. Thread B starves Ã Â¢Ã¢â  Â¬ it makes no forward progress while A and its friends
keep jumping the queue.

This is thread starvation: a lockholder (or a set of them together) prevents others
from ever getting a turn.

## The real-world picture

A busy coffee shop has one espresso machine (the lock). One barista makes drinks
all morning, never stepping away. The second barista stands there with an empty
cup in her hand Ã Â¢Ã¢â  Â¬ permanently waiting. She is starved.

## How it works in C#

```csharp
lock (_gate)  // Thread A grabs the lock
{
    DoWork();
}
// lock released here Ã Â¢Ã¢â  Â¬ but Thread A immediately re-enters before Thread B can.
lock (_gate)  // Thread A again!
```

`lock` is NOT fair Ã Â¢Ã¢â  Â¬ it does not guarantee who gets in next. If one thread
re-acquires the lock rapidly, others may starve.

## See it move

Press **Run demo**. A greedy worker re-acquires a lock 8 times while two polite
workers wait. Watch one lane stacked with grey wait spans.

## Watch out

- The thread holding the lock should work fast. The golden rule: **never Sleep,
  never I/O, never a network call inside a lock.**
- `lock` favours speed over fairness. It uses an efficient spin-then-wait
  mechanism that can reward the last locker.
- A single slow lock holder can stall the entire application.

## Key takeaways

- Starvation = a thread waiting indefinitely for a resource others keep taking.
- Keep locked sections SHORT Ã Â¢Ã¢â  Â¬ milliseconds, not seconds.
- Never I/O inside a lock.
- In Category 3 you learn fairer primitives like SemaphoreSlim.
