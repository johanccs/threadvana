---
id: c4-l11-cancellation-in-parallel
category: c4-concurrent-collections-and-parallelism
order: 11
title: "Cancellation in Parallel Loops"
difficulty: intermediate
description: "Cancel a running parallel loop gracefully with CancellationToken and ParallelLoopState.Stop()."
explainer: cancellation
interview:
  - q: "How do you cancel a Parallel.ForEach mid-execution?"
    a: "Pass a ParallelOptions with a CancellationToken. When the token is signalled, Parallel.ForEach stops launching new iterations and the loop throws OperationCanceledException. Already-running iterations are NOT forcefully killed  -  they must poll the token themselves with ThrowIfCancellationRequested() to stop mid-work. Otherwise, they finish, and the loop cancels after they complete."
  - q: "What happens to already-started iterations when the CancellationToken fires?"
    a: "They complete unless they have their own cancellation check. Parallel.ForEach does not abort threads  -  it prevents NEW iterations and throws once all current iterations finish. For long-running per-item work, pass the SAME token into the lambda and periodically check it."
---

## What is it?

Cancelling a parallel loop is a two-step cooperation: the outer loop stops scheduling new items AND the inner item bodies must check the token to stop what they are doing. Without both, cancellation is incomplete — you get a partial result with some items still running.

## Watch out

> **OperationCanceledException from Parallel.ForEach wraps the user exception.** If a lambda throws its own OCE, the parallel loop rethrows it but may also fire the AggregateException wrapper. Always catch OperationCanceledException specifically and check the inner exceptions.

## Key takeaways

- `ParallelOptions { CancellationToken = token }` → cancel new iterations.
- Lambda must ALSO check `token.ThrowIfCancellationRequested()` for in-flight items.
- No thread abort — current iterations finish unless they check.
