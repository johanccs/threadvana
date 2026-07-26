---
id: c1-l05-foreground-vs-background
category: c1-threading-foundations
order: 5
title: Foreground vs Background Threads
difficulty: intermediate
description: "Understand the critical difference between foreground and background threads and when each type is appropriate."
visualization: thread-timeline
explainer: foreground-background
interview:
  - q: What is the difference between a foreground and a background thread?
    a: The program stays alive until its last foreground thread ends. Background threads do not keep it alive - when the last foreground thread ends, they are cut off instantly, with no warning and no finally block. New threads are foreground by default.
  - q: When would you make a thread background?
    a: For work that must never hold the program open, like a looping monitor or cleanup helper. If the work has to finish - saving a file, writing a result - keep it foreground or Join it.
  - q: Your background thread sometimes never finishes its work. Why?
    a: Because the program exited before the thread was done. Background threads are cut off the moment the last foreground thread ends. If the work matters, Join the thread or leave it foreground.
---

## What is it?

Every thread is either **foreground** (the default) or **background**. The
difference only matters at the end: a program stays alive until its last
**foreground** thread ends. Background threads do not count. When the last
foreground thread ends, background threads are cut off instantly - mid-line,
no `finally`, no goodbye.

## The real-world picture

A shop stays open as long as at least one employee is inside. Contractors may
still be stacking shelves, but they do not count. The moment the last employee
walks out, the lights go off and the door locks - mid-shelf, no warning.

Employees are foreground threads. Contractors are background threads. If a
contractor's work must actually get done, someone has to stay and wait for
them - or make them an employee.

## How it works in C#

```csharp
using System;
using System.Threading;

var worker = new Thread(() =>
{
    for (int i = 1; i <= 10; i++)
    {
        Thread.Sleep(100);
        Console.WriteLine("tick " + i);
    }
});

// Must be set BEFORE Start(). Default is false (= foreground).
worker.IsBackground = true;

worker.Start();

// If the main thread ends here, the program exits -
// and the worker vanishes after only a tick or two.
worker.Join(); // Want the work DONE? Then wait for it, whatever IsBackground says.
```

Three facts to hold on to:
- `new Thread(...)` is **foreground by default**.
- `IsBackground` must be set **before** `Start()`.
- Thread-pool threads (coming in a later lesson) are always background.

## See it move

Press **Run demo**. The background worker has a 10-step job, but the "program"
(the main lane) ends after only a few steps. Watch the background lane: it
stops right there - "CUT OFF at step 4 of 10!" - because no foreground thread
was left to keep the program alive. (The demo pretends to exit so the story
fits in a second; in a real program the process itself would end.)

## Watch out

- You might think `IsBackground = true` means "less important, but it still
  finishes." It does not - the thread might not finish at all.
- You might set `IsBackground` after `Start()`. That throws an exception.
  Set it before.
- You might think making a thread foreground is a way to wait for it. It is
  not: foreground only keeps the PROCESS alive, your method still returns
  immediately. To wait, use `Join` (see "Join Ã Â¢Ã¢â  Â¬ Waiting for a Worker to
  Finish").

## Key takeaways

- Foreground threads keep the program alive; background threads do not.
- The program exits when the last foreground thread ends - background threads
  are cut off mid-work, with no `finally` block.
- `new Thread()` is foreground by default; set `IsBackground` before `Start()`.
- Foreground vs background decides who keeps the PROCESS alive. It never makes
  anyone WAIT - that is what `Join` is for.
- Work that must finish (saving, writing results): keep it foreground or Join
  it.
