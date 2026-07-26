---
id: c1-l03-passing-data-to-threads
category: c1-threading-foundations
order: 3
title: Giving Your Thread Some Data
difficulty: beginner
description: "Learn how to send data into a thread and get results back, safely and correctly using ParameterizedThreadStart and closures."
visualization: thread-timeline
explainer: thread-basics
interview:
  - q: How do you pass data to a thread?
    a: The easiest way is a lambda - the thread's code can simply use the variables around it. This is called capturing. For example, new Thread(() => Process(fileName)) hands fileName to the new worker.
  - q: Why do all my threads print the same number when I start them in a loop?
    a: They captured the loop variable itself, not its value at that moment. By the time they run, the loop has moved on or finished, so they all see the same final value. The fix is to copy the variable into a fresh local inside the loop and capture the copy.
  - q: What is a closure?
    a: A closure is a lambda plus the variables it captured from around it. C# captures variables, not values - which is exactly why the loop-variable trap exists, and why copying into a fresh local per loop round fixes it.
---

## What is it?

A thread usually needs some data to work on - a name, a number, a file path.
When you write the thread's code as a **lambda**, it can simply use the
variables around it. This is called **capturing**, and the bundle of a lambda
plus its captured variables is called a **closure**.

Capturing is convenient - and it is home to the most famous beginner bug in
threading.

## The real-world picture

You hire three helpers and each one gets a number: 0, 1, 2. The right way is to
write the number on a paper ticket and hand each helper their own ticket.

The wrong way is to point at a whiteboard and say "your number is whatever is
written there." Then you erase 0, write 1, erase 1, write 2... When the helpers
finally look, they ALL see 2. Three helpers, same number, total confusion.

A captured variable is that whiteboard. A copied local variable is the paper
ticket.

## How it works in C#

```csharp
using System;
using System.Threading;

// THE TRAP - all three threads usually print 3!
for (int i = 0; i < 3; i++)
{
    var t = new Thread(() => Console.WriteLine(i)); // captures the VARIABLE i
    t.Start();
}

// THE FIX - each thread gets its own copy.
for (int i = 0; i < 3; i++)
{
    int mine = i;  // a brand-new local, created fresh each loop round
    var t = new Thread(() => Console.WriteLine(mine)); // captures the copy
    t.Start();
}
```

The difference is one line: `int mine = i;` **inside** the loop. Each round
creates a new `mine`, so each thread captures a different variable and keeps
its own number forever.

(Threads like these should also be Joined - the demo does that part properly.)

## See it move

Press **Run demo**. First, three trap workers: look at their lanes - every one
reports the SAME number, because they all read the shared loop variable after
the loop had already finished. Then three fixed workers: each lane shows its
own number (0, 1, 2), because each one captured a private copy.

## Watch out

- You might think `() => Console.WriteLine(i)` "freezes" the value of `i`. It
  does not. C# captures the variable, not the value.
- You might think the bug is rare because it sometimes prints the right
  numbers. It is a race - it can work on your machine and fail on your
  teammate's.
- You might fix it by renaming the loop variable. Renaming changes nothing.
  You need a NEW variable per loop round: `int mine = i;`.

## Key takeaways

- Lambdas hand data to threads by capturing the variables around them.
- C# captures variables, not values.
- The loop-variable trap: every thread ends up reading the same final value.
- The fix is one line inside the loop: `int mine = i;` then use `mine`.
- If threads need different data, give each one its own copy.
