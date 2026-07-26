---
id: c2-l10-exceptions-in-async
category: c2-tasks-and-async-await
order: 10
title: "Exceptions in Async Code Ã¢â‚¬â€ Where Did My Stack Trace Go?"
difficulty: intermediate
description: "Handle exceptions in async code: they get wrapped in AggregateException and re-thrown in specific ways you must understand."
visualization: thread-timeline
explainer: async-state-machine
interview:
  - q: "What happens when an async method throws before the first await?"
    a: "The exception is thrown directly to the caller Ã¢â‚¬â€ because the method runs synchronously up to the first await. Once it hits await and returns a Task to the caller, the exception is captured inside the Task. The caller won't see it until they await the Task. If they never await, the exception is silently lost (and the TaskScheduler.UnobservedTaskException event fires later)."
  - q: "What is AggregateException and when do you see it?"
    a: "AggregateException wraps multiple exceptions Ã¢â‚¬â€ you see it when you use .Wait(), .Result, or Task.WhenAll on multiple tasks where several threw. await automatically unwraps the AggregateException and re-throws the first inner exception, which is almost always what you want. The rest are available via Task.Exception.InnerExceptions."
---

## What is it?

Exceptions thrown inside `async` methods behave differently from synchronous methods. The key rule: the exception is **captured** inside the `Task` the moment the method returns a Task to the caller. It does NOT propagate up the call stack immediately Ã¢â‚¬â€ because the caller might be long gone when the task faults.

When you `await` a faulted task, the exception is **re-thrown** at the `await` point Ã¢â‚¬â€ but not the raw exception: `await` unwraps any `AggregateException` and throws the first inner exception, preserving the original stack trace as best it can.

## The real-world picture

You order a pizza for delivery. If the pizza kitchen catches fire before they accept your order, the cashier tells you immediately (synchronous throw). If they accept the order and THEN the fire starts, you get a call 30 minutes later saying "your order is cancelled" (faulted Task). You only find out when you check (await or .Result).

## How it works in C#

```csharp
// Before first await Ã¢â‚¬â€ synchronous throw, immediate.
async Task<string> BuggyAsync()
{
    throw new InvalidOperationException("Boom!"); // thrown NOW, not captured.
}

// After first await Ã¢â‚¬â€ captured in the Task.
async Task<string> BuggyAsync()
{
    await Task.Delay(1);
    throw new InvalidOperationException("Boom!"); // caught inside the Task, re-thrown when awaited.
}
```

When multiple tasks fault in `WhenAll`:

```csharp
var t1 = Task.Run(() => throw new Exception("E1"));
var t2 = Task.Run(() => throw new Exception("E2"));

try { await Task.WhenAll(t1, t2); } // await throws E1 (first)
catch (Exception ex) { Console.WriteLine(ex.Message); } // E1
// t2.Exception.InnerExceptions contains E2.
```

## Watch out

> **async void methods cannot be catch-ed.** The exception is thrown directly on the SynchronizationContext (or the thread pool) Ã¢â‚¬â€ if unhandled, it crashes the process. Never use async void outside event handlers.

> **Don't assume WhenAll reports ALL exceptions.** `await WhenAll` throws only the first. Loop the tasks' `.Exception` field to collect all.

## Key takeaways

- Exceptions after `await` Ã¢â€ â€™ captured in the Task Ã¢â€ â€™ re-thrown by `await`.
- `await` unwraps AggregateException Ã¢â€ â€™ the first inner exception.
- `async void` exceptions bypass Task capture Ã¢â€ â€™ immediate crash if unhandled.
