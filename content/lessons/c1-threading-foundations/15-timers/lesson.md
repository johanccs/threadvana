---
id: c1-l15-timers
category: c1-threading-foundations
order: 15
title: Timers  -  Running Code on a Schedule
difficulty: intermediate
description: "Explore System.Threading.Timer and PeriodicTimer for scheduling work to run on a timer without blocking threads."
visualization: thread-pool
interview:
  - q: Which timer class should you use in a .NET Core application?
    a: System.Threading.Timer for repeating background work. It runs callbacks on thread-pool threads and is the lightest option. Avoid System.Timers.Timer (wraps System.Threading.Timer with unnecessary ceremony) and System.Windows.Forms.Timer (only works on UI threads).
  - q: How do you stop a repeating timer?
    a: Call timer.Change(Timeout.Infinite, Timeout.Infinite) to disable it, or Dispose() when you are done. Dispose waits for any currently-running callback to finish unless you pass the waitHandle timeout.
---

## What is it?

A **timer** runs a piece of code automatically after a delay, or on a repeating
schedule. Instead of looping with `Thread.Sleep`, you register a callback and the
timer calls it for you on a thread-pool thread.

## The real-world picture

A kitchen timer buzzes once after 5 minutes. A wall clock ticks every second.
`System.Threading.Timer` can do both: one-shot ("notify me in 3 seconds") and
repeating ("check the queue every 10 seconds").

## How it works in C#

```csharp
// One-shot: run DoWork once, 2 seconds from now.
using var timer = new Timer(_ => DoWork(), null, dueTime: 2000, period: Timeout.Infinite);

// Repeating: run CheckQueue every 5 seconds.
using var timer2 = new Timer(_ => CheckQueue(), null, dueTime: 0, period: 5000);

// Stop: change to infinite timeout.
timer2.Change(Timeout.Infinite, Timeout.Infinite);
```

The callback runs on a **thread-pool thread**. That means it should be quick, not
block, and never throw unhandled exceptions (that kills the pool worker).

## See it move

Press **Run demo**. A one-shot timer fires after 1 second, then a repeating timer
ticks 3 times at 200ms intervals. Watch the pool workers appear.

## Watch out

- A timer keeps the app alive. If you forget to `Dispose` or stop it, the program
  will not exit.
- Timer callbacks should be FAST. If one takes 2 seconds, the next scheduled tick
  may overlap Ã¢â‚¬â€ use a flag or lock to prevent re-entry.
- `System.Timers.Timer` (with an `s`) is an older wrapper Ã¢â‚¬â€ just use
  `System.Threading.Timer` in new code.

## Key takeaways

- `System.Threading.Timer` schedules callbacks on the thread pool.
- `dueTime` = first delay; `period` = repeat interval (or `Timeout.Infinite` for
  one-shot).
- Call `Change` to adjust, `Dispose` to clean up.
- Keep callbacks short and never let them throw.
