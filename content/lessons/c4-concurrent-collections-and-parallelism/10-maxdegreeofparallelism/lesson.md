---
id: c4-l10-maxdegreeofparallelism
category: c4-concurrent-collections-and-parallelism
order: 10
title: "MaxDegreeOfParallelism Ã Â¢Ã¢â  Â¬ Tuning the Worker Count"
difficulty: intermediate
description: "Control parallelism with MaxDegreeOfParallelism: limit how many cores your parallel loop is allowed to use."
explainer: semaphore
interview:
  - q: "Why would you cap MaxDegreeOfParallelism below Environment.ProcessorCount?"
    a: "Because your server may be running other things Ã Â¢Ã¢â  Â¬ ASP.NET pipelines, background services, GC. Hogging all cores with a Parallel loop starves everything else. A common starting point is ProcessorCount - 1 (leave one core for the OS and other work). Also, if the work has contention (shared lock, shared cache line), more parallelism can be SLOWER Ã Â¢Ã¢â  Â¬ measure, don't assume."
  - q: "How do you set MaxDegreeOfParallelism?"
    a: "var opts = new ParallelOptions { MaxDegreeOfParallelism = 4 }; Parallel.ForEach(items, opts, item => Work(item)); The default is -1, which means 'unlimited' (effectively ProcessorCount after the partitioner creates its chunks)."
---

## What is it?

`ParallelOptions.MaxDegreeOfParallelism` is the concurrency cap for `Parallel.For`, `Parallel.ForEach`, and PLINQ. It's not the number of threads created Ã Â¢Ã¢â  Â¬ it's the maximum number of concurrent operations. The partitioner may still use fewer if the input is smaller than the cap.

## Watch out

> **Environment.ProcessorCount returns logical cores (hyperthreads), not physical.** On a 4-core hyperthreaded CPU, it's 8. For pure CPU work, capping at physical core count may be better Ã Â¢Ã¢â  Â¬ but you must measure.

## Key takeaways

- Cap parallelism to avoid starving other work on the machine.
- Common starting point: `ProcessorCount - 1`.
- Always measure Ã Â¢Ã¢â  Â¬ contention and cache effects can make MORE threads SLOWER.
