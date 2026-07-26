---
id: c5-l01-producer-consumer-real-world
category: c5-real-world-and-interview-prep
order: 1
title: The Producer-Consumer Pattern  -  for Real
difficulty: advanced
description: "Build the classic producer-consumer pattern: one thread produces items, another consumes them, with a thread-safe buffer between them."
visualization: thread-pool
explainer: channel
interview:
  - q: Describe the producer-consumer pattern and when you have used it.
    a: A producer generates work items and puts them in a shared queue. One or more consumer threads dequeue and process them. I have used it for background email sending (producer = web request handler, consumer = email sender), log batching, and job processing queues. The key advantage is decoupling  -  producers and consumers can scale independently.
  - q: How do you shut down a consumer thread cleanly?
    a: The producer adds a sentinel item (e.g. null or a special wrapper) or signals completion via a flag/event. The consumer exits its loop when it sees the sentinel or the channel is closed. Cooperative cancellation is always preferred.
---

The classic background-processing pattern. A producer enqueues work, a consumer
dequeues and processes. The exercise wraps everything into a clean Solution with
signal-based shutdown.
