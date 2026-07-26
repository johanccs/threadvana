---
id: c2-l08-cancellation-token
category: c2-tasks-and-async-await
order: 8
title: CancellationToken Ã¢â‚¬â€ Cancelling Politely
difficulty: intermediate
description: "Learn cooperative cancellation in async code with CancellationToken: cancel running tasks gracefully."
visualization: async-activity
explainer: cancellation
interview:
  - q: What is the difference between CancellationToken and CancellationTokenSource?
    a: CancellationTokenSource is the "trigger" Ã¢â‚¬â€ you call .Cancel() on it. CancellationToken is the "listener" Ã¢â‚¬â€ you poll .IsCancellationRequested or pass it to async methods. You create the source, hand out its Token, and cancel the source when you want the work to stop. Dispose the source to free its timer resources.
  - q: Where should CancellationToken appear in a method signature?
    a: It should be the LAST parameter, and optional with a default of default(CancellationToken). This is the convention across the BCL. If your method is async and does any real work (calls other async methods, loops, I/O), it should accept and honour one.
---

## What is it?

A `CancellationToken` is a polite tap on the shoulder Ã¢â‚¬â€ it says "please stop what you are doing." It does NOT kill threads; it is a cooperative flag that methods check. If you ignore it, nothing happens (unlike `Thread.Abort`, which is deprecated for good reason).

Every async BCL method that waits accepts a `CancellationToken`: `Task.Delay(ms, token)`, `SemaphoreSlim.WaitAsync(token)`, `HttpClient.GetAsync(url, token)`. When the token is signalled, those methods throw `OperationCanceledException` immediately instead of waiting.

## The real-world picture

A chef has a buzzer that the manager can press to say "stop this order." The chef finishes the current sautÃƒÂ© toss (they don't drop the pan mid-air), then stops. A `CancellationToken` is that buzzer Ã¢â‚¬â€ cooperative, not violent.

## How it works in C#

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
CancellationToken token = cts.Token;

try
{
    await Task.Delay(10_000, token); // would wait 10 s, but...
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled after 5 seconds.");
}
```

Three patterns to honour a token:

```csharp
// 1. Pass it through to the inner call
await SomeAsync(token);

// 2. Check manually in a loop
token.ThrowIfCancellationRequested();

// 3. Register a callback (rare)
token.Register(() => Cleanup());
```

## See it move

Press **Run demo** Ã¢â‚¬â€ we start a long operation with a 3-second CancellationToken. The timeline shows the work starting, the token being signalled at 3s, and the task catching the exception and finishing clean. No thread is aborted Ã¢â‚¬â€ the task simply stops when it checks the token.

## Watch out

> **Always dispose CancellationTokenSource.** The linked timer inside it can leak if you don't. Dispose it in a `finally` block (or use `using`).

> **Pass the token to EVERY async BCL call in the chain.** One call that ignores the token keeps the whole operation alive.

> **Don't catch OperationCanceledException unless you mean to.** If you swallow it, the caller won't know the operation was cancelled Ã¢â‚¬â€ they will think it finished successfully.

> **Cancelled tasks show as 'Canceled', not 'Faulted'.** `task.IsCanceled` returns true; `task.Exception` is null.

## Key takeaways

- `CancellationToken` = cooperative stop signal. Not a kill switch.
- `CancellationTokenSource` creates the token and triggers it.
- BCL methods throw `OperationCanceledException` when the token is signalled Ã¢â‚¬â€ catch it to perform cleanup, then re-throw.
- Always pass the token as the last parameter, and dispose the source.
