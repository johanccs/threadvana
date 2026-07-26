---
id: c5-l02-deadlock-detective
category: c5-real-world-and-interview-prep
order: 2
title: Deadlock Detective Ã Â¢Ã¢â  Â¬ Spot the Circular Wait
difficulty: advanced
description: "Become a deadlock detective: learn to spot the four conditions that cause deadlocks and the strategies to break each one."
visualization: thread-timeline
explainer: deadlock
interview:
  - q: What causes a deadlock?
    a: "Four conditions must all be true Ã Â¢Ã¢â  Â¬ mutual exclusion (locks), hold-and-wait (thread holds lock A while waiting for B), no preemption (locks cannot be taken away), and circular wait (A waits for B, B waits for A). Break any one and the deadlock is impossible."
  - q: How do you fix a deadlock?
    a: "The simplest fix is consistent lock ordering Ã Â¢Ã¢â  Â¬ always acquire locks in the SAME order everywhere. Never lock A then B in one place and B then A in another. Use timeouts, reduce lock scope, or use lock-free data structures."
---

Two threads each grab one lock and wait for the other's lock Ã Â¢Ã¢â  Â¬ classic deadlock.
The demo shows it on the timeline. The exercise gives you deadlocked code and asks
you to fix the lock order.
