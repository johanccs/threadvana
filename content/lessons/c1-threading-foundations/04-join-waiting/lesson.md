---
id: c1-l04-join-waiting
category: c1-threading-foundations
order: 4
title: Join  -  Waiting for a Worker to Finish
difficulty: beginner
description: "Master Thread.Join(): the simple way to pause one thread until another thread finishes its work."
visualization: thread-timeline
explainer: thread-join
interview:
  - q: What does Thread.Join do?
    a: It pauses the current thread until the other thread has completely finished. That is how you say "wait for my worker" as a guarantee, instead of hoping the worker is done in time.
  - q: How do you wait for a thread but give up after a while?
    a: Use Join with a timeout, for example worker.Join(2000). It returns true if the thread finished in time and false if you gave up waiting, so you can decide what to do next.
  - q: What is IsAlive?
    a: A property that is true while a thread is still running. It is a peek, not a wait - useful for logging, or for deciding whether to wait at all.
---

## What is it?

`Join` is how one thread waits for another to finish. Calling `worker.Join()`
means "pause me here until worker is completely done." Without it, you are
guessing when the worker will be finished - and guessing wrong is the classic
bug.

## The real-world picture

You pay at the till and the cashier starts counting your change. You do not
walk out the moment you hand over the money - you wait at the till until the
change is in your hand. That wait is `Join`.

Leaving early is your program moving on before the worker's result exists.
`Join(2000)` is "I wait two minutes at the till, then I leave anyway."
`IsAlive` is glancing at the till to see if the cashier is still counting -
a peek, not a wait.

## How it works in C#

```csharp
using System;
using System.Threading;

var worker = new Thread(() =>
{
    Thread.Sleep(300);           // pretend to work
    Console.WriteLine("done!");  // the worker's result
});

worker.Start();

worker.Join();      // main PAUSES here until the worker is done
// From this line on, the worker's result is guaranteed to exist.

// The two useful variations:
bool finishedInTime = worker.Join(2000); // wait at most 2s - false means "gave up"
bool stillBusy = worker.IsAlive;         // peek: still running? (no waiting)
```

One line does the waiting, and it always goes in this order: **Start first,
Join after.**

## See it move

Press **Run demo**. Watch the main lane: it works for a bit, then turns grey
(waiting). First comes a short wait with a deadline - the worker is still
busy, so main gives up for now. Then comes a full `Join`: the grey
wait-stretch ends exactly where the worker's lane ends. That handover point
is `Join` doing its job.

## Watch out

- You might think a small `Thread.Sleep` on your own thread is "waiting long
  enough." It is a guess, not a guarantee - machines vary. `Join` is the
  guarantee.
- You might call `Join` on a thread that was never started. That throws an
  exception. Start first, Join after.
- You might wait forever when a deadline would be safer. `Join(timeout)`
  lets you give up gracefully and handle the "too slow" case.

## Key takeaways

- `worker.Join()` pauses your thread until worker is completely finished.
- After `Join` returns, the worker's results are guaranteed to exist.
- `Join(timeout)` waits with a deadline and returns false if it gave up.
- `IsAlive` peeks at whether a thread is still running - no waiting involved.
- Sleeping "long enough" is a guess; `Join` is a promise.
