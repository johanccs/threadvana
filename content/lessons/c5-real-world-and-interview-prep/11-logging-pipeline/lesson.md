---
id: c5-l11-logging-pipeline
category: c5-real-world-and-interview-prep
order: 11
title: "Build It: A High-Throughput Logging Pipeline on Channels"
difficulty: advanced
description: "Design an async logging pipeline: producers enqueue log entries, a background consumer writes them to disk in batches."
explainer: channel
interview:
  - q: "Why use Channels for a logging pipeline?"
    a: "Channels decouple the producer (the code writing logs) from the consumer (the code writing to disk/network). The producer just calls WriteAsync on the Channel and never blocks for I/O. The consumer drains the Channel in a loop and does the slow disk write on a single background thread. This gives you async, non-blocking logging with natural backpressure when the buffer fills."
---

Write `Solution.WriteLogAsync(string message)` that writes to a bounded `Channel<string>(100)`. A background `Task.Run` loop drains the Channel and increments `Solution.Logged`. Return `"done"`.
