---
id: c4-l03-channels
category: c4-concurrent-collections-and-parallelism
order: 3
title: Channels Ã¢â‚¬â€ the Modern Producer-Consumer Pipe
difficulty: advanced
description: "Discover System.Threading.Channels: the modern, async-native producer-consumer pipeline that replaces BlockingCollection in async code."
visualization: thread-pool
explainer: channel
interview:
  - q: What is a Channel in .NET?
    a: A Channel is a modern thread-safe pipe between a producer and a consumer. One side writes items (Writer), the other reads them (Reader). It supports bounded capacity (back-pressure Ã¢â‚¬â€ writer waits when full) and async/await natively.
  - q: How is a Channel different from a BlockingCollection?
    a: Channel is newer, lighter, and async-native. BlockingCollection wraps a ConcurrentQueue with blocking Take/Add. Channel has built-in bounded capacity with async waiting, making it the preferred choice in .NET Core 3.0+.
---

A Channel is a pipe: write on one end, read on the other. The exercise builds a
producer that sends 5 items and a consumer that reads them Ã¢â‚¬â€ using a bounded
Channel with one slot.
