Switch to the thread-safe version.

The starter uses a normal `Dictionary<int, int>` — two threads both add 500 items.
But the dictionary breaks under concurrent writes and the test fails.

Switch to `ConcurrentDictionary<int, int>`. That's it — change the declaration and
the `Add` call to `TryAdd`. Everything else stays the same.

## Hints
1. Change the field type: `new ConcurrentDictionary<int, int>()` instead of `new Dictionary<int, int>()`.
2. `dict.Add(key, value)` becomes `dict.TryAdd(key, value)` (needs `using System.Collections.Concurrent;`).
3. The harness checks Count == 1000 after both threads finish.
