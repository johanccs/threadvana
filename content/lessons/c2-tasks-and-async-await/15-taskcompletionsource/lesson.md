---
id: c2-l15-taskcompletionsource
category: c2-tasks-and-async-await
order: 15
title: "TaskCompletionSource Ã¢â‚¬â€ Wrapping Old Callbacks as Tasks"
difficulty: advanced
description: "Create your own tasks with TaskCompletionSource: bridge callback-based APIs into the Task world."
explainer: async-state-machine
interview:
  - q: "What is TaskCompletionSource and when would you use it?"
    a: "It lets you create a Task you control by hand Ã¢â‚¬â€ you decide when it completes, what result it gets, or what exception it throws. Every async API that wraps old callback-based or event-based code (e.g., wrapping a Socket.BeginReceive/EndReceive pair) uses a TaskCompletionSource under the hood. You call SetResult, SetException, or SetCanceled to finish it."
  - q: "What happens if you call SetResult twice?"
    a: "The second call throws InvalidOperationException. A TaskCompletionSource can only be completed once Ã¢â‚¬â€ after that, it is immutable. Use TrySetResult (which returns false on failure) if there is a chance the source has already been completed by another thread."
---

## What is it?

Most of the time you get a `Task` from `Task.Run`, an `async` method, or a BCL API. But what if you have a legacy callback Ã¢â‚¬â€ an event, a timer, a `Socket.Begin/End` Ã¢â‚¬â€ and you want to expose it as a modern `Task`?

`TaskCompletionSource<T>` is the bridge. You create one, start the legacy operation, and call `SetResult(value)` from the legacy callback. The `Task` it exposes can then be `await`ed by the rest of your async code.

## The real-world picture

You have a pager (legacy API) that beeps when your laundry is done. You wrap that pager in a `TaskCompletionSource` Ã¢â‚¬â€ the whole app can now `await LaundryTask` instead of listening for a beep. You transformed a callback into a promise.

## How it works in C#

```csharp
public Task<string> RunCommandAsync(string command)
{
    var tcs = new TaskCompletionSource<string>();

    var process = new Process { StartInfo = ... };
    process.Exited += (_, _) => tcs.SetResult(process.StandardOutput.ReadToEnd());
    process.Start();

    return tcs.Task; // caller can await this
}
```

## Watch out

> **SetResult, SetException, SetCanceled each complete the source ONCE.** Calling any twice throws. Use the `Try` variants (e.g., `TrySetResult`) if multiple threads may race to complete it.

> **Always complete the source Ã¢â‚¬â€ even on failure.** If your legacy callback never fires, the Task will hang forever. Put a timeout or cancellation guard around it.

> **Don't use TaskCompletionSource for simple fire-and-forget.** If you don't need to bridge legacy code, there is probably a simpler pattern.

## Key takeaways

- `TaskCompletionSource<T>` Ã¢â€ â€™ hand-crafted Task you complete manually.
- Perfect for bridging legacy callback/event APIs into the async world.
- Call `SetResult`, `SetException`, or `SetCanceled` exactly once.
