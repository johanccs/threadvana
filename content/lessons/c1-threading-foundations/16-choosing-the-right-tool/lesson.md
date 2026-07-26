---
id: c1-l16-choosing-the-right-tool
category: c1-threading-foundations
order: 16
title: Choosing the Right Tool  -  Sync, Async, or Parallel?
difficulty: intermediate
description: "A decision flowchart: Thread vs ThreadPool vs Task - which tool should you use for which kind of work?"
visualization: thread-timeline
explainer: thread-basics
interview:
  - q: When would you use a new Thread vs the ThreadPool vs async/await?
    a: new Thread for a long-running, dedicated background worker that lives for the app's lifetime. ThreadPool for many short, independent work items you want to hand off and forget. async/await for I/O-bound work (files, network, database) that should never block a thread while waiting.
  - q: What is the difference between parallel and asynchronous?
    a: Parallel means multiple workers doing CPU work at the same time (Parallel.For, multiple threads). Asynchronous means not waiting  -  a single thread issues a request and goes do other work while it completes (async/await). A breakfast is asynchronous; a kitchen with 4 cooks is parallel.
---

## What is it?

After 15 lessons, you have four toolboxes:
1. **new Thread** Ã¢â‚¬â€ hire a dedicated worker. Best for: long-running, permanent background jobs (a file watcher, a game loop).
2. **ThreadPool** Ã¢â‚¬â€ borrow a pool worker. Best for: hundreds of short independent tasks (handling web requests, queue processing).
3. **async/await** Ã¢â‚¬â€ order and get a buzzer. Best for: I/O (calling an API, reading a file, querying a database). Never blocks a thread while waiting.
4. **Parallel** Ã¢â‚¬â€ split CPU work across cores. Best for: heavy number-crunching on large data (Parallel.For, PLINQ).

## The real-world picture

You run a cafÃƒÂ©:
- **new Thread** = hiring a full-time cleaner who works all shift.
- **ThreadPool** = an on-call temp agency Ã¢â‚¬â€ you phone for a waiter when a bus arrives.
- **async/await** = online ordering with a buzzer Ã¢â‚¬â€ you place the order and immediately go help another customer.
- **Parallel** = four baristas all making different drinks at the same time.

## How to decide (two questions)

**Does it wait on something external?** (file, network, DB) Ã¢â€ â€™ use async/await.

**Is it CPU-bound and heavy?** (math, image processing, parsing)
   - Lots of independent items Ã¢â€ â€™ Task.Run / ThreadPool / Parallel.
   - One big item you must wait for Ã¢â€ â€™ Task.Run to offload it, keep UI responsive.
   - Sequential dependency Ã¢â€ â€™ run it inline (the caller's thread). Parallelism won't help.

**Neither?** Just run it inline. Extra threads slow things down.

## See it move

Press **Run demo**. It runs a dummy numeric calculation four ways:
inline (blocking), Task.Run, Parallel.For, and async (simulated). Compare the
timeline Ã¢â‚¬â€ inline blocks the main lane while the others distribute work.

## Watch out

- Never use `async void` except in event handlers. It cannot be awaited.
- `Task.Run` is NOT for I/O work Ã¢â‚¬â€ I/O already uses async under the hood. Task.Run
  for I/O wastes a pool thread that sits idle.
- Parallel.For is NOT for I/O. It blocks pool threads while waiting Ã¢â‚¬â€ use
  async/await with Task.WhenAll instead.

## Key takeaways

- **CPU work** Ã¢â€ â€™ ThreadPool / Task.Run / Parallel.For.
- **I/O work** Ã¢â€ â€™ async/await. Never block a thread for I/O.
- **Long-running** Ã¢â€ â€™ dedicated Thread (not pool).
- **Inline** Ã¢â€ â€™ if it's fast and you need the result now.
