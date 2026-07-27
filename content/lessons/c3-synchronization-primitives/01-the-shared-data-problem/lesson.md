---
id: c3-l01-the-shared-data-problem
category: c3-synchronization-primitives
order: 1
title: The Shared Data Problem  -  When Workers Trip Over Each Other
difficulty: beginner
description: "Understand the fundamental problem: when two threads touch the same data, bad things happen. This lesson sets up everything that follows."
visualization: thread-timeline
explainer: race-interleaving
interview:
  - q: What is a race condition?
    a: A race condition happens when two threads use the same data at the same time and the result depends on which one wins. Even one line like counter++ can race, because it is really three steps - read, add, write. Bonus point - races depend on unlucky timing, which is why they can pass tests all week and fail once in production.
  - q: Why can counter++ lose increments when two threads run it?
    a: counter++ reads the value, adds one, and writes it back. If both threads read before either writes, they both write the same result and one increment disappears. Bonus point - over thousands of iterations the losses are random, so the wrong total is different every run.
  - q: What does the lock keyword do?
    a: lock takes an object as a key and lets only one thread inside the braces at a time; the others wait at the door. If every thread locks on the same key before touching shared data, the delicate steps can no longer interleave. Bonus point - the waiting is exactly why locked sections should stay as tiny as possible.
---

## What is it?

When two threads touch the same data at the same time, they can corrupt it —
even with a single line like `counter++`. This is called a **race condition**:
the result depends on which thread happens to win an unlucky race.

A **`lock`** is the simplest fix: it makes one thread wait while the other
finishes the delicate step.

## The real-world picture

Two baristas share one paper order book. Anna reads "3 orders" and gets
distracted by a customer. Bruno reads "3 orders" too, writes "4", and moves on.
Anna comes back and writes "4" as well — she still had "3" in her head.

Two customers arrived, but the book says one. Neither barista made a mistake.
The *sharing* was the mistake.

## How it works in C#

`counter++` looks like one step. It is actually THREE:

```csharp
// counter++ really means:
int temp = counter;   // 1. READ
temp = temp + 1;      // 2. ADD
counter = temp;       // 3. WRITE
```

Two threads can interleave these steps: both read 5, both write 6. Two
increments happen, the counter goes up by ONE. Repeat a thousand times and the
total is anyone's guess.

The fix is a lock — think of a single bathroom key; only one person inside at
a time:

```csharp
private static readonly object _gate = new object(); // the ONE shared key

lock (_gate)   // only ONE thread may be inside the braces at a time
{
    counter++; // read + add + write now runs as one unbreakable step
}
```

Both threads must lock on the SAME object — two different keys open two
different bathrooms.

## See it move

Press **Run demo**. Two threads each increment a shared counter 1000 times with
NO protection. Watch their work spans overlap, then look at the final total: it
comes out BELOW 2000, and a little different almost every run. Those missing
numbers are increments that collided mid-step and vanished.

## Watch out

- You might think `counter++` is one unbreakable step. It is three steps, and
  threads can interleave between any of them.
- You might think "it worked on my machine". Races hide: they need unlucky
  timing, so they pass 99 times and fail once — usually in production.
- You might lock on the wrong thing. If each thread locks its OWN key, nobody
  ever waits, and the race is still there. One shared key, or no protection.

## Key takeaways

- Shared data + multiple threads + no protection = race conditions.
- `counter++` is read + add + write — never one step.
- Races need unlucky timing, so they hide in tests and strike randomly.
- `lock (key) { ... }` lets only one thread run the delicate section at a time.
- Every thread must lock on the SAME object, or the lock does nothing.
