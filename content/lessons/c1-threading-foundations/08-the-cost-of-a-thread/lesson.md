---
id: c1-l08-the-cost-of-a-thread
category: c1-threading-foundations
order: 8
title: The Cost of a Thread
difficulty: intermediate
description: "See the real cost of creating threads: memory overhead, CPU context switching, and why one-thread-per-task does not scale."
visualization: thread-timeline
explainer: thread-pool
interview:
  - q: How much memory does a thread cost?
    a: About 1 MB of stack space each by default, plus some bookkeeping. That sounds small until you multiply - a thousand threads reserve about a gigabyte before they have done any work at all.
  - q: What is a context switch?
    a: When the CPU stops running one thread and starts another, it must save one thread's place and reload the other's - like a manager re-reading each worker's notes before every check-in. A few switches are fine; thousands per second become pure overhead.
  - q: Why not just make 10,000 threads for 10,000 tasks?
    a: 10,000 threads reserve about 10 GB of stack, and the CPU spends its time switching between them instead of working. The fix is reuse - a thread pool keeps a small team and hands tasks to it, which is the next lesson.
---

## What is it?

A thread is not free. Every thread reserves about **1 MB of memory** for its
**stack** (its private scratch space for method calls), and the operating
system spends CPU time juggling all your threads - that juggling is called
**context switching**. A few threads: nothing to worry about. Thousands: the
overhead becomes the whole show.

## The real-world picture

Every worker you hire needs their own desk - even if they mostly sit idle.
Two hundred workers means two hundred desks, and the office budget feels it.

And picture the manager: walking desk to desk, re-reading each worker's notes
before speaking to them. That is a context switch. With ten workers it is a
rounding error. With ten thousand, the manager spends the whole day
re-briefing and no real work happens.

## How it works in C#

```csharp
using System;
using System.Threading;

// Every started thread reserves ~1 MB for its stack - immediately,
// even if all it does is sleep.
var worker = new Thread(() => Thread.Sleep(5000));
worker.Start();

// The math that matters:
//    10 threads  ->   ~10 MB  (nothing)
// 1,000 threads  ->   ~1 GB   (ouch)
// 10,000 threads ->  ~10 GB   (disaster)

// Rule of thumb: for CPU-hungry work, about one thread per CPU core.
// For "lots of small tasks"? Borrow workers from the pool - next lesson.
```

## See it move

Press **Run demo**. The demo hires 200 workers that do nothing but wait
around. Watch the messages: 200 threads x ~1 MB = ~200 MB of stack, reserved
in a blink - for 600ms of waiting. The timeline stays almost empty (they
really do nothing!), which is exactly the point: all that cost, zero work.

## Watch out

- You might think "threads are just objects - make as many as you like."
  Each one is a real operating-system worker with a real ~1 MB desk.
- You might think idle threads cost nothing. An idle thread holds onto its
  stack the entire time it exists.
- You might try to fix "10,000 tasks" with "10,000 threads." The fix is
  reuse, not more desks - the thread pool (next lesson) exists exactly for
  this.

## Key takeaways

- Every thread reserves ~1 MB of stack, whether it works or sleeps.
- Context switches are the CPU re-briefing workers - a few are fine,
  thousands are pure overhead.
- 10,000 threads ~ 10 GB of stacks: the classic way to kill a server.
- The right number of CPU-hungry threads is roughly the number of cores.
- Many small tasks? Borrow workers from the thread pool instead of hiring
  new ones.
