---
id: c2-l05-task-run
category: c2-tasks-and-async-await
order: 5
title: Task.Run  -  Handing Work to the Thread Pool
difficulty: beginner
description: "Understand Task.Run: when to use it to push CPU-bound work to the thread pool, and when it is the wrong tool."
visualization: thread-pool
explainer: thread-pool
interview:
  - q: What is the difference between Task.Run and new Thread?
    a: Task.Run hands work to the thread-pool  -  it reuses idle workers instead of creating a new, expensive thread every time. The pool is faster for short bursts of work; new Thread gives you a long-lived worker you manage yourself. For almost everything in modern C#, Task.Run is the right call.
  - q: When should you NOT use Task.Run?
    a: Don't use Task.Run when you are already on an async Task-returning method  -  wrapping sync-over-async with Task.Run hides the problem and exhausts pool threads. Also avoid Task.Run for I/O work (network/disk)  -  the pool runs CPU work; I/O is better handled by truly async I/O methods.
---

## What is it?

`Task.Run` is the modern way to say "here is some work — give it to a pool worker so the main thread can keep going." It takes a delegate (a chunk of code wrapped in an `Action` or `Func<T>`) and returns a `Task` — a promise that the work will finish.

The old way was `new Thread(Worker).Start()`, which you learned in Category 1. But creating a thread is expensive (about 1 MB of stack, plus setup time), and threads sit idle after their work is done. `Task.Run` skips all that: it hands the work to the **thread pool**, a team of on-call workers .NET keeps ready. Same idea as `ThreadPool.QueueUserWorkItem`, but `Task.Run` gives you a `Task` you can `await`, instead of raw signals like `CountdownEvent`.

## The real-world picture

Imagine a restaurant kitchen. Creating a new thread is like hiring a brand-new cook for one five-minute task — onboarding, uniform, paperwork, all wasted when they finish. `Task.Run` is like posting the task on a whiteboard; any idle cook grabs it, does it, and goes back to waiting for the next one. Same result, zero hiring cost.

## How it works in C#

```csharp
// The old Category 1 way — new thread per task
var done = new CountdownEvent(2);
new Thread(() => { Work(1); done.Signal(); }).Start();
new Thread(() => { Work(2); done.Signal(); }).Start();
done.Wait();

// The modern way — hand both to the pool with Task.Run
var task1 = Task.Run(() => Work(1));
var task2 = Task.Run(() => Work(2));
await Task.WhenAll(task1, task2);
```

Three key facts:
1. `Task.Run` always returns **immediately** — you get a `Task` before the work finishes (if the work returns a value, it is `Task<T>`).
2. The work runs on a **background pool thread** — your app won't wait for it at shutdown unless you `await` or `Join` the result.
3. If the work **throws**, the exception is captured inside the `Task` — `await` will re-throw it, or you can check `task.Exception` without await.

## See it move

Press **Run demo** and watch the timeline. We post six tasks to a pool capped at 2 workers. Each task sleeps a bit — watch them queue up (`pool-queued`) and get picked up (`pool-dequeued`), two at a time. Six tasks, 2 swimlanes — zero new threads created.

## Watch out

> **Task.Run inside an async method is a smell.** If you write `await Task.Run(() => ...)`, ask yourself: could I just call `...` directly or use a truly async counterpart? Wrapping CPU work that is already on a background thread is wasteful.

> **Pool threads are background threads.** If your main thread exits without awaiting a Task.Run task, that work is silently abandoned. Always `await` or hold a reference.

> **Long-running pool tasks starve other work.** The pool has limited threads. A 30-second Task.Run blocks that worker for everything else. For genuinely long CPU work, use `Task.Factory.StartNew(..., TaskCreationOptions.LongRunning)` to get a dedicated, non-pool thread.

## Key takeaways

- `Task.Run` → pool, fast, modern. `new Thread` → dedicated worker, slower, only when you really need it.
- Returns `Task` or `Task<T>` — `await` to wait, or compose with `WhenAll`/`WhenAny`.
- Exceptions are captured inside the `Task`, not thrown to the caller until `await`ed.
- Don't Task.Run I/O, don't Task.Run inside async, don't leave pool tasks un-awaited.
