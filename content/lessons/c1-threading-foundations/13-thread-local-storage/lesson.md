---
id: c1-l13-thread-local-storage
category: c1-threading-foundations
order: 13
title: Thread-Local Storage  -  Every Thread Gets Its Own Copy
difficulty: intermediate
description: "Discover ThreadLocal<T> and AsyncLocal<T>: give each thread its own private copy of data so they never step on each other."
visualization: thread-timeline
explainer: thread-local
interview:
  - q: What does the [ThreadStatic] attribute do?
    a: It turns a static field into thread-local storage. Every thread gets its own separate copy of the value. One thread's write does not affect another thread's read.
  - q: What is the difference between [ThreadStatic] and ThreadLocal&lt;T&gt;?
    a: "The [ThreadStatic] attribute is simpler but cannot set an automatic default  -  every thread starts at null/0. ThreadLocal<T> lets you provide a factory so every thread begins with a proper value."
---

## What is it?

Normally a `static` field in C# is **shared** — every thread sees the same value.
With `[ThreadStatic]` you turn it into **thread-local**: each thread gets its own
separate copy of the field.

Think of it like every worker having their own clipboard. Worker A writes on their
clipboard — Worker B cannot read it, and vice versa.

## The real-world picture

A restaurant has one whiteboard for all waiters (static field). Everyone writes
their own table number on it — chaos!

Each waiter now carries their own notepad ([ThreadStatic] field). They write their
tables without conflict, because nobody else reads their notepad.

## How it works in C#

```csharp
[ThreadStatic]
private static int _count; // every thread sees its own copy

new Thread(() =>
{
    _count++;
    Console.WriteLine(_count); // prints 1
}).Start();

new Thread(() =>
{
    _count++;
    Console.WriteLine(_count); // prints 1 (not 2 — different copy!)
}).Start();
```

The newer `ThreadLocal<int>` class handles initialisation cleanly:

```csharp
private static ThreadLocal<int> _count = new(() => 0);
// Now every thread starts with 0 automatically.
```

## See it move

Press **Run demo**. Two threads each increment their own copy. The timeline shows
each lane with its private counter.

## Watch out

- With `[ThreadStatic]`, the inline initialiser `private static int _x = 5` only
  sets the value for the FIRST thread that touches it. All other threads get 0.
  Use `ThreadLocal<T>` if you need a default value everywhere.
- `[ThreadStatic]` does not work inside `async` methods. For async code, use
  `AsyncLocal<T>` instead (cover this in Category 2).

## Key takeaways

- `[ThreadStatic]` gives each thread its own private copy of a field.
- It is perfect for cached objects or per-thread counters — no locks needed.
- `ThreadLocal<T>` is the modern, initial-value-friendly alternative.
- Neither `[ThreadStatic]` nor `ThreadLocal` works with async/await — see Category 2.
