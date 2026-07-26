---
id: c1-l09-meet-the-thread-pool
category: c1-threading-foundations
order: 9
title: Meet the Thread Pool Ã Â¢Ã¢â  Â¬ Borrow Workers Instead of Hiring
difficulty: beginner
description: "Meet the thread pool: a team of reusable worker threads that saves you from the cost of creating threads manually."
visualization: thread-pool
interview:
  - q: What is the thread pool?
    a: A team of ready-made worker threads that .NET keeps on call for you. You hand in small tasks, idle workers pick them up, and when a task finishes the worker goes back on call - no hiring and firing per task.
  - q: Why is the thread pool better than new Thread for small tasks?
    a: Creating a thread costs memory and setup time every single time. The pool creates its workers once and reuses them, so a thousand small tasks run on a handful of threads instead of a thousand.
  - q: How do you run work on the thread pool?
    a: The direct way is ThreadPool.QueueUserWorkItem with the code to run. In modern code you usually write Task.Run, which also uses the pool - that is Category 2. Either way, you cannot Join pool threads, so you wait on a signal like CountdownEvent or on the Task itself.
---

## What is it?

The **thread pool** is a team of worker threads that .NET keeps on call for
you. Instead of hiring a new thread per task (expensive - see "The Cost of a
Thread"), you hand the task to the pool. An idle worker picks it up, does it,
and goes back on call for the next one.

## The real-world picture

A company with small jobs all day does not hire a new employee per job.
Imagine the overhead: desk, laptop, onboarding, farewell party - for twenty
minutes of work!

Instead it keeps a small on-call team. A job comes in, whoever is free grabs
it, finishes, and goes back on call. The team stays small; the jobs still
all get done. That team is the thread pool.

## How it works in C#

```csharp
using System;
using System.Threading;

// Hand a task to the pool - an on-call worker picks it up.
ThreadPool.QueueUserWorkItem(_ =>
{
    Console.WriteLine("Pool thread? " + Thread.CurrentThread.IsThreadPoolThread); // True
});

// Pool threads are BACKGROUND and not yours - you cannot Join them.
// To know when queued work is done, count finished tasks instead:
using var done = new CountdownEvent(2); // two tasks to wait for

ThreadPool.QueueUserWorkItem(_ => { /* work */ done.Signal(); });
ThreadPool.QueueUserWorkItem(_ => { /* work */ done.Signal(); });

done.Wait(); // main pauses until the count reaches zero
```

`CountdownEvent` is a countdown latch: create it with a count, each finished
task calls `Signal()`, and `Wait()` blocks until the count hits zero.

## See it move

Press **Run demo**. Six tasks are handed to a pool we capped at 2 workers.
Watch the queue: each task waits in line (`pool-queued`) until one of the
TWO workers is free (`pool-dequeued`). Six tasks, two swimlanes - each
worker handles three tasks, one after another. That is reuse, live.

## Watch out

- You might try to `Join` a pool thread. You cannot - the workers are not
  yours to hold. Count completions (`CountdownEvent`) or await Tasks
  (Category 2).
- You might queue long, blocking work. A sleeping pool worker is unavailable
  for everyone else's tasks. The pool is for short jobs.
- You might expect queued tasks to run in order. They run whenever a worker
  is free - no order, no guarantees, like every lesson so far.

## Key takeaways

- The thread pool is a team of reusable, on-call worker threads managed by
  .NET.
- `ThreadPool.QueueUserWorkItem` hands a task to the pool.
- Pool threads are background threads - you cannot Join them; count
  completions instead (`CountdownEvent`).
- Reuse beats hiring: 1000 small tasks should not mean 1000 threads (see
  "The Cost of a Thread").
- Modern code usually reaches the pool through `Task.Run` - that is
  Category 2.
