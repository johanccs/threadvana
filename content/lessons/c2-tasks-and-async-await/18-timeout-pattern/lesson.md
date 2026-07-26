---
id: c2-l18-timeout-pattern
category: c2-tasks-and-async-await
order: 18
title: "Timeout Patterns Ã¢â‚¬â€ WhenAny + CancellationToken Powers Combined"
difficulty: advanced
description: "Implement timeout patterns: give every async operation a deadline using CancellationTokenSource.CancelAfter."
visualization: async-activity
explainer: cancellation
interview:
  - q: "How do you apply a timeout to an async operation?"
    a: "The cleanest pattern: combine a CancellationToken timeout with Task.WhenAny. Create a CancellationTokenSource with a timeout, pass its Token to the async call, then race the call against a Task.Delay. If the delay wins, cancel the source. The async call must honour the token for this to work Ã¢â‚¬â€ if it ignores cancellation, you still have a running task with no observer. Always try/catch OperationCanceledException."
  - q: "What is the difference between a timeout via WhenAny+Delay and a CancellationToken timeout?"
    a: "A bare WhenAny+Delay race leaves the original task running (it may finish later with no observer Ã¢â‚¬â€ a fire-and-forget leak). A CancellationToken timeout signals the task to STOP, which is the right approach. Use both: WhenAny for the timeout detection + CancellationToken to actually stop the work."
---

## What is it?

A timeout is just a race between your real work and a deadline Ã¢â‚¬â€ exactly what `Task.WhenAny` excels at. But a bare race leaves the loser running. The industry-standard pattern layers a `CancellationToken` timeout INTO the work: the race detects the timeout, and the token actually STOPS the work.

## How it works in C#

```csharp
public async Task<T> WithTimeoutAsync<T>(Task<T> work, TimeSpan timeout)
{
    using var cts = new CancellationTokenSource(timeout);
    var delay = Task.Delay(timeout);
    var winner = await Task.WhenAny(work, delay);
    if (winner == delay)
    {
        cts.Cancel(); // signal the work to stop
        throw new TimeoutException($"Operation timed out after {timeout}");
    }
    return await work; // unwrap the result (work already finished)
}
```

## See it move

Press **Run demo** Ã¢â‚¬â€ a slow task runs with a 2-second timeout. At 2s, the delay wins, the CancellationToken fires, and the task catches `OperationCanceledException` and exits clean. No orphaned work.

## Watch out

> **Always dispose the CancellationTokenSource.** The internal timer lives until disposed.

> **The real work MUST observe the cancellation token.** If the task ignores `token.ThrowIfCancellationRequested()`, it will keep running after the timeout Ã¢â‚¬â€ a resource leak.

## Key takeaways

- Timeout = `Task.WhenAny` + `CancellationTokenSource(timeout)` + honouring the token.
- Never leave the loser running Ã¢â‚¬â€ cancel it.
- Always dispose the `CancellationTokenSource`.
