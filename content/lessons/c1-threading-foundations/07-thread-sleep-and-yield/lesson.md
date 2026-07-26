---
id: c1-l07-thread-sleep-and-yield
category: c1-threading-foundations
order: 7
title: Pausing a Thread  -  Sleep and Yield
difficulty: beginner
description: "Explore Thread.Sleep and Thread.Yield: when to pause a thread, and when yielding is better than sleeping."
visualization: thread-timeline
explainer: thread-basics
interview:
  - q: What does Thread.Sleep do?
    a: It pauses the current thread for at least the given time. While sleeping, the thread does no work and uses no CPU, and the operating system wakes it up afterwards. It is the standard way to wait or pace work.
  - q: What is the difference between Thread.Sleep(0) and Thread.Yield?
    a: Both say "let someone else have a turn" - a polite offer the operating system may ignore if nobody else needs the CPU. Sleep with a real duration is the strong version, since it always pauses for at least that long.
  - q: Is Thread.Sleep accurate?
    a: No - it promises at least the time you asked for, not exactly that time. The operating system wakes the thread when it gets to it, often with a delay of several milliseconds. Never use Sleep for precise timing.
---

## What is it?

`Thread.Sleep` pauses the current thread for a set time - a timed break where
the thread does nothing and uses no CPU. `Thread.Yield` is a quick courtesy:
"let someone else go ahead of me" - an offer the operating system may ignore.

## The real-world picture

Sleep is a worker taking a timed coffee break. They are not working, not
blocking the counter, and they come back when the timer rings. On the
timeline you will see it as a grey gap in their lane.

Yield is a worker at the coffee machine saying "you go ahead of me in line."
Polite! But if nobody is behind them, they just keep making coffee. An offer,
not a guarantee.

## How it works in C#

```csharp
using System;
using System.Threading;

// SLEEP: guaranteed pause of AT LEAST 100ms. No CPU used while waiting.
Thread.Sleep(100);

// SLEEP(0) and YIELD: "anyone else want a turn?" - a polite offer.
// The operating system may ignore it if nobody else is waiting.
Thread.Sleep(0);
Thread.Yield();

// Typical use: stay polite inside a long, busy loop.
for (int i = 0; i < 2000; i++)
{
    // ... crunch numbers ...
    if (i % 100 == 99)
        Thread.Yield(); // offer the CPU to others every 100 rounds
}
```

## See it move

Press **Run demo**. Watch the "sleepy" lane: work, then a long grey gap (that
grey span IS the `Sleep`), then more work. The "polite" lane crunches numbers
in rounds and logs "you go ahead of me!" between rounds - those are Yields.
Notice the polite lane never turns grey: a Yield has no set duration, it is
over in a blink.

## Watch out

- You might think `Sleep(100)` means exactly 100ms. It means AT LEAST 100ms -
  the operating system wakes the thread when it gets to it. Never use `Sleep`
  for precise timing.
- You might think `Yield` guarantees someone else runs. It does not - if
  nobody is waiting, your thread just continues. Yield is politeness, not
  synchronization.
- You might use `Sleep` to "wait for" another thread's result. That is a
  guess, and guesses flake - use `Join` (see "Join Ã¢â‚¬â€ Waiting for a Worker to
  Finish"). Sleep is for pacing, not for waiting on results.

## Key takeaways

- `Thread.Sleep(ms)` pauses the current thread for at least that long, using
  no CPU.
- A Sleep shows up on the timeline as a grey wait span.
- `Thread.Yield` (and `Sleep(0)`) offer the CPU to others - an offer, not a
  guarantee.
- An occasional Yield inside a long loop keeps your thread polite.
- Sleep is neither exact timing nor a way to wait for results - `Join` is.
