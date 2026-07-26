`Solution.Counter` is a shared `int` incremented from many threads â   with a race condition.
Fix it by adding a `private static readonly object` lock object and wrapping the increment
in a `lock` block.

Also add a `Reset()` method that sets `Counter` to 0 â   also under the lock.

## Hints
1. `private static readonly object _gate = new();` as the lock object.
2. `lock (_gate) { Counter++; }` â   keep it short.
3. `Reset()` should also lock â   read+write must be atomic together.
