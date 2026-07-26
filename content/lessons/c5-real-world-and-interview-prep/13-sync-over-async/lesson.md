---
id: c5-l13-sync-over-async
category: c5-real-world-and-interview-prep
order: 13
title: "Sync-Over-Async in Legacy Code Ã¢â‚¬â€ Escape Strategies"
difficulty: advanced
description: "Understand the dangers of calling async code from synchronous methods, the deadlock risk, and the rare safe patterns."
explainer: deadlock
interview:
  - q: "You must call an async method from a synchronous context. What do you do?"
    a: "The safest path: refactor the call chain to be async all the way up. If that's impossible: (1) Use Task.Run(() => DoAsync()).GetAwaiter().GetResult() to run the async work on a pool thread, avoiding the deadlock-prone direct .Result call. (2) If in ASP.NET Framework (with a SynchronizationContext), GetAwaiter().GetResult() on the same thread still deadlocks Ã¢â‚¬â€ Task.Run is required. (3) In ASP.NET Core (no context), GetAwaiter().GetResult() works but wastes a thread. Always document the hack Ã¢â‚¬â€ it's technical debt."
---

Write `Solution.CallAsyncFromSync()` that calls `Solution.FetchAsync()` (async) from a synchronous method WITHOUT deadlocking. Return the result. Use `Task.Run(() => FetchAsync()).GetAwaiter().GetResult()`.
