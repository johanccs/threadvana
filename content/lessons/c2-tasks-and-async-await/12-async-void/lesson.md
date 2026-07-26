---
id: c2-l12-async-void
category: c2-tasks-and-async-await
order: 12
title: "async void Ã¢â‚¬â€ the Fire-and-Forget Trap"
difficulty: intermediate
description: "Understand why async void is dangerous (fire-and-forget with no error handling) and the one place it is required: event handlers."
explainer: async-state-machine
interview:
  - q: "When is async void acceptable?"
    a: "Only in event handlers Ã¢â‚¬â€ a button click, a timer tick, an HTTP handler. The framework expects void return and does not await the handler, so async void is the only option. Everywhere else, use async Task. async void is poison: the caller cannot await it, exceptions break out to the SynchronizationContext (crashing the process in some cases), and there is no Task to track completion."
  - q: "What happens if an async void method throws?"
    a: "The exception is raised directly on the SynchronizationContext (or on the thread pool if there is no context). If unhandled, it triggers AppDomain.UnhandledException and crashes the process. You cannot wrap it in try/catch in the caller because the caller has already returned Ã¢â‚¬â€ there is no Task to catch into."
---

## What is it?

`async void` is the dangerous sibling of `async Task`. It exists for exactly one reason: event handlers must return `void`, not `Task`. But `async void` has two fatal flaws: (1) the caller cannot `await` it Ã¢â‚¬â€ it is true fire-and-forget, and (2) exceptions are not captured into a Task Ã¢â‚¬â€ they crash the process if unhandled.

The rule is iron: **never async void outside an event handler.**

## The real-world picture

A firework with no fuse. You light it and immediately turn your back Ã¢â‚¬â€ you cannot watch it, cannot stop it, and if it explodes in your face, you won't know until you feel the burn. `async Task` is a firework with a long fuse and a launcher you can watch. `async void` is a lit match thrown over your shoulder.

## How it works in C#

```csharp
// ONLY acceptable use Ã¢â‚¬â€ UI event handler:
private async void SaveButton_Click(object sender, EventArgs e)
{
    await SaveAsync(); // works, but if SaveAsync throws Ã¢â€ â€™ process crash
}

// WRONG Ã¢â‚¬â€ should be async Task:
async void FetchDataAsync() // caller cannot await, cannot catch
{
    await HttpClient.GetAsync("...");
}
```

## See it move

Press **Run demo** Ã¢â‚¬â€ six fire-and-forget workers start, with no coordination. Three are `async Task` (their completion is tracked), three are `async void` (the demo has no way to know when they finish). Watch the timeline: the void workers appear, run, and vanish Ã¢â‚¬â€ untrackable.

## Watch out

> **async void + await = untracked exception.** If the awaited task faults, the exception crashes the process. Use `try/catch` INSIDE every async void method and log + swallow (never re-throw).

> **Unit testing async void is impossible.** xUnit/NUnit cannot await void methods and cannot detect failures inside them. If your method is `async void` for testing reasons, refactor to `async Task` Ã¢â‚¬â€ test frameworks handle it.

> **async void in a fire-and-forget background job is a double trap.** The exception crash might not happen for minutes, making it a time-delayed heisenbug.

## Key takeaways

- `async void` Ã¢â€ â€™ only for event handlers. Everywhere else: `async Task`.
- Exceptions crash the process Ã¢â‚¬â€ always try/catch inside them.
- Cannot be awaited, composed, or tested. `async Task` for everything else.
