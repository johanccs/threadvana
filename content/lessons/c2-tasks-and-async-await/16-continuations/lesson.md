---
id: c2-l16-continuations
category: c2-tasks-and-async-await
order: 16
title: "Continuations Ã¢â‚¬â€ ContinueWith and Why await Won"
difficulty: advanced
description: "Chain work with ContinueWith: attach follow-up actions that run automatically when a task completes."
explainer: async-state-machine
interview:
  - q: "What is ContinueWith and is it recommended?"
    a: "ContinueWith attaches a callback that runs when the task finishes, on whatever thread the task completed on (or a specified scheduler). Before async/await (C# 5+), this was the standard way to chain work. Today await is almost always better: it preserves SynchronizationContext automatically, has cleaner error handling, and avoids the subtle pitfall of ContinueWith running the continuation inline on the same thread that sets the result (when you use the default TaskContinuationOptions)."
  - q: "When is ContinueWith still useful?"
    a: "Fire-and-forget after completion (with TaskContinuationOptions.OnlyOnRanToCompletion), or when you need to schedule work on a specific TaskScheduler (e.g., a single-threaded custom scheduler). For everyday async code, await handles all of this with cleaner syntax."
---

## What is it?

A **continuation** is "the code that runs after this task finishes." In ancient C# (pre-2012), you wrote continuations by hand with `ContinueWith`. Today the compiler does it for you with `await`.

But `ContinueWith` isn't dead Ã¢â‚¬â€ it reveals exactly what `await` is doing under the hood: the rest of your method is wrapped in a lambda and passed to `.ContinueWith(action, SynchronizationContext.Current)`.

## The real-world picture

You hand a letter to a receptionist and say "shout my name when the reply arrives" (`ContinueWith`). With `await`, you just sit in the waiting area and let the receptionist tap you on the shoulder Ã¢â‚¬â€ no shouting, no missed cues.

## How it works in C#

```csharp
// Old style Ã¢â‚¬â€ ContinueWith
var task = Task.Run(() => 42);
task.ContinueWith(t => Console.WriteLine(t.Result), TaskScheduler.Default);

// Modern style Ã¢â‚¬â€ await (compiler generates ContinueWith for you)
int result = await Task.Run(() => 42);
Console.WriteLine(result);
```

Under the hood, the compiler transforms the second block into something very close to the first Ã¢â‚¬â€ but with safer defaults and context preservation.

## Watch out

> **ContinueWith without a TaskScheduler runs on whatever thread the task completed on Ã¢â‚¬â€ often the pool.** If you need the UI thread, use `TaskScheduler.FromCurrentSynchronizationContext()` as the scheduler argument Ã¢â‚¬â€ `await` does this automatically.

> **Always check t.Exception in ContinueWith callbacks Ã¢â‚¬â€ `t.Result` will throw if the task faulted, and an unhandled exception in a ContinueWith callback is swallowed silently (it becomes a faulted task you never observe).**

## Key takeaways

- `ContinueWith` Ã¢â€ â€™ manual chaining; `await` Ã¢â€ â€™ automatic chaining.
- `await` preserves context, unwraps exceptions, and reads naturally.
- `ContinueWith` still useful for fire-and-forget after completion or custom schedulers.
