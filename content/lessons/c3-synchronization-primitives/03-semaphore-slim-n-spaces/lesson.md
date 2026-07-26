---
id: c3-l03-semaphore-slim-n-spaces
category: c3-synchronization-primitives
order: 3
title: SemaphoreSlim Ã¢â‚¬â€ the Parking Lot with N Spaces
difficulty: intermediate
description: "Learn SemaphoreSlim: an async-friendly gate that lets N threads in at once. Think of it as a parking lot with N spaces."
visualization: semaphore
interview:
  - q: What is a semaphore?
    a: A semaphore limits how many threads can enter a section of code at the same time - like a parking lot with N spaces. Threads take a permit to enter and release it when they leave; when all permits are taken, the rest queue. Bonus point - with one permit it acts like a lock, with more it throttles concurrency, like limiting calls to an API.
  - q: What is the difference between Semaphore and SemaphoreSlim?
    a: SemaphoreSlim is the modern, lighter version and the only one with WaitAsync, so it fits async code without blocking a thread. Semaphore is the older, heavier Windows-kernel one you only need for named, cross-process scenarios. Bonus point - the rule of thumb is to default to SemaphoreSlim unless you specifically need cross-process synchronization.
  - q: Why must Release go in a finally block?
    a: If the code throws before Release, that permit is lost forever - one less thread can ever enter, and eventually everyone queues forever. finally guarantees the permit is returned even when the work fails. Bonus point - it is the same rule as the bathroom key in lock - the key must come back no matter how the visit ended.
---

## What is it?

A **SemaphoreSlim** is a bouncer that lets AT MOST N threads into a section of
code at once. You create it with N permits: `new SemaphoreSlim(2)`.
`WaitAsync()` takes a permit (or waits for one to free up); `Release()` hands
it back.

In *lock Ã¢â‚¬â€ One Key to the Bathroom* the key had exactly one copy. A semaphore
is the same idea with N copies of the key.

## The real-world picture

A parking lot with N spaces. Cars drive in while there is a free space; when
the lot is full, the rest queue at the entrance. One car leaves, one waiting
car drives in. The lot does not care WHO parks Ã¢â‚¬â€ only HOW MANY.

## How it works in C#

```csharp
// The parking lot: AT MOST 2 callers inside at once.
private static readonly SemaphoreSlim _lot = new SemaphoreSlim(2);

public static async Task CallApiAsync()
{
    await _lot.WaitAsync();   // drive in - or queue at the entrance
    try
    {
        // Limited work: at most 2 threads are EVER here at the same moment.
        await Task.Delay(200); // pretend: the actual slow call
    }
    finally
    {
        _lot.Release();        // ALWAYS drive out - even if something crashed
    }
}
```

The `try/finally` is not decoration: if the work throws and `Release()` is
skipped, a space is lost FOREVER Ã¢â‚¬â€ until nobody gets in.

`SemaphoreSlim` vs `Semaphore`, in one line: Slim is the modern, lighter,
async-friendly one Ã¢â‚¬â€ reach for it by default.

## See it move

Press **Run demo**. Five cars, two spaces. Watch two `semaphore-enter` spans
light up while the other three cars sit grey (queued). Every `semaphore-exit`
instantly lets one waiting car in. At every single moment: at most 2 inside.

## Watch out

- You might forget `Release()`, or lose it when an exception flies. One missing
  Release = one less space forever. Always `try/finally`.
- You might write `new SemaphoreSlim(1)`. That is a lock with extra steps Ã¢â‚¬â€ if
  you want exactly one thread inside, use `lock`.
- You might call the blocking `.Wait()` inside async code. `WaitAsync()` queues
  without parking a thread.

## Key takeaways

- `SemaphoreSlim(n)` = at most n threads inside at once; the rest queue.
- `await WaitAsync()` drives in; `Release()` drives out.
- Always Release in `finally` Ã¢â‚¬â€ a lost permit never comes back.
- n = 1 is a lock; n > 1 is the semaphore's superpower.
- Prefer SemaphoreSlim over Semaphore: lighter and async-friendly.