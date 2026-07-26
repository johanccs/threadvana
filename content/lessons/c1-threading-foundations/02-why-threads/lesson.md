---
id: c1-l02-why-threads
category: c1-threading-foundations
order: 2
title: Why Threads at All?
difficulty: beginner
description: "Discover why threads matter: doing multiple things at the same time instead of waiting for each task to finish one after another."
visualization: thread-timeline
explainer: thread-basics
interview:
  - q: Why would you use threads instead of doing work one piece at a time?
    a: Because two pieces of work can finish faster when they run at the same time. Two 400ms jobs take about 800ms one after another, but only about 400ms on two threads. Threads let you trade extra workers for less total waiting.
  - q: Does adding more threads always make a program faster?
    a: No. Each thread costs memory and setup time, and work that needs the CPU is limited by how many cores the machine has. Threads help most when pieces of work can genuinely happen side by side.
  - q: What is the difference between parallel and async?
    a: Parallel means several workers active at the same time. Async means one worker who does not stand idle while waiting - they go do something else and come back when the wait is over. Category 2 of this course is all about async.
---

## What is it?

Running code one piece after another is called **sequential**. Running two
pieces at the same time is called **parallel**. Threads are the tool that takes
you from sequential to parallel: one thread per piece of work, all active at
once.

## The real-world picture

Dinner orders take 30 minutes each. One cook handles two orders in 60 minutes:
finish the first, then start the second. Two cooks handle them in 30 minutes,
because both dishes cook at the same time.

Same kitchen, same recipes, same work. The only difference is how many cooks
are working at once. That is exactly what a second thread buys your program.

## How it works in C#

```csharp
using System;
using System.Threading;

// Pretend jobs - each one takes 400ms.
static void JobA() { Thread.Sleep(400); }
static void JobB() { Thread.Sleep(400); }

// SEQUENTIAL: one after another - about 800ms in total.
JobA();
JobB();

// PARALLEL: at the same time - about 400ms in total.
var t1 = new Thread(JobA);  // hand JobA to worker 1
var t2 = new Thread(JobB);  // hand JobB to worker 2
t1.Start();
t2.Start();   // BOTH are now running at the same time
t1.Join();
t2.Join();    // wait until both are done
```

Notice the shape: **Start both first, then Join both.** If you Join the first
thread before Starting the second, you are back to sequential - worker 2 only
begins after worker 1 has completely finished.

## See it move

Press **Run demo**. First the main thread does both jobs by itself - watch its
swimlane fill up with two work blocks, one after the other (~800ms). Then two
fresh workers take one job each - their swimlanes run **side by side**, and the
pair finishes in about half the time. Same work, shorter timeline.

## Watch out

- You might think more threads always means more speed. It does not. Threads
  cost memory and setup time, and CPU-hungry work is limited by the cores in
  the machine.
- You might Start and Join one thread at a time. That is sequential with extra
  steps! Start everything first, then Join everything.
- You might think you need a new thread every time your program waits (for a
  file, a website, a database). Often you do not - **async** lets one thread
  avoid standing idle without hiring another worker. Category 2 teaches that.

## Key takeaways

- Sequential = one after another. Parallel = at the same time.
- Two threads can finish two jobs in roughly the time of the slower job.
- Start all your threads first, then Join them all.
- More threads is not automatically faster - every thread has a real cost.
- For waiting-heavy work, async (Category 2) is often the better tool.
