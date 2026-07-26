---
id: c1-l01-what-is-a-thread
category: c1-threading-foundations
order: 1
title: What is a Thread?
difficulty: beginner
description: "Your first step: understand what a thread is, how it runs code line by line, and why every C# program starts with one main thread."
visualization: thread-timeline
explainer: thread-basics
interview:
  - q: What is a thread?
    a: A thread is a worker that runs one piece of code at a time. Every C# program starts with one main thread, and you can start extra threads to do work at the same time.
  - q: What is the difference between a process and a thread?
    a: A process is your running program with its own memory. Threads are the workers inside that process Ã¢â‚¬â€ they share the same memory, which is exactly why two threads can accidentally mess with the same data.
---

## What is it?

A **thread** is a worker that does one thing at a time. Every C# program starts with
one worker, called the **main thread**. When you want two things to happen at the
same time, you hire an extra worker: you start a new thread.

Each thread runs your code line by line, top to bottom, on its own. Two threads can
be inside your program *at the same moment* Ã¢â‚¬â€ that is the whole point, and also the
source of every bug you will learn to fix in this course.

## The real-world picture

Think of a coffee shop with one barista. They take your order, then make your drink,
then take the next order. One barista = one thread = one thing at a time.

Now the shop hires a second barista. Two drinks can be made at the same time. But if
both baristas grab the same milk carton at once... you can see the problem coming.
Sharing is powerful and dangerous. That is multithreading in one sentence.

## How it works in C#

```csharp
using System;
using System.Threading;

// 1. Create a worker and tell it WHAT to do (a method).
Thread worker = new Thread(() =>
{
    Console.WriteLine("Hello from the new thread!");
});

// 2. Start it. From here on, BOTH threads run at the same time.
worker.Start();

// 3. Wait for it to finish (otherwise the program may exit first).
worker.Join();
```

Three ideas to remember:
- `new Thread(...)` creates the worker but does **not** start it.
- `Start()` sets it running. The order in which the two threads' lines run is
  **not guaranteed** Ã¢â‚¬â€ run the demo twice and compare!
- `Join()` means "pause my thread until that thread is done".

## See it move

Press **Run demo**. The timeline shows one swimlane per thread. Watch how the main
thread and the worker thread overlap in time Ã¢â‚¬â€ both are running at once. Then look at
the `Join` point: the main lane goes grey (waiting) until the worker lane ends.

## Watch out

- You might think the new thread's lines always print after the main thread's lines.
  They don't. Once you call `Start()`, the operating system decides who runs when.
- You might forget `Join()`. If the main thread reaches the end of the program, your
  worker can be cut off mid-work.
- You might create threads in a loop for lots of small tasks. Threads are expensive;
  later lessons (thread pool!) show the cheaper way.

## Key takeaways

- A thread is a worker that runs code one line at a time.
- Every program starts with one main thread; `new Thread(...)` + `Start()` adds more.
- Threads run **at the same time**, in an order you cannot predict.
- `Join()` waits for a thread to finish.
- Threads share the same memory Ã¢â‚¬â€ great for teamwork, dangerous for shared data.
