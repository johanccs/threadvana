---
id: c5-l15-race-detective
category: c5-real-world-and-interview-prep
order: 15
title: "Race Condition Detective  -  Hunt the Heisenbug"
difficulty: advanced
description: "Hunt down race conditions with practical techniques: logging, deterministic repro steps, and tools that reveal the race."
explainer: race-interleaving
interview:
  - q: "How do you find a race condition that only happens once every thousand runs?"
    a: "Instrument the code: add counters around suspect operations, run in a tight loop (10,000+ iterations), and assert invariants after each run. Tools like CHESS (research), Coyote (Microsoft's systematic testing), or even Thread.Sleep at strategic points can widen the race window. Log the interleaving  -  record thread ids + timestamps at each shared access. The Heisenbug disappears under the debugger; force it out with stress + observation."
---

`Solution.Transfer(int from, int to, int amount)` is buggy Ã¢â‚¬â€ two transfers from the same account can create money out of thin air. Fix it by adding a proper lock. Return `"fixed"`.
