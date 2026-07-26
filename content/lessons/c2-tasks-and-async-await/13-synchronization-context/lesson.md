---
id: c2-l13-synchronization-context
category: c2-tasks-and-async-await
order: 13
title: "The SynchronizationContext Ã¢â‚¬â€ Where Continuations Go"
difficulty: advanced
description: "Understand SynchronizationContext: the invisible dispatcher that routes async continuations back to the right thread."
explainer: async-state-machine
interview:
  - q: "What is a SynchronizationContext?"
    a: "It is a scheduler that decides WHERE the code after an await runs Ã¢â‚¬â€ back on the original thread (UI, request context) or on a thread-pool thread. WPF and WinForms install one that posts back to the main UI thread. ASP.NET Framework installs one bound to the HTTP request. ASP.NET Core (8+) does NOT install one Ã¢â‚¬â€ continuations always run on the pool. Library code uses ConfigureAwait(false) to tell the awaiter: don't capture or post back, I am fine on any thread."
  - q: "How does ConfigureAwait(false) interact with SynchronizationContext?"
    a: "ConfigureAwait(false) tells the awaiter to NOT call SynchronizationContext.Post Ã¢â‚¬â€ the continuation is scheduled directly on the thread pool. As far as the continuation is concerned, there is no context. This avoids deadlocks in UI threads and request threads when library code is called from blocking callers."
---

## What is it?

Every .NET thread lives in an apartment ruled by a `SynchronizationContext`. In a WPF app, the UI thread's context makes sure that `label.Text = "done"` runs on the correct thread. In classic ASP.NET, the context ties work to a single HTTP request.

When you `await`, the compiler captures the current `SynchronizationContext` and, after the awaited task finishes, **posts** the rest of your method back to that context Ã¢â‚¬â€ so you wake up on the same "kind" of thread you left.

## The real-world picture

A worker in a hospital. When a nurse finishes one patient (the awaited task finishes), the nurse goes to the dispatcher (the synchronization context) who says "go back to room 302" (the original UI thread). If there is no dispatcher (ASP.NET Core), the nurse picks up the next patient from the waiting room (the thread pool).

## How it works in C#

```csharp
// Console app Ã¢â‚¬â€ no SynchronizationContext.
Console.WriteLine(SynchronizationContext.Current); // null

// WPF app Ã¢â‚¬â€ DispatcherSynchronizationContext installed automatically.
// After await, the continuation magically re-enters the UI thread.
private async void Button_Click()
{
    await File.ReadAllTextAsync("data.txt");
    label.Text = "Loaded"; // safe Ã¢â‚¬â€ we are back on the UI thread
}
```

In your Sandbox (console), `SynchronizationContext.Current` is `null`. That means every continuation runs on the thread pool Ã¢â‚¬â€ there is no "home" to return to. This is why `.Result` doesn't deadlock in console apps or ASP.NET Core.

## Watch out

> **SynchronizationContext.Current can change mid-method.** Each `await` recaptures whatever context is current at that point Ã¢â‚¬â€ it is not cached once and reused.

> **Don't install a context unless you know exactly why.** ASP.NET Core deliberately removed it because thread-pool continuations scale better.

## Key takeaways

- `SynchronizationContext` decides where code after `await` runs Ã¢â‚¬â€ UI thread, request thread, or pool.
- Console and ASP.NET Core Ã¢â€ â€™ null Ã¢â€ â€™ continuations on the pool.
- WPF and WinForms install a UI context; ASP.NET Framework installs a request context.
- `ConfigureAwait(false)` Ã¢â€ â€™ skip the context, always run on the pool.
