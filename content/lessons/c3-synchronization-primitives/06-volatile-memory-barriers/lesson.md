---
id: c3-l06-volatile-memory-barriers
category: c3-synchronization-primitives
order: 6
title: "volatile and Memory Barriers Ã Â¢Ã¢â  Â¬ Always Seeing the Latest Value"
difficulty: intermediate
description: "Understand volatile and memory barriers: when the compiler and CPU reorder your reads and writes, and how to stop them."
visualization: thread-timeline
explainer: race-interleaving
interview:
  - q: "What does the volatile keyword do in C#?"
    a: "It tells the compiler and the CPU: never cache this field's value Ã Â¢Ã¢â  Â¬ always read the latest value from main memory, and always write straight through. Without volatile, the compiler may hoist a read out of a loop (register caching), and the CPU may reorder reads/writes around each other. volatile prevents both optimisations for that field. It does NOT make ++ atomic Ã Â¢Ã¢â  Â¬ Interlocked is still needed for atomic operations."
  - q: "Is volatile enough for thread-safe code?"
    a: "Almost never by itself. It ensures visibility Ã Â¢Ã¢â  Â¬ other threads see the latest write Ã Â¢Ã¢â  Â¬ but does not provide atomicity or ordering guarantees for operations involving more than one access. For a simple 'stop' flag (bool _running), volatile is fine. For anything involving two or more fields, use a lock or Interlocked+volatile together. In practice, prefer lock or Volatile.Read/Write which are more explicit."
---

## What is it?

CPUs and compilers lie to you Ã Â¢Ã¢â  Â¬ they reorder reads/writes and cache values in registers to go faster. Most of the time this is invisible and harmless. But in multithreaded code, it means one thread may never see a value another thread just wrote.

`volatile` is the fence that says: "do not optimise this field Ã Â¢Ã¢â  Â¬ read it from memory every single time, and write it through every single time."

## The real-world picture

Two people sharing a whiteboard. Without volatile, each person looks at their own notepad (register cache) and never looks up at the board. The first person erases and rewrites a number Ã Â¢Ã¢â  Â¬ the second person never sees the change because they are still reading their notepad. `volatile` forces everyone to look at the actual whiteboard.

## How it works in C#

```csharp
// WITHOUT volatile Ã Â¢Ã¢â  Â¬ the loop may never exit (compiler hoists _running to a register).
private bool _running = false;
// Thread 1: while (!_running) { } // may loop forever
// Thread 2: _running = true;

// WITH volatile Ã Â¢Ã¢â  Â¬ the read always goes to memory.
private volatile bool _running = false;
// Thread 1: while (!_running) { } // will definitely exit
// Thread 2: _running = true;
```

## See it move

Press **Run demo** Ã Â¢Ã¢â  Â¬ one thread spins on a volatile flag, another sets it after 500ms. Watch the spin counter stop exactly at 500ms. Then watch the same code WITHOUT volatile (simulated) Ã Â¢Ã¢â  Â¬ the spinner never stops.

## Watch out

> **volatile does NOT make `_running = !_running` atomic.** Another thread can read between the read and the write. Use Interlocked or lock for compound operations.

> **volatile in C# applies only to fields of reference types, pointer types, and enum types with an underlying integer, or certain integral types.** You cannot mark a `double` as `volatile` in C# (use `Volatile.Read/Write` instead).

## Key takeaways

- `volatile` Ã Â¢Ã¢â ¬Â  read/write go directly to memory, never cached.
- Good for simple status flags; insufficient for compound operations.
- Prefer `Volatile.Read(ref field)` and `Volatile.Write(ref field, value)` for explicit control.
