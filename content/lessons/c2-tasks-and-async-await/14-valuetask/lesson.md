---
id: c2-l14-valuetask
category: c2-tasks-and-async-await
order: 14
title: "ValueTask  -  Avoiding Allocations on Hot Paths"
difficulty: advanced
description: "Optimize hot paths with ValueTask: avoid heap allocations when the result is often already available synchronously."
explainer: async-state-machine
interview:
  - q: "When should you use ValueTask instead of Task?"
    a: "When the result is often available synchronously (e.g., from a cache or a completed operation), ValueTask avoids heap-allocating a Task object every call. But ValueTask can only be awaited once  -  it is a struct that may be consumed, and re-awaiting it throws. Prefer Task unless profiling shows allocation pressure and you have measured that ValueTask helps."
  - q: "What is ValueTask<T>.Preserve()?"
    a: "It wraps the ValueTask in a new Task, allowing multiple awaits. If you need to pass a ValueTask to code that may await it multiple times (like Task.WhenAll), call .Preserve() first. This is the escape hatch when the ValueTask rules are too restrictive."
---

## What is it?

`Task` and `Task<T>` are reference types Ã¢â‚¬â€ every `await` that doesn't return synchronously allocates a Task on the heap. For millions of calls per second, that GC pressure adds up.

`ValueTask<T>` is a **struct** wrapper that can represent three states: a completed result (zero heap allocation), a real `Task<T>`, or a boxed `IValueTaskSource<T>` Ã¢â‚¬â€ an advanced low-level token. The first case is the key one: when a method has a cached result and returns immediately, `new ValueTask<T>(result)` costs nothing.

## The real-world picture

A pizza place that prepackages popular slices. If you order pepperoni (cached), the cashier hands you a slice from the warmer Ã¢â‚¬â€ no box (heap allocation) needed. If you order anchovy-pineapple (fresh), they make it from scratch and put it in a box (real Task).

## How it works in C#

```csharp
// High-frequency API Ã¢â‚¬â€ result is cached 90% of the time.
public ValueTask<int> GetConfigValueAsync(string key)
{
    if (_cache.TryGetValue(key, out var cached))
        return new ValueTask<int>(cached); // zero allocation

    return new ValueTask<int>(FetchFromBackendAsync(key));
}

// Consumer Ã¢â‚¬â€ await ONCE.
int value = await GetConfigValueAsync("max-retries");
```

The rules:
- `ValueTask` can only be **awaited once**. Re-awaiting throws `InvalidOperationException`.
- Use `v.Preserve()` to escape Ã¢â‚¬â€ wraps the value in a `Task` that can be awaited multiple times.
- Never `.Result` or `.Wait()` a `ValueTask` Ã¢â‚¬â€ it may be already consumed.

## Watch out

> **Don't use ValueTask unless you measured.** The rules (one await, no WhenAll, must check AsTask) make it error-prone. The BCL uses it internally (Socket, Stream) because their synchronous fast paths are extremely hot.

> **You cannot pass a ValueTask to Task.WhenAll directly.** Convert to a Task first if you need composition.

## Key takeaways

- `ValueTask<T>` Ã¢â€ â€™ struct, zero-allocation for synchronous results.
- Await exactly once; use `.Preserve()` to convert to a reusable Task.
- Prefer `Task<T>` unless allocation profiling says otherwise.
