---
id: c3-l02-lock-one-key
category: c3-synchronization-primitives
order: 2
title: lock Ã Â¢Ã¢â  Â¬ One Key to the Bathroom
difficulty: beginner
description: "Master the lock statement: the simplest tool for protecting shared data. Think of it as a single bathroom key - only one thread inside at a time."
visualization: thread-timeline
explainer: lock-key
interview:
  - q: What does the lock keyword do in C#?
    a: lock takes an object as a key and lets only one thread run the locked block at a time; every other thread that reaches the same lock waits until the first one exits. Bonus point - lock is shorthand for Monitor.Enter and Monitor.Exit in a try/finally, so the key is always handed back even if the block throws.
  - q: What should you use as the lock object?
    a: A private readonly object field that exists only for locking, like private static readonly object gate = new object(). Private so nobody outside can grab your key and jam your lock, readonly so it is never swapped mid-flight. Bonus point - never lock on this, on a string (strings are interned and shared!), or on a value type.
  - q: Why should the locked section be as small as possible?
    a: While one thread holds the key, every other thread waits - the locked code is effectively single-threaded. The bigger the section, the more your program behaves like a queue. Bonus point - lock only the lines that touch shared data, and do the slow work outside the lock.
---

## What is it?

`lock` is C#'s bathroom key. `lock (key) { ... }` means: **only one thread may
run the code between the braces at a time.** Every other thread that reaches
the same lock WAITS at the door until the first one comes out.

In *The Shared Data Problem Ã Â¢Ã¢â  Â¬ When Workers Trip Over Each Other* you saw the
race; this lesson is about the key itself.

## The real-world picture

A small cafÃ Æ Ã Â© has one bathroom, and its one key hangs at the counter. Whoever
wants in takes the key; the next person politely waits. Nobody argues, nobody
walks in on anyone.

The key works not by magic, but because EVERYONE uses the same key before
entering. A second key hanging next to it would ruin the whole system.

## How it works in C#

```csharp
// The key: a private, readonly object that exists ONLY to be locked on.
private static readonly object _gate = new object();

lock (_gate)                    // take the key (or wait for it)
{
    // CRITICAL SECTION - only one thread at a time in here.
    balance = balance - 100;
}                               // the key is handed back automatically
```

Three rules for the key:

- Make it `private` Ã Â¢Ã¢â  Â¬ if outsiders can grab your key, they can jam your lock.
- Make it `readonly` Ã Â¢Ã¢â  Â¬ never swap the key while threads are flying.
- Keep the locked part TINY Ã Â¢Ã¢â  Â¬ remember, everyone else is waiting outside.

And what happens to the threads that wait? Exactly that: they wait. On the
timeline you will see them as grey spans Ã Â¢Ã¢â  Â¬ parked at the door until the key
comes back.

## See it move

Press **Run demo**. Three workers want into the same locked section. Watch them
take turns: one `lock-acquire`, a work span, a `lock-release` Ã Â¢Ã¢â  Â¬ while the other
two sit in grey wait spans. Inside the section, nobody ever overlaps.

## Watch out

- You might lock on `this` or a public field. Then anyone can take your key Ã Â¢Ã¢â  Â¬
  and hold it hostage. Private readonly key, always.
- You might give each thread its OWN key object. Then nobody ever waits, and
  the lock quietly does nothing.
- You might put EVERYTHING inside the lock "to be safe". It is safe Ã Â¢Ã¢â  Â¬ and slow:
  your threads are back in a queue. Lock only the shared-data lines.

## Key takeaways

- `lock (key) { ... }` = one thread inside at a time; the rest WAIT.
- The key is just an object: `private static readonly object _gate = new object();`
- Waiting threads are not broken Ã Â¢Ã¢â  Â¬ waiting is the lock doing its job.
- Never lock on `this`, a string, or anything public.
- Keep the locked section as small as possible.