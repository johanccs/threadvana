---
id: c1-l10-threadpool-reuse-in-action
category: c1-threading-foundations
order: 10
title: Watch the Pool Reuse Its Workers
difficulty: beginner
description: "Watch the thread pool reuse workers across multiple tasks and see why this is dramatically faster than creating new threads."
visualization: thread-pool
interview:
  - q: You queue 100 small tasks on the thread pool. How many threads run them?
    a: Usually just a handful - far fewer than 100. The pool keeps a small team and every worker runs many tasks one after another. That reuse is the whole point.
  - q: How can you tell the pool is reusing threads?
    a: Record the thread id of each task as it runs and count the distinct values at the end. Ten tasks but only a few distinct ids means workers were borrowed and returned.
  - q: Why is pool reuse better than one thread per task?
    a: Each new thread costs about 1 MB of stack plus setup time (see The Cost of a Thread). Reusing a small team avoids both, so small tasks start faster and the app stays light.
---

## What is it?

Queue a pile of small tasks and watch closely: only a handful of DISTINCT
workers ever touch them. The pool lends you a worker per task, and the moment
the task ends, that worker goes back on call and grabs the next one. Borrow,
return, repeat.

## The real-world picture

Back to the on-call team from "Meet the Thread Pool Ã Â¢Ã¢â  Â¬ Borrow Workers Instead
of Hiring". Ten packages arrive at the depot - and you do NOT see ten
couriers. You see the same two or three faces, over and over, because every
delivery ends with the courier coming back for the next package.

This lesson is simply watching the faces and counting them.

## How it works in C#

```csharp
using System;
using System.Collections.Concurrent;
using System.Threading;

// task id -> the id of the thread that handled it
var whoDidWhat = new ConcurrentDictionary<int, int>();
using var done = new CountdownEvent(10);

for (int i = 1; i <= 10; i++)
{
    int mine = i; // own copy per task - the capture trap never sleeps
    ThreadPool.QueueUserWorkItem(_ =>
    {
        Thread.Sleep(100); // pretend to work
        whoDidWhat[mine] = Environment.CurrentManagedThreadId;
        done.Signal();
    });
}

done.Wait(); // pool threads are background - wait on the countdown
// Now count the distinct VALUES in whoDidWhat... expect a small number!
```

## See it move

Press **Run demo**. Ten tasks, and the pool is capped at four workers. Watch
the swimlanes: only four lanes appear, and each lane lights up two or three
times as its worker comes back for the next task. The closing message counts
the distinct workers out loud. Now compare lesson 8: 200 threads doing
nothing - here, 4 threads doing everything.

## Watch out

- You might expect one swimlane per task. Tasks outnumber workers on
  purpose - that is the reuse you came to see.
- You might read the results before all tasks have signaled. Wait for the
  countdown first, or your count keeps changing while you look at it.
- You might skip the `int mine = i;` copy because "it worked last time."
  It is a race, and races are lost at the worst moment. Keep the copy.

## Key takeaways

- Many tasks, few workers: the pool borrows and returns instead of hiring.
- Count distinct thread ids to SEE the reuse with your own eyes.
- Pool workers are background threads - wait on a `CountdownEvent`, not
  `Join`.
- Reuse is why the pool beats one-thread-per-task (remember lesson 8's
  200 MB!).
- The capture trap applies to queued lambdas too: copy the loop variable.
