---
id: c2-l09-whenany
category: c2-tasks-and-async-await
order: 9
title: "Task.WhenAny  -  First Past the Post"
difficulty: intermediate
description: "Race tasks against each other with Task.WhenAny: respond to whichever finishes first, then cancel or ignore the rest."
visualization: async-activity
explainer: async-state-machine
interview:
  - q: "What is Task.WhenAny and when would you use it?"
    a: "Task.WhenAny takes multiple tasks and completes as soon as ANY one finishes  -  it returns the winner plus a Task you can await to see which one won. Common uses: timeout patterns (race your real task against Task.Delay), responding to the fastest of several data sources, or cancelling remaining work once the first finishes."
  - q: "What do you do with the tasks that DID NOT win WhenAny?"
    a: "The loser tasks keep running unless you cancel them. A good pattern is: pass a CancellationToken to all contestants, cancel the source after the winner finishes, then wait for the losers to observe the cancellation (or catch OperationCanceledException). Never just abandon a running task  -  it holds resources."
---

## What is it?

Up until now we have used `Task.WhenAll` Ã¢â‚¬â€ wait for EVERY task to finish. `Task.WhenAny` does the opposite: it completes as soon as the FIRST task finishes (success or fault). You get back the completed task, and the others keep running (unless you stop them).

The classic use is a **timeout**: race your real work against `Task.Delay(timeout)` Ã¢â‚¬â€ whichever finishes first decides the path.

## The real-world picture

You phone three friends asking for a ride. The first one who says "I'm coming" Ã¢â‚¬â€ you stop the other calls. `WhenAny` is exactly that: you fire off all the options, react to the first response, and cancel the rest.

## How it works in C#

```csharp
var dataTask = FetchFromSlowApiAsync(token);
var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5), token);

var winner = await Task.WhenAny(dataTask, timeoutTask);

if (winner == timeoutTask)
{
    // Timeout Ã¢â‚¬â€ the slow API never answered.
    Console.WriteLine("Timed out after 5 seconds.");
}
else
{
    // dataTask finished first Ã¢â‚¬â€ unwrap the result.
    var data = await dataTask;
    Console.WriteLine($"Got data: {data}");
}
```

## See it move

Press **Run demo** Ã¢â‚¬â€ three faked network calls finish at 300ms, 600ms and 900ms. `WhenAny` returns after 300ms (the fastest). The timeline shows the other two cancelled gracefully right after.

## Watch out

> **WhenAny doesn't cancel the losers.** You must cancel them yourself or they will run to completion wasting CPU. Always pass a shared CancellationToken.

> **WhenAny + fire-and-forget = resource leak.** If you don't store the returned task, the work keeps running with no observer Ã¢â‚¬â€ exceptions go unobserved until the finalizer (or the GC) complains.

## Key takeaways

- `Task.WhenAny` Ã¢â€ â€™ completes when the FIRST task finishes.
- Classic use: timeout = race against `Task.Delay`.
- Always cancel the losers; always dispose the `CancellationTokenSource`.
