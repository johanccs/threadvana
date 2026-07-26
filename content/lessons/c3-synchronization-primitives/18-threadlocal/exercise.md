Write `Solution.GetThreadLocalId()`:

1. Use a `ThreadLocal<int>` to give each thread a unique id.
2. The factory seeds it with `Interlocked.Increment(ref _nextId)`.
3. Return `Thread.CurrentThread.ManagedThreadId.ToString()` (the value doesn't matter â   the harness checks that 4 threads produce 4 consistent values). Actually just return the ThreadLocal's `.Value.ToString()`.

The starter has `Solution._nextId` and a `ThreadLocal<int>` field ready â   initialise the ThreadLocal in a static ctor or inline.

## Hints
1. `new ThreadLocal<int>(() => Interlocked.Increment(ref _nextId))` seeds each thread's slot.
2. `return _local.Value.ToString();` returns the id for the calling thread.
3. No locking needed â   ThreadLocal handles isolation.
