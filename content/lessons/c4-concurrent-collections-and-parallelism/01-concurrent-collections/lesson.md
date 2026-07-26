---
id: c4-l01-concurrent-collections
category: c4-concurrent-collections-and-parallelism
order: 1
title: Concurrent Collections Ã Â¢Ã¢â  Â¬ the Thread-Safe Toolbox
difficulty: intermediate
description: "Get started with thread-safe collections: drop-in replacements for List, Dictionary, and Queue that work correctly under concurrency."
visualization: thread-timeline
explainer: lock-key
interview:
  - q: Why does a normal List or Dictionary break under multiple threads?
    a: Class collections are not thread-safe. Adding from two threads simultaneously can corrupt internal state (lost items, duplicated entries) or throw exceptions mid-operation.
  - q: What is the first concurrent collection you should reach for?
    a: ConcurrentDictionary is the most commonly used. It allows multiple threads to read and write without locks in user code Ã Â¢Ã¢â  Â¬ its operations are atomic per key.
---

ConcurrentDictionary/Queue/Bag are designed for multi-threaded use. A normal
Dictionary breaks when two threads write simultaneously. The demo shows both side by
side. The exercise has you switch from Dictionary to ConcurrentDictionary and see
the race disappear.
