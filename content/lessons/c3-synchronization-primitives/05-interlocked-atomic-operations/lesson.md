---
id: c3-l05-interlocked-atomic-operations
category: c3-synchronization-primitives
order: 5
title: Interlocked Ã¢â‚¬â€ Atomic Operations Without a Lock
difficulty: beginner
description: "Go lock-free with Interlocked: atomic Increment, Decrement, Exchange, and CompareExchange - no lock object needed."
visualization: thread-timeline
explainer: race-interleaving
interview:
  - q: What does Interlocked.Increment do?
    a: It adds one to an int as a single atomic step - the CPU itself guarantees that no other thread can interrupt the read-add-write. No lock object, no waiting, and no lost increments. Bonus point - atomic means indivisible, so other threads only ever see the value before or after, never in between.
  - q: When would you choose Interlocked over lock?
    a: When the shared state is a single number and the operation is simple - increment, add, or swap a value. Interlocked is lighter than a lock because threads never queue up. Bonus point - the moment your rule spans two or more variables, like moving money between two accounts, you need a lock instead.
  - q: What does Interlocked.CompareExchange do?
    a: It sets a variable to a new value only if it still holds the old value you expected, all in one atomic step. It is the building block of lock-free code - compare, swap, and retry if someone beat you to it. Bonus point - the concurrent collections in Category 4 are built on exactly this idea.
---

## What is it?

**Interlocked** is a small set of *atomic* operations for numbers:
`Increment`, `Decrement`, `Add`, `Exchange`, `CompareExchange`. Atomic means
indivisible Ã¢â‚¬â€ the read-add-write from *Race Conditions Up Close* becomes ONE
step that no thread can interrupt.

It lives in `System.Threading` and works on `int` and `long` fields. No lock
object, no queuing threads Ã¢â‚¬â€ the CPU does the guarding.

## The real-world picture

Remember the two cashiers and the paper tally sheet? Interlocked replaces the
sheet with a mechanical clicker. Click = counted. There is no "read, think,
write" sequence for anyone to interrupt Ã¢â‚¬â€ the step has no middle.

That is why it needs no key and no queue: you cannot collide inside a step
that has no gaps.

## How it works in C#

```csharp
using System.Threading;

private static int _visitors = 0;

// The racy way (see Race Conditions Up Close):
// _visitors++;

// The atomic ways - each one indivisible:
Interlocked.Increment(ref _visitors);   // +1
Interlocked.Add(ref _visitors, 5);      // +5
Interlocked.Exchange(ref _visitors, 0); // reset to 0, atomically

// The lock-free building block: "set to 11 only if it is still 10"
Interlocked.CompareExchange(ref _visitors, 11, 10);
```

Note the `ref`: Interlocked needs the real variable, not a copy of its value,
so every method takes the field by reference.

## See it move

Press **Run demo**. Two pairs of workers do the same hammering as last lesson Ã¢â‚¬â€
one pair on a plain counter, one pair on an Interlocked counter. Watch both
pairs work, then read the totals: the plain counter falls short (and changes
every run), the Interlocked counter is exact EVERY time.

## Watch out

- You might protect one variable with Interlocked and leave a second one
  plain. Interlocked guards exactly ONE variable per call Ã¢â‚¬â€ rules spanning
  two variables need a `lock`.
- You might think Interlocked fixes check-then-act logic. "If balance is
  enough, withdraw" is two steps; Interlocked cannot fuse them.
- You might read the field plainly while others write it. A plain read can
  see a stale value Ã¢â‚¬â€ use `Volatile.Read` for a fresh snapshot.

## Key takeaways

- Interlocked makes read-add-write one indivisible CPU step Ã¢â‚¬â€ no lock needed.
- Use it for single-number updates: `Increment`, `Add`, `Exchange`.
- `CompareExchange` = "swap only if unchanged" Ã¢â‚¬â€ the lock-free building block.
- Threads never queue for Interlocked, so it beats a lock for tiny counter work.
- Two or more related variables? That is `lock` territory, not Interlocked.
