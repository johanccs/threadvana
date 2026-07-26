---
id: c4-l17-bounded-channels
category: c4-concurrent-collections-and-parallelism
order: 17
title: "Bounded Channels - Backpressure in Action"
difficulty: advanced
description: "Master bounded channels: the async-native producer-consumer pipe that automatically slows down writers when the buffer is full. No semaphores, no polling, no blocking - just natural backpressure."
explainer: channel
interview:
  - q: "What is a bounded Channel and how does it enforce backpressure?"
    a: "A bounded Channel is created with a capacity: Channel.CreateBounded<T>(N). When the writer calls WriteAsync() and the channel is full, the writer asynchronously waits until a reader frees a slot. This is the modern replacement for BlockingCollection in async code."
  - q: "How is a bounded Channel different from a BlockingCollection?"
    a: "Channels are fully async - WriteAsync/ReadAsync yield the thread, no blocking. BlockingCollection's Add/Take block the calling thread. Channels also separate reader and writer APIs so you can hand each half to different components."
---

## What is it?

A `Channel<T>` is an async-native producer-consumer pipe. A **bounded** channel adds a capacity cap: when the pipe is full, `WriteAsync` automatically awaits until a reader makes room. When empty, `ReadAsync` awaits until a writer adds an item. This is **backpressure** - the channel naturally slows down the fast side so the slow side can keep up, with zero blocking and zero polling.

## The real-world picture

Think of a restaurant kitchen with a pass-through window that holds exactly 5 plates. The chefs (producers) cook and place plates on the window. The servers (consumers) pick up plates and deliver them. If the window has 5 plates already, the chef cannot place another - they wait until a server grabs one. If the window is empty, the server waits for the chef. No one shouts, no one polls "is there room yet?" The window's size IS the backpressure mechanism.

## How it works in C#

```csharp
using System.Threading.Channels;

var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(3)
    { FullMode = BoundedChannelFullMode.Wait });

// Producer writes 10 items but channel only holds 3
var producer = Task.Run(async () => {
    for (int i = 0; i < 10; i++) {
        await channel.Writer.WriteAsync(i);
        Console.WriteLine($"Produced: {i}");
    }
    channel.Writer.Complete();
});

// Consumer reads slower than producer writes
var consumer = Task.Run(async () => {
    await foreach (var item in channel.Reader.ReadAllAsync()) {
        await Task.Delay(500);
        Console.WriteLine($"Consumed: {item}");
    }
});

await Task.WhenAll(producer, consumer);
```

Watch: the producer races ahead to fill the 3 slots, then *pauses* (WriteAsync awaits) until the consumer frees a slot. This is backpressure in action.

## See it move

Press **Run demo**. The channel visualization shows items flowing through a fixed-capacity pipe. Watch the producer lane slow down when the pipe fills up, then speed up as the consumer drains items. The producer and consumer naturally synchronize through the channel's capacity.

## Watch out

> **Forgetting Complete() hangs the reader forever.** If the writer never calls `writer.Complete()`, `ReadAllAsync()` loops forever. Always complete the writer in a finally block.

> **BoundedChannelFullMode.Wait is not the only option.** You can use `DropNewest`, `DropOldest`, or `DropWrite` when losing data is acceptable.

## Key takeaways

- `Channel.CreateBounded<T>(N)` creates a pipe with capacity N and natural backpressure.
- `WriteAsync` awaits when full; `ReadAsync` awaits when empty.
- Cleaner than `BlockingCollection` for async code; separate reader/writer handles.
- Always call `writer.Complete()` when done producing.