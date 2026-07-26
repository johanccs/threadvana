---
id: c2-l06-returning-values-task-t
category: c2-tasks-and-async-await
order: 6
title: Returning Values ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â Task of T
difficulty: beginner
description: "Return values from async methods with Task<T>: the result arrives when the work is done, not before."
visualization: async-activity
explainer: async-state-machine
interview:
  - q: What is the difference between Task and Task<T>?
    a: Task represents work that finishes with no return value (void-equivalent). Task<T> wraps a return value ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â the T becomes available after the task completes. Both can be awaited, composed with WhenAll/WhenAny, and carry exceptions the same way. Think of Task<T> as a "future" ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â a box that will contain a T once the work is done.
  - q: How do you get the result of a Task<T> without blocking?
    a: await it. var result = await Task.Run(() => Compute()). Calling .Result blocks the calling thread (and can deadlock with sync contexts). await frees the thread while the task runs, then resumes with the value.
---

## What is it?

`Task.Run` returns `Task` when the work has no return value ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â it is a fire-and-forget signal. But most real work produces a result: a computed value, a parsed object, a fetched price. That is when you use `Task<T>`.

The `T` in `Task<T>` is the type of the value the task will produce. When you `await` the task, you get that value directly ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â no casting, no `.Result`.

## The real-world picture

Placing an order at a coffee shop: "Task" is the buzzer that vibrates when your drink is ready. `Task<Latte>` is the same buzzer, but when it vibrates, you walk up and pick up an actual `Latte` ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â not just "something is done."

## How it works in C#

```csharp
Task<int> answerTask = Task.Run(() =>
{
    Thread.Sleep(500); // pretend heavy math
    return 42;         // int returned ÃƒÂ¢Ã¢â‚¬Â Ã¢â‚¬â„¢ Task<int>
});

// ... main thread can do other work here ...

int answer = await answerTask;
Console.WriteLine(answer); // 42
```

Key points:
1. The lambda inside `Task.Run` returns `int` ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â the compiler infers `Task<int>`.
2. `await` unwraps the `Task<int>` into an `int`.
3. If the task threw, `await` re-throws the exception ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â the `T` is never produced.

You can also use `Task.FromResult(value)` to create an already-completed `Task<T>`:

```csharp
Task<int> cached = Task.FromResult(42);
int x = await cached; // immediate
```

## Watch out

> **Never use `.Result` if you can use `await`.** `.Result` blocks the calling thread until the value is ready ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â it is a synchronous wait disguised as an async operation. Same deadlock risks as `.Wait()`.

> **`await task` when task is already completed acts like `.Result` ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â but without blocking.** It returns the cached result synchronously on the same thread, so it is always safe.

## Key takeaways

- `Task<T>` ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â a promise that resolves to a T. Use it when your work returns a value.
- `await task` unwraps the value (or throws if the task faulted).
- `.Result` exists for edge cases (e.g., a synchronous method calling into async code), but it has all the same deadlock risks.
