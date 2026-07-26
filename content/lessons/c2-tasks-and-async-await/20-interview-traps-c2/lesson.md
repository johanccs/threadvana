---
id: c2-l20-interview-traps-c2
category: c2-tasks-and-async-await
order: 20
title: "Async Interview Traps Ã Â¢Ã¢â  Â¬ the c2 Gotchas Reviewed"
difficulty: intermediate
description: "Avoid the most common async/await interview traps: deadlocks, fire-and-forget, and thread-pool starvation pitfalls."
explainer: async-state-machine
interview:
  - q: "Name three common async/await interview mistakes."
    a: "1. async void outside an event handler Ã Â¢Ã¢â  Â¬ untracked, un-catchable, crashes the process. 2. .Result or .Wait() on a Task Ã Â¢Ã¢â  Â¬ blocks the thread, deadlocks in classic ASP.NET/WPF. 3. Forgetting CancellationToken Ã Â¢Ã¢â  Â¬ operations run forever with no way to stop them. Interviewers want to hear that you know the WHY, not just the rule."
  - q: "What is the difference between Task.Run, Task.Factory.StartNew, and new Thread?"
    a: "Task.Run Ã Â¢Ã¢â ¬Â  default options on the thread pool, the go-to for modern code. Task.Factory.StartNew Ã Â¢Ã¢â ¬Â  more options (LongRunning, custom scheduler), but risky because default parameters differ from Task.Run (e.g., TaskCreationOptions.DenyChildAttach vs None). new Thread Ã Â¢Ã¢â ¬Â  dedicated OS thread, expensive, rarely needed since .NET 4.0."
---

## What is it?

This lesson collects the sharp edges from all of Category 2 into one review Ã Â¢Ã¢â  Â¬ the gotchas interviewers love because they reveal whether you truly understand async, or just memorised the keywords.

None of the individual points are new; the value here is seeing them SIDE BY SIDE so you can pattern-match quickly under interview pressure.

## The c2 trap sheet

| Trap | Why it bites | The fix |
|------|-------------|---------|
| `async void` | Exceptions crash the process; cannot be awaited | `async Task` everywhere except event handlers |
| `.Result` / `.Wait()` | Blocks thread; deadlocks with SynchronizationContext | `await` Ã Â¢Ã¢â  Â¬ async all the way down |
| Missing `CancellationToken` | Operation runs forever, no escape hatch | Pass a token; honour `ThrowIfCancellationRequested` |
| `GetOrAdd` with side effects | Factory runs more than once | Wrap in `Lazy<T>` |
| `Task.Run` for I/O | Burns a pool thread doing nothing while I/O completes | Use truly async I/O methods |
| `ContinueWith` without scheduler | Continuation may run on wrong thread | Prefer `await`; if you must, pass a scheduler |
| `await` inside `lock` | The thread may change after `await` Ã Â¢Ã¢â  Â¬ lock violations | `SemaphoreSlim(1,1).WaitAsync()` |
| `ConfigureAwait(false)` in app code | UI updates run on pool thread Ã Â¢Ã¢â ¬Â  crash | Keep the default in UI/controller code |

## See it move

Press **Run demo** Ã Â¢Ã¢â  Â¬ each row of the trap sheet plays out in a timeline. Watch `.Result` hog a pool thread while the awaiters breeze past. Then watch an `async void` worker throw silently into the void.

## Key takeaways

- Know WHY, not just the rule Ã Â¢Ã¢â  Â¬ interviewers probe for depth.
- `async Task` all the way up; `async void` only at the very top for events.
- If you block on async, you undo all the benefits. `await` or re-architect.
- Progress, timeout, cancellation Ã Â¢Ã¢â  Â¬ every async pattern has a standard answer.
