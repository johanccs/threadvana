---
id: c3-l04-race-conditions-up-close
category: c3-synchronization-primitives
order: 4
title: Race Conditions Up Close  -  Watching a Torn Read Happen
difficulty: beginner
description: "See race conditions up close: watch two threads fight over a counter and understand why the result is unpredictable."
visualization: thread-timeline
explainer: race-interleaving
interview:
  - q: What is a torn read?
    a: A torn read is when a thread reads a value while another thread is in the middle of updating it, so it sees a half-old, half-new mix that never really existed. It happens with updates that are not atomic, like counter++ or a 64-bit long on a 32-bit machine. Bonus point - protecting every read AND write with the same lock makes torn reads impossible.
  - q: Why do race conditions often pass tests but fail in production?
    a: A race needs unlucky timing - one thread must interrupt the other at exactly the wrong line. Tests are short and gentle, so the unlucky moment may never happen; production runs for hours under real load, so it eventually does. Bonus point - you reproduce races on purpose with many threads and many iterations, then you measure the damage.
  - q: How can you make a race condition visible on demand?
    a: Remove all protection, use several threads, and give each one a lot of iterations - for example 6 threads doing 100,000 counter++ each. The final total almost always lands below the expected 600,000, and it is different every run. Bonus point - the fact that the wrong total is random is itself the fingerprint of a race.
---

## What is it?

A **race condition** (you met it in *The Shared Data Problem*) happens when two
threads use the same data at the same time and the result depends on who wins.
This lesson puts that bug under a microscope: you will make a race happen ON
PURPOSE, measure the damage, and learn its fingerprint.

A **torn read** is the close-up version: a thread reads a value while another
thread is halfway through writing it, so it sees a mix that never really
existed.

## The real-world picture

Two cashiers share one paper tally sheet. Both read "41", both add one in
their head, both write "42". Two customers served — the sheet says 42. One
sale vanished, and nobody dropped anything. The sheet simply cannot handle
two writers at once.

The sneaky part: on slow days the cashiers never collide. The bug only shows
up on the busiest day of the year. That is exactly how races behave in code.

## How it works in C#

`counter++` looks like one step. It is really three:

```csharp
// What you write:
counter++;

// What the computer actually does:
int temp = counter;   // 1. READ
temp = temp + 1;      // 2. ADD
counter = temp;       // 3. WRITE
```

If thread A reads 41 and thread B reads 41 before A writes, both write 42.
One increment is gone forever. No exception, no warning — the number is just
silently wrong.

```csharp
// Reproduce it on demand: 6 threads, 100,000 increments each.
for (int t = 0; t < 6; t++)
{
    new Thread(() =>
    {
        for (int i = 0; i < 100_000; i++)
            counter++;          // unprotected = race fuel
    }).Start();
}
// Expected: 600,000. Actual: less — and different every single run.
```

## See it move

Press **Run demo**. Two workers each add 100,000 to one shared counter. Watch
their work spans overlap on the timeline — that overlap is where increments
vanish. The demo then prints expected vs actual: the gap is the race, caught
on camera. Run it again and the wrong number changes. A *different* wrong
answer each run is the race's fingerprint.

## Watch out

- You might think "it worked on my machine" means the code is safe. Races hide
  until timing gets unlucky — usually under real production load.
- You might protect only the WRITE. The read must be protected too; it is the
  read-add-write COMBINATION that must not be interrupted.
- You might expect a big, loud failure. Races rarely crash. They return
  slightly wrong numbers, which is far worse than a crash.

## Key takeaways

- `counter++` is really read-add-write — three steps another thread can interrupt.
- A torn read sees a value mid-update: half old, half new.
- Races need unlucky timing, so they pass tests and fail in production.
- Reproduce on purpose: many threads ÃÆ'â€" many iterations, then compare totals.
- A different wrong total every run is the fingerprint of a race.
