---
id: c3-l19-asynclocal
category: c3-synchronization-primitives
order: 19
title: "AsyncLocal<T> Ã¢â‚¬â€ Data That Flows Across Awaits"
difficulty: advanced
description: "Discover AsyncLocal<T>: thread-local-like storage that flows across async/await continuations, unlike [ThreadStatic]."
explainer: thread-local
interview:
  - q: "What is AsyncLocal<T> and how is it different from ThreadLocal<T>?"
    a: "AsyncLocal<T> flows with the logical call context, NOT with the physical thread. When an async method awaits and resumes on a different pool thread, the AsyncLocal value follows the logical operation Ã¢â‚¬â€ like a request-scoped variable. ThreadLocal is pinned to the thread; AsyncLocal follows the async flow. This is how ASP.NET Core's HttpContext flows across async/await barriers."
  - q: "How does AsyncLocal propagate across thread changes?"
    a: "It uses ExecutionContext Ã¢â‚¬â€ a snapshot of ambient data that is captured at each async point, stored on the Task, and restored when the continuation runs. Every AsyncLocal<T> lives inside the ExecutionContext. When you await, the context is copied, carried, and restored transparently."
---

## What is it?

`AsyncLocal<T>` is ambient data that rides with your async operation across `await` boundaries. Set it once at the top of a request pipeline, and every async method down the chain can read it Ã¢â‚¬â€ even if they resume on different threads. It's the plumbing underneath `HttpContext`, `Activity.Current` (distributed tracing), and `ILogger.BeginScope`.

## See it move

Press **Run demo** Ã¢â‚¬â€ set an AsyncLocal, await Task.Yield (forces a thread change), then read the value. It is still there.

## Key takeaways

- `AsyncLocal<T>` follows the logical call tree, not the physical thread.
- Backed by `ExecutionContext` Ã¢â‚¬â€ copied at each `await` point.
- Use for request-scoped ambient data, logging scopes, tracing context.
