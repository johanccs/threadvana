---
id: c2-l19-progress-reporting
category: c2-tasks-and-async-await
order: 19
title: "Progress Reporting  -  IProgress<T>"
difficulty: intermediate
description: "Report progress from async operations with IProgress<T> and Progress<T>: keep the UI responsive during long-running work."
explainer: async-state-machine
interview:
  - q: "How do you report progress from an async method safely?"
    a: "Use IProgress<T>  -  its Report method automatically marshals the callback to the SynchronizationContext where the IProgress was created (e.g., the UI thread). The standard implementation, Progress<T>, captures the context at construction and invokes your callback there. Never update a progress bar or UI element directly from a background thread  -  use IProgress<T>."
  - q: "Why not just pass an Action<int> callback for progress?"
    a: "An Action runs on whatever thread calls it  -  if your async method is on a background thread, the callback runs there too, and any UI updates it makes will throw. IProgress<T>.Report posts to the captured SynchronizationContext, guaranteeing the callback runs on the right thread."
---

## What is it?

`IProgress<T>` is the contract for reporting a value (like percentage) from a long-running async operation back to the caller — **on the right thread**. It is one method: `Report(T value)`, which internally dispatches to the `SynchronizationContext` captured at creation time.

The standard implementation is `Progress<T>` — you give it a callback, and every `Report()` call runs that callback on the original context (the UI thread, or wherever you created the instance).

## The real-world picture

A pizza tracker app. The kitchen (background thread) shouts "30% done" into a megaphone repeatedly. `IProgress<T>` is the earpiece the UI thread wears — it hears every update and updates the progress bar, all without the kitchen ever touching the UI directly.

## How it works in C#

```csharp
// UI side: creates the progress handler ON THE UI THREAD.
var progress = new Progress<int>(percent => label.Text = $"{percent}% done");

// Background side: calls Report from any thread — the callback runs on UI.
await Task.Run(() => HeavyWork(progress));

async Task HeavyWork(IProgress<int> progress)
{
    for (var i = 0; i <= 100; i += 20)
    {
        await Task.Delay(300);
        progress.Report(i); // marshalled to the captured context
    }
}
```

## Watch out

> **Create Progress<T> on the thread you want callbacks to run on.** If you create it on a pool thread, the callbacks run on the pool — defeating the purpose.

> **In ASP.NET Core, there is no SynchronizationContext — IProgress degrades to a direct call on the reporting thread.** That is fine for console/server code but not for UI.

## Key takeaways

- `IProgress<T>.Report(...)` → marshalled to the captured SynchronizationContext.
- `new Progress<T>(Action<T>)` → standard implementation.
- Never touch UI from a background thread — progress-report through `IProgress<T>`.
