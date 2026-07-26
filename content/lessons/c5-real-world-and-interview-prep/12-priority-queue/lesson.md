---
id: c5-l12-priority-queue
category: c5-real-world-and-interview-prep
order: 12
title: "Build It: A Priority Work Queue"
difficulty: advanced
description: "Build a priority work queue: high-priority items jump the line ahead of normal-priority items using multiple channels."
explainer: channel
interview:
  - q: "How would you implement a thread-safe priority queue?"
    a: "Use a lock-guarded SortedSet or a min-heap per priority level, or use multiple ConcurrentQueues Ã¢â‚¬â€ one per priority Ã¢â‚¬â€ and always dequeue from the highest-priority non-empty queue first. The multiple-queue approach avoids lock contention at the cost of fairness across priorities."
---

Write `Solution.EnqueueAsync(int priority, string work)` and `DequeueAsync()`. Use 3 `ConcurrentQueue<string>` for low/medium/high priority. Dequeue checks high first, then medium, then low.
