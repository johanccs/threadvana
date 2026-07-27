---
id: c1-l14-cooperative-cancellation
category: c1-threading-foundations
order: 14
title: Telling a Thread to Stop  -  Cooperative Cancellation
difficulty: intermediate
description: "Learn cooperative cancellation with CancellationToken: the right, safe way to ask a running thread to stop."
visualization: thread-timeline
explainer: cancellation
interview:
  - q: How do you stop a running thread?
    a: "You do not stop a thread by force (Thread.Abort does not exist on .NET Core). Instead ask it to cooperate  -  a bool flag the worker checks regularly. In modern .NET, CancellationToken is the standard."
  - q: Why not kill a thread?
    a: "A forced-killed thread might be holding a lock, a file handle, or mid-update of shared data. Killing it leaves the program corrupted. Cancellation must always be cooperative."
---

## What is it?

You cannot safely kill a thread from the outside. The thread might be holding a lock,
writing to a file, or updating shared data — killing it could leave everything broken.

Instead, you ask the thread to stop **politely** and it checks your request while it
works. A simple `volatile bool _shouldStop` flag, checked in a loop, is the simplest
form. In real code you use `CancellationToken` (Category 2).

## The real-world picture

You cannot pull the plug on a surgeon mid-surgery. You knock on the door and hand
her a note. She reads it between steps, and when it says "stop", she finishes the
current stitch, puts down the tools, and walks out.

The `_shouldStop` flag is the note. The `while (!_shouldStop)` loop is the surgeon
checking the note between stitches.

## How it works in C#

```csharp
private static volatile bool _shouldStop;

var worker = new Thread(() =>
{
    while (!_shouldStop)
    {
        // Do one unit of work.
        Thread.Sleep(50);
    }
    // Clean up, then exit.
    Console.WriteLine("Worker stopped cleanly.");
});
worker.Start();

// … later, from another thread:
_shouldStop = true;
worker.Join(); // wait for the clean exit
```

The `volatile` keyword makes sure the worker actually SEES the new value — without
it, the compiler or CPU might cache `_shouldStop` and the worker never notices.

## See it move

Press **Run demo**. The worker runs until the main thread sets the stop flag. Watch
the worker lane end cleanly after the flag, not mid-task.

## Watch out

- Without `volatile`, the worker might run forever. The CPU can cache the value
  in a register and never re-read from memory. Category 3 covers this deeper.
- You might check the flag too rarely. If a unit of work takes 10 seconds and you
  only check at the end, the thread ignores your request for 10 seconds. Check often!
- .NET Core has no `Thread.Abort`. That's a good thing — cooperative cancellation
  is the only correct way. Use it.

## Key takeaways

- Never kill a thread — ask it to stop politely.
- A `volatile bool` flag + a loop that checks it = the simplest stop signal.
- `volatile` is the minimum you need to make sure the flag works across threads.
- `CancellationToken` (next category) builds on exactly this idea, with a richer API.
