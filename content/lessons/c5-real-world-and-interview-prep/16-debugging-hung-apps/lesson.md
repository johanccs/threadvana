---
id: c5-l16-debugging-hung-apps
category: c5-real-world-and-interview-prep
order: 16
title: "Debugging Hung Apps Ã Â¢Ã¢â  Â¬ Thinking in Dumps and Stacks"
difficulty: advanced
description: "Debug hung applications: find which threads are blocked, what they are waiting on, and why they never wake up."
explainer: deadlock
interview:
  - q: "Your production app is hanging. No crash, no exception. What do you do?"
    a: "1. Capture a dump: dotnet-dump collect -p <pid>. 2. Analyse: dotnet-dump analyze <dump> Ã Â¢Ã¢â ¬Â  clrthreads to see all managed threads. 3. Look for threads with long wait times, held locks, or Monitor.Enter at the bottom of the stack. 4. Use dumpheap and gcroot to find large object leaks. 5. If it's a deadlock, the parallel stacks view shows circular waits. The key mindset: don't guess Ã Â¢Ã¢â  Â¬ the dump has the answer."
---

Conceptual lesson Ã Â¢Ã¢â  Â¬ no exercise. Return `"ok"` from `Solution.Analyze()`.
