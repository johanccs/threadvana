---
id: c2-l12-async-void
category: c2-tasks-and-async-await
order: 12
title: "async void  -  the Fire-and-Forget Trap"
difficulty: intermediate
description: "Understand why async void is dangerous (fire-and-forget with no error handling) and the one place it is required: event handlers."
explainer: async-state-machine
interview:
  - q: "When is async void acceptable?"
    a: "Only in event handlers  -  a button click, a timer tick, an HTTP handler. The framework expects void return and does not await the handler, so async void is the only option. Everywhere else, use async Task. async void is poison: the caller cannot await it, exceptions break out to the SynchronizationContext (crashing the process in some cases), and there is no Task to track completion."
  - q: "What happens if an async void method throws?"
    a: "The exception is raised directly on the SynchronizationContext (or on the thread pool if there is no context). If unhandled, it triggers AppDomain.UnhandledException and crashes the process. You cannot wrap it in try/catch in the caller because the caller has already returned  -  there is no Task to catch into."
---

## What is it?

`async void` is the dangerous sibling of `async Task`. It exists for exactly one reason: event handlers must return `void`, not `Task`. But `async void` has two fatal flaws: (1) the caller cannot `await` it — it is true fire-and-forget, and (2) exceptions are not captured into a Task — they crash the process if unhandled.

The rule is iron: **never async void outside an event handler.**

## The real-world picture

A firework with no fuse. You light it and immediately turn your back — you cannot watch it, cannot stop it, and if it explodes in your face, you won't know until you feel the burn. `async Task` is a firework with a long fuse and a launcher you can watch. `async void` is a lit match thrown over your shoulder.

## How it works in C#

```csharp
// ONLY acceptable use — UI event handler:
private async void SaveButton_Click(object sender, EventArgs e)
{
    await SaveAsync(); // works, but if SaveAsync throws → process crash
}

// WRONG — should be async Task:
async void FetchDataAsync() // caller cannot await, cannot catch
{
    await HttpClient.GetAsync("...");
}
```

## See it move

Press **Run demo** — six fire-and-forget workers start, with no coordination. Three are `async Task` (their completion is tracked), three are `async void` (the demo has no way to know when they finish). Watch the timeline: the void workers appear, run, and vanish — untrackable.

## Watch out

> **async void + await = untracked exception.** If the awaited task faults, the exception crashes the process. Use `try/catch` INSIDE every async void method and log + swallow (never re-throw).

> **Unit testing async void is impossible.** xUnit/NUnit cannot await void methods and cannot detect failures inside them. If your method is `async void` for testing reasons, refactor to `async Task` — test frameworks handle it.

> **async void in a fire-and-forget background job is a double trap.** The exception crash might not happen for minutes, making it a time-delayed heisenbug.

## Key takeaways

- `async void` → only for event handlers. Everywhere else: `async Task`.
- Exceptions crash the process — always try/catch inside them.
- Cannot be awaited, composed, or tested. `async Task` for everything else.
