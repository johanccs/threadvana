---
id: c1-l12-exceptions-on-threads
category: c1-threading-foundations
order: 12
title: Exceptions Don't Cross Threads
difficulty: beginner
description: "Handle exceptions that escape threads: they do not propagate the way you might expect and can crash your application silently."
visualization: thread-timeline
explainer: thread-basics
interview:
  - q: What happens if an exception is thrown on a worker thread and nobody catches it?
    a: The worker thread dies and, in modern .NET, the whole process usually ends. Exceptions do not magically appear on the main thread  -  you must catch them inside the thread's own code.
  - q: How do you safely handle errors on background work?
    a: Wrap the thread body in try/catch and either store the error and signal the main thread, or (better) use Task with async/await where exceptions automatically travel back through `await`.
---

## What is it?

When a worker thread throws an exception, **the main thread does not see it**. Each
thread has its own call stack. An error on Thread A does not interrupt Thread B Ã¢â‚¬â€
unless you design a way to pass it over.

If nobody catches the exception, the worker dies. In older .NET the rest of the
program kept running (which was worse Ã¢â‚¬â€ it just stopped working). In modern .NET
the whole process shuts down.

## The real-world picture

Two cooks are prepping vegetables. One cuts herself and leaves the kitchen. The
other cook keeps chopping Ã¢â‚¬â€ she doesn't automatically know about the accident.
Unless someone goes to tell her, she'll just wonder why the station is empty.

In code: the main cook (thread) needs a way to find out without looking over the
other cook's shoulder the whole time Ã¢â‚¬â€ that's try/catch inside the thread.

## How it works in C#

```csharp
new Thread(() =>
{
    try
    {
        DoSomethingRisky();
    }
    catch (Exception ex)
    {
        // The error stays on THIS thread Ã¢â‚¬â€ handle it here.
        Console.WriteLine($"Worker failed: {ex.Message}");
    }
}).Start();

Console.WriteLine("Main thread keeps going.");
```

Later (Category 2) you will learn a much cleaner way: async/await. With Tasks, the
exception travels back to whoever called `await` Ã¢â‚¬â€ no try/catch in the background
thread needed!

## See it move

Press **Run demo**. A background task throws Ã¢â‚¬â€ watch it die in its swimlane. The
main lane keeps going. Then watch the fixed version catch and report it.

## Watch out

- You might expect the main thread to crash when a worker throws. It doesn't Ã¢â‚¬â€ but
  the whole program might still end depending on your .NET version.
- You might wrap ONLY the risky call in try/catch. Wrap the entire thread body so
  unexpected errors at the edges are caught too.
- You might forget to SIGNAL the main thread. Catching is step one; saving the error
  and telling the main thread is step two.

## Key takeaways

- Exceptions on one thread do not automatically appear on another.
- Always catch exceptions inside the thread that might throw them.
- An unhandled exception on any thread can kill the whole process in modern .NET.
- For future lessons: Tasks make error handling much easier.
