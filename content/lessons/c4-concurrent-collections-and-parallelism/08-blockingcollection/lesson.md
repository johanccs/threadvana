---
id: c4-l08-blockingcollection
category: c4-concurrent-collections-and-parallelism
order: 8
title: "BlockingCollection Ã Â¢Ã¢â  Â¬ Bounding the Buffer"
difficulty: intermediate
description: "Use BlockingCollection with concurrent collections: bounded capacity, cancellation support, and GetConsumingEnumerable."
explainer: channel
interview:
  - q: "What problem does BlockingCollection solve?"
    a: "It puts a capacity limit on any IProducerConsumerCollection (usually a ConcurrentQueue). When the buffer is full, Add blocks the producer until a consumer frees a slot. When empty, Take blocks the consumer until a producer adds an item. This is the simplest bounded producer-consumer in .NET Ã Â¢Ã¢â  Â¬ no semaphore needed, the collection IS the back-pressure mechanism."
  - q: "How do you signal a BlockingCollection that no more items are coming?"
    a: "Call CompleteAdding(). After that, Add throws InvalidOperationException, and consumers using foreach (var item in collection.GetConsumingEnumerable()) will exit when the buffer is drained. This is the clean shutdown pattern for producer-consumer pipelines."
---

## What is it?

`BlockingCollection<T>` is a thread-safe queue with a lid Ã Â¢Ã¢â  Â¬ it blocks producers when full and consumers when empty. No manual signaling, no polling, no semaphores. Perfect for pipelines where one stage produces items faster than the next stage can consume them. Just set a capacity and the back-pressure is automatic.

## See it move

Press **Run demo** Ã Â¢Ã¢â  Â¬ a fast producer adds 10 items to a BlockingCollection capped at 3. A slow consumer takes one every 400ms. The producer blocks when the buffer hits 3, waiting for the consumer to free a slot.

## Key takeaways

- `new BlockingCollection<T>(capacity)` Ã Â¢Ã¢â  Â¬ bounded buffer.
- `Add()` blocks producer when full; `Take()` blocks consumer when empty.
- `CompleteAdding()` signals end-of-input; `GetConsumingEnumerable()` auto-exits.
