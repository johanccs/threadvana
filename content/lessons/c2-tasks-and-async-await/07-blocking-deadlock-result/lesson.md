---
id: c2-l07-blocking-deadlock-result
category: c2-tasks-and-async-await
order: 7
title: "Blocking on Async Ã Â¢Ã¢â  Â¬ Why .Result and .Wait() Deadlock"
difficulty: intermediate
description: "Understand why calling .Result or .Wait() on a Task can deadlock your application, and how to avoid it."
visualization: async-activity
explainer: deadlock
interview:
  - q: Why does calling .Result on a task deadlock in some contexts?
    a: "The async method, when awaited, tries to marshal its continuation back to the captured SynchronizationContext (e.g., the UI thread in WPF, or the request context in ASP.NET Framework). Calling .Result blocks the current thread Ã Â¢Ã¢â  Â¬ if that thread is the same one the continuation needs, the two wait on each other forever. .NET Core / ASP.NET Core (8+) has no SynchronizationContext, so the classic deadlock does NOT happen there, but .Result STILL wastes a thread Ã Â¢Ã¢â  Â¬ a silent pool leak."
  - q: In .NET 8, is .Result still dangerous?
    a: "It won't deadlock at the synchronization-context level (there is none), but it burns a thread doing nothing. On a loaded server with limited pool threads, a thousand .Result calls can starve the pool, and new work queues up while threads are blocked. The rule is the same: never block on async."
---

## What is it?

There are exactly two ways to wait for a Task to finish: **cooperatively** (`await`) or **forcefully** (`.Result` / `.Wait()`). Only one is correct in async code.

When you call `.Result` or `.Wait()`, you are saying: "I refuse to give up this thread. Stop everything until that task finishes." If the task internally is trying to get BACK to this same thread (via a `SynchronizationContext`), you have a **deadlock** Ã Â¢Ã¢â  Â¬ two things each waiting on the other, forever.

.NET 8 removed the default SynchronizationContext from ASP.NET Core, so the *classic* deadlock is gone Ã Â¢Ã¢â  Â¬ but `.Result` is still a pool-killer. A blocked thread cannot do other work while it waits, so at scale every `.Result` is one fewer worker for real requests.

## The real-world picture

Two people approach the same door from opposite sides. Alice pushes, Bob pulls. Neither can move while the other is holding their side. A deadlock is exactly that Ã Â¢Ã¢â  Â¬ but instead of people, it is a thread holding a lock that the other thread needs.

The pool version: a manager assigns tasks to 5 workers. One worker sits still for 3 minutes "waiting for a report." The other 4 handle all incoming calls. At 100 calls/minute, the queue fills up because worker #5 isn't doing anything. `.Result` is worker #5.

## How it works in C#

```csharp
// BAD Ã Â¢Ã¢â  Â¬ blocks the thread (pool starvation at scale)
int result = SomeAsyncMethod().Result;

// GOOD Ã Â¢Ã¢â  Â¬ yields the thread until the result is ready
int result = await SomeAsyncMethod();
```

The key insight: `await` does NOT wait Ã Â¢Ã¢â  Â¬ it **returns** to the caller, tells the runtime "call me back here when this is done," and frees the thread for other work. `.Result` physically holds the thread, doing nothing, until the task finishes.

## See it move

Press **Run demo** Ã Â¢Ã¢â  Â¬ we simulate 50 concurrent "requests," half using `await` and half using `.Result`. Watch the pool-swimlane chart: the awaiters finish fast because they share pool threads, while the blockers pile up behind the few threads stuck in `.Result`.

## Watch out

> **aspnetcore SynchronizationContext is gone, but ASP.NET Framework (classic) has one.** If you ever work in legacy ASP.NET, `.Result` inside a controller action will deadlock the request thread.

> **Don't wrap async with sync to "make it simpler."** The rule "async all the way down" exists because one blocking call at the bottom can cascade into pool starvation at the top.

> **Task.GetAwaiter().GetResult() has the same problem** Ã Â¢Ã¢â  Â¬ it is just `.Result` without wrapping the exception in AggregateException. Still blocks.

## Key takeaways

- `await` Ã Â¢Ã¢â  Â¬ frees the thread. `.Result` Ã Â¢Ã¢â  Â¬ holds it.
- Classic deadlock = blocked thread waiting on a continuation that needs the same thread. Fixed in .NET 8+ but the pool cost remains.
- Never block on async. If a synchronous method must call async code, rethink the design Ã Â¢Ã¢â  Â¬ or accept the pool cost with full awareness.
