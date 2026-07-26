---
id: c2-l11-configureawait
category: c2-tasks-and-async-await
order: 11
title: "ConfigureAwait Ã Â¢Ã¢â  Â¬ Who Needs the Context?"
difficulty: intermediate
description: "Master ConfigureAwait(false): avoid deadlocks in library code by not capturing the synchronization context."
visualization: async-activity
explainer: async-state-machine
interview:
  - q: "What does ConfigureAwait(false) do and when must you use it?"
    a: "It tells the awaiter: do NOT capture the SynchronizationContext. The continuation that runs after the await will be scheduled on any available thread-pool thread, not the original context. Library code should almost always use ConfigureAwait(false) to avoid deadlocking the caller's context. Application code (controllers, event handlers) should keep the default (true) so the continuation returns to the UI/main thread."
  - q: "In ASP.NET Core, do you still need ConfigureAwait(false)?"
    a: "No Ã Â¢Ã¢â  Â¬ ASP.NET Core removed the SynchronizationContext, so continuations already run on any thread by default. ConfigureAwait(false) does nothing for correctness, but it is harmless. In legacy ASP.NET Framework, omitting it inside a library can deadlock the request thread if the caller uses .Result."
---

## What is it?

Every `await` does two things: (1) pauses execution and releases the thread, and (2) captures the current `SynchronizationContext` so the continuation can resume on the "right" thread Ã Â¢Ã¢â  Â¬ the UI thread in a desktop app, or the request context in legacy ASP.NET.

`ConfigureAwait(false)` suppresses step 2: the continuation can run on ANY pool thread. This is faster (no context-switching overhead) and crucial for library code that must not depend on the caller's context.

## The real-world picture

You check into a hotel. The clerk gives you a buzzer and says "come back to THIS desk when it goes off" (ConfigureAwait(true)). Or the clerk says "any desk can help you" (ConfigureAwait(false)). If you insist on THIS desk and it is blocked, you wait forever (deadlock). If any desk works, you get served immediately.

## How it works in C#

```csharp
// LIBRARY code Ã Â¢Ã¢â  Â¬ always use false.
public async Task<int> FetchAsync()
{
    await Task.Delay(100).ConfigureAwait(false);
    return 42;
}

// APPLICATION code (WPF click handler, ASP.NET controller) Ã Â¢Ã¢â  Â¬ default (true) is correct.
private async void Button_Click(object sender, EventArgs e)
{
    await SomeAsync(); // continuation returns to the UI thread Ã Â¢Ã¢â  Â¬ good.
    label.Text = "Done"; // safe Ã Â¢Ã¢â  Â¬ we are back on the UI thread.
}
```

## Watch out

> **ConfigureAwait(false) in a non-library async method is usually wrong.** If the method is called from a UI framework and needs to touch UI controls after the await, omitting the context means those updates will throw (wrong thread).

> **One ConfigureAwait per await Ã Â¢Ã¢â  Â¬ it doesn't cascade.** If method A calls method B with ConfigureAwait(false), method A's await still captures the context unless method A also uses ConfigureAwait(false). Every `await` is its own decision point.

> **In .NET 8+, ASP.NET Core has no context Ã Â¢Ã¢â  Â¬ ConfigureAwait(false) is a no-op there.** But library authors should still use it for compatibility with legacy callers.

## Key takeaways

- `ConfigureAwait(false)` Ã Â¢Ã¢â ¬Â  do not capture the SynchronizationContext; continuation on any thread.
- Library code: always false. Application code: default (true).
- Does not cascade Ã Â¢Ã¢â  Â¬ each `await` decides independently.
