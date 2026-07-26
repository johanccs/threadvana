---
id: c1-l17-debugging-multithreaded-code
category: c1-threading-foundations
order: 17
title: Debugging Multithreaded Code  -  Survival Tactics
difficulty: advanced
description: "Practical techniques for debugging multithreaded code: breakpoints, the Threads window, and Parallel Stacks in Visual Studio."
visualization: thread-timeline
explainer: race-interleaving
interview:
  - q: How do you debug a race condition that only happens once every fifty runs?
    a: First, give every thread a unique Name so stack traces make sense. Second, add strategic logging (timestamps, thread ids). Third, run the suspected section in a loop or under stress (10x+ iterations). Races become near-certainties under load.
  - q: Why are multithreaded bugs harder to fix than single-threaded ones?
    a: Because they are non-deterministic  -  they depend on the exact timing of the OS scheduler. The same code can work 49 times and fail on the 50th. This makes reproducing the bug the first and hardest step.
---

## What is it?

Single-threaded bugs happen the same way every time. Multithreaded bugs play
hide-and-seek Ã¢â‚¬â€ they might only show up 2% of runs because they depend on the
exact order the operating system schedules the threads.

Three survival tactics:
1. **Name every thread** so your logs say "data-worker" instead of "Thread 14".
2. **Log ruthlessly** Ã¢â‚¬â€ timestamps, thread id, what the thread is about to do.
3. **Reproduce under stress** Ã¢â‚¬â€ run the buggy section 100 times in a loop.

## The real-world picture

A waiter drops a glass every time he walks through a specific doorway. If you watch
one walk-through, you miss it. If you stand at that doorway for an hour, you'll
definitely catch it.

Running your code 100 times in a loop is standing at the doorway.

## How it works in C#

```csharp
var t = new Thread(() => { /* ... */ });
t.Name = "data-uploader"; // a life-saver in logs
t.Start();
```

And the simplest log: `Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{Thread.CurrentThread.Name}] doing X");`

## See it move

Press **Run demo**. Three named threads each log their work. The timeline shows
their names, making the trace human-readable. Now imagine the same trace with
"Thread 7, Thread 8, Thread 11" Ã¢â‚¬â€ debugging blind.

## Watch out

- Console.WriteLine is SLOW and can actually change thread timing, hiding the
  bug. Consider collecting log entries in a thread-safe queue and writing them
  after the test finishes.
- Thread.Name can only be set ONCE. Set it before calling Start().
- Don't rely ONLY on debugger breakpoints Ã¢â‚¬â€ they freeze time and hide races.

## Key takeaways

- Name every thread Ã¢â‚¬â€ it's a one-liner that saves hours of debugging.
- Log what each thread does, with timestamps and thread ids.
- Reproduce races in a tight loop: `for (var i = 0; i < 100; i++) { BuggyCode(); }`.
