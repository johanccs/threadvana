---
id: c4-l18-channel-completion
category: c4-concurrent-collections-and-parallelism
order: 18
title: "Channel Completion - Complete() and Clean Shutdown"
difficulty: intermediate
description: "Learn how to shut down a channel gracefully with Complete(). See how ReadAllAsync naturally stops when the channel is done, why sentinel values are obsolete, and how to handle errors during completion."
explainer: channel
interview:
  - q: "How do you signal a Channel that no more items are coming?"
    a: "Call writer.Complete() - this marks the channel as 'no more writes.' The reader's ReadAllAsync() naturally stops when the channel completes. You can also pass an Exception to Complete() which is surfaced to the reader."
  - q: "What happens if you call Complete() while the reader is still reading?"
    a: "The reader will finish processing all remaining buffered items, then ReadAllAsync returns. Complete() does NOT abort the reader mid-item - it is a graceful shutdown signal."
---

## What is it?

`ChannelWriter.Complete()` is the clean way to say "I am done writing - no more items will ever arrive." It replaces old tricks like sending a sentinel value (-1 means "done") or setting a shared boolean flag. The reader drains remaining items, then `ReadAllAsync()` returns naturally. You can pass an `Exception` to `Complete()` to tell the reader "we stopped because something went wrong."

## The real-world picture

A food production line has a conveyor belt (the channel). Workers at the start place items (writers). Workers at the end pick them up (readers). When the shift ends, the foreman flips a switch that stops new items from being placed - but the belt keeps running until all items already on it are processed. That switch is `Complete()`. No one throws away remaining items. No one puts a "LAST ONE" sticker on the final box. The belt runs dry and the workers know the shift is over. Graceful, clean, and obvious.

## How it works in C#

```csharp
using System.Threading.Channels;

var channel = Channel.CreateUnbounded<string>();

var producer = Task.Run(async () => {
    for (int i = 0; i < 5; i++) {
        await channel.Writer.WriteAsync($"Item {i}");
        await Task.Delay(200);
    }
    channel.Writer.Complete(); // "No more items."
});

var consumer = Task.Run(async () => {
    await foreach (var item in channel.Reader.ReadAllAsync())
        Console.WriteLine($"Received: {item}");
    Console.WriteLine("Channel completed.");
});

await Task.WhenAll(producer, consumer);

// Completion with an error
var errorChannel = Channel.CreateUnbounded<int>();
errorChannel.Writer.Complete(new InvalidOperationException("DB lost"));
try { await errorChannel.Reader.ReadAsync(); }
catch (ChannelClosedException ex) {
    Console.WriteLine($"Closed: {ex.InnerException?.Message}");
}
```

## See it move

Press **Run demo**. The channel visualization shows items flowing, then the writer lane stops and the Complete() signal travels through. The reader drains buffered items, then gracefully shuts down. No items are lost - the reader sees every item produced before Complete().

## Watch out

> **Do not write after Complete().** Calling WriteAsync after Complete() throws ChannelClosedException. Once you signal completion, the channel is sealed.

> **Complete() is one-way.** Only the writer calls it. The reader cannot signal "stop sending" through the channel - use a CancellationToken for that.

> **ReadAllAsync() is the cleanest pattern.** It handles completion, cancellation, and draining automatically. If you use raw ReadAsync, catch ChannelClosedException.

## Key takeaways

- `writer.Complete()` signals end-of-input - no sentinel values needed.
- `await foreach (var item in reader.ReadAllAsync())` stops when the channel completes.
- Pass an Exception to Complete() to signal an error shutdown.
- Once completed, the channel is sealed - no more writes.