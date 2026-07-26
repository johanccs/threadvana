---
id: c1-l11-threadpool-tuning-min-threads
category: c1-threading-foundations
order: 11
title: Tuning the Thread Pool Ã¢â‚¬â€ Min Threads and the Slow Ramp-Up
difficulty: advanced
description: "Learn to set the minimum thread pool size with ThreadPool.SetMinThreads and understand when (and when not) to tune it."
visualization: thread-pool
interview:
  - q: What does SetMinThreads do and when would you use it?
    a: It tells the pool to keep a minimum number of workers ready at all times. You use it when bursts of work arrive all at once and you do not want to wait for the pool's slow ramp-up Ã¢â‚¬â€ one new worker can take about 500 ms to appear.
  - q: Should you ever change the maximum thread count?
    a: Rarely. The default is huge (thousands). Raising it can hide a real problem Ã¢â‚¬â€ if you are running out of threads, more threads usually make things slower, not faster. Fix the bottleneck instead.
---

## What is it?

The thread pool does not create all its workers at once. When many tasks arrive at the
same time, the pool adds workers *slowly* Ã¢â‚¬â€ roughly one every half-second. This is
called the **slow ramp-up**.

If your app handles a sudden burst (a spike of requests), you can **pre-hire workers**
with `ThreadPool.SetMinThreads`. This tells the pool "keep at least N workers warm
and ready Ã¢â‚¬â€ don't wait for the burst to ramp you up."

## The real-world picture

A restaurant with one waiter can handle a steady stream of two tables. But a bus of
tourists just pulled up outside Ã¢â‚¬â€ 40 people need to order NOW. The restaurant
very slowly sends in one extra waiter every 30 seconds while the tourists grumble.

`SetMinThreads` is the manager saying "I knew the bus was coming Ã¢â‚¬â€ I kept 10 waiters
on shift."

## How it works in C#

```csharp
// Before the burst: say "keep at least X workers ready"
ThreadPool.SetMinThreads(workerThreads: 20, completionPortThreads: 20);

// Now the burst of 20 tasks each gets a worker almost instantly.

// After the burst (optional, but polite): reset to defaults
ThreadPool.SetMinThreads(1, 1);
```

Two things never to forget:
- `SetMinThreads` does **not** reserve actual threads Ã¢â‚¬â€ it only means "grow quickly
  UP TO this many". If no tasks arrive, no threads are created.
- Setting the min too high wastes memory. Set it for the burst, then dial it back.

## See it move

Press **Run demo**. The demo queues 12 blocking tasks. Phase 1 runs with default
min threads Ã¢â‚¬â€ watch the queue drain slowly. Phase 2 runs with `SetMinThreads(12,12)`
Ã¢â‚¬â€ the queue empties fast.

## Watch out

- You might think `SetMinThreads` instantly creates threads. It doesn't Ã¢â‚¬â€ it just
  removes the *throttle* on creating them. First task arrivals still trigger creation.
- You might set the min and forget. You should reset it after the burst, or set it
  once at startup for the whole app.
- Setting min threads to hundreds "just in case" wastes memory and makes the pool
  less efficient at recycling.

## Key takeaways

- The thread pool grows slowly (one worker ~every 500 ms).
- `SetMinThreads` removes the throttle Ã¢â‚¬â€ tell it how many workers you will need.
- Use it for bursts; reset afterward (or set once at app startup if you know your load).
- Never touch `SetMaxThreads` unless you are fixing a very specific, measured bottleneck.
