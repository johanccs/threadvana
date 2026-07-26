Wire up the signal.

A consumer thread loops, dequeuing from a `ConcurrentQueue<int>`. The producer
enqueues 5 numbers and then sets `Solution.Signal.Set()`. The consumer should exit
when the signal is set AND the queue is empty.

Your job: complete the consumer loop inside `Solution.Run()`. Use `queue.TryDequeue`
and check `Signal.IsSet && queue.IsEmpty` for the exit condition.

## Hints
1. `while (true) { if (queue.TryDequeue(out var item)) { ProcessItem(item); } else if (Signal.IsSet && queue.IsEmpty) break; else Thread.Sleep(1); }`
2. `ProcessItem` just calls `Interlocked.Increment(ref Solution.ProcessedCount);`
3. The producer (provided) already enqueues and calls `Signal.Set()`.
