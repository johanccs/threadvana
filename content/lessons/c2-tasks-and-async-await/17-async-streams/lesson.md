---
id: c2-l17-async-streams
category: c2-tasks-and-async-await
order: 17
title: "Async Streams Ã Â¢Ã¢â  Â¬ IAsyncEnumerable and await foreach"
difficulty: advanced
description: "Stream results as they arrive with IAsyncEnumerable and await foreach: process items one at a time as each becomes available."
visualization: async-activity
explainer: async-state-machine
interview:
  - q: "What is IAsyncEnumerable<T> and how is it different from IEnumerable<T>?"
    a: "IAsyncEnumerable lets you stream results one at a time ASYNCHRONOUSLY Ã Â¢Ã¢â  Â¬ each element may involve an await. IEnumerable pulls all items synchronously into memory. The consumer uses await foreach to iterate. It is perfect for paging APIs, reading large files line-by-line, or streaming database results without buffering everything."
  - q: "How do you produce an IAsyncEnumerable?"
    a: "With an async iterator method that uses yield return inside an async IAsyncEnumerable<T> method body. You can await between yields: yield return await FetchNextAsync(). The compiler builds the state machine, same as for async Task methods but with MoveNextAsync() instead of GetAwaiter()."
---

## What is it?

You know `foreach` iterates an `IEnumerable` Ã Â¢Ã¢â  Â¬ each step is synchronous. `await foreach` iterates an `IAsyncEnumerable<T>` Ã Â¢Ã¢â  Â¬ each step may `await` some I/O before the next item arrives. This means you can stream items AS they arrive, without waiting for the whole batch to complete.

Under the hood, `await foreach` calls `MoveNextAsync()` instead of `MoveNext()`, yielding control between items so the consumer never blocks.

## The real-world picture

A sushi conveyor belt. `IEnumerable<T>` is the chef placing all 40 plates on a tray and handing it to you at once. `IAsyncEnumerable<T>` is the belt slowly delivering one plate at a time Ã Â¢Ã¢â  Â¬ you eat as they arrive, and the chef replenishes as needed.

## How it works in C#

```csharp
// Producer Ã Â¢Ã¢â  Â¬ returns items one at a time with await between each.
public static async IAsyncEnumerable<int> ReadSensorDataAsync(
    [EnumeratorCancellation] CancellationToken token = default)
{
    for (var i = 0; i < 10; i++)
    {
        await Task.Delay(200, token);  // await inside the iterator!
        yield return Random.Shared.Next(100);
    }
}

// Consumer Ã Â¢Ã¢â  Â¬ await foreach
await foreach (var reading in ReadSensorDataAsync())
{
    Console.WriteLine($"Sensor: {reading}");
}
```

## Watch out

> **Always accept a CancellationToken and decorate it with [EnumeratorCancellation].** Without the attribute, the compiler generates a state machine that doesn't wire cancellation properly.

> **IAsyncEnumerable must be consumed with await foreach.** Using plain `foreach` gives a compile error. Linq's `.ToEnumerable()` can bridge but loses the async benefit.

## Key takeaways

- `IAsyncEnumerable<T>` Ã Â¢Ã¢â ¬Â  stream items with `await` between each.
- Producer: `async IAsyncEnumerable<T>` + `yield return`.
- Consumer: `await foreach`.
- Always add `[EnumeratorCancellation] CancellationToken` to the iterator.
