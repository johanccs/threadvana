`Solution.Counter` is a shared `int` incremented from many threads — with a race condition.
Fix it by adding a `private static readonly object` lock object and wrapping the increment
in a `lock` block.

Also add a `Reset()` method that sets `Counter` to 0 — also under the lock.

## Hints
1. `private static readonly object _gate = new();` as the lock object.
2. `lock (_gate) { Counter++; }` — keep it short.
3. `Reset()` should also lock — read+write must be atomic together.
