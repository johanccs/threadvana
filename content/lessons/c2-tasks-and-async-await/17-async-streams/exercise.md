Write `Solution.CountToAsync(int n)` as an `async IAsyncEnumerable<int>` that yields
numbers from 1 to `n`, waiting 50ms between each yield.

Then write `Solution.SumStreamAsync(int n)` that consumes the stream with
`await foreach` and returns the sum of all yielded numbers.

The harness calls `SumStreamAsync(5)` and expects `15` (1+2+3+4+5).

## Hints
1. `async IAsyncEnumerable<int>` with `yield return i` inside the loop.
2. In `SumStreamAsync`, `await foreach (var x in Solution.CountToAsync(n))` to iterate.
3. You need `using System.Collections.Generic;` for `IAsyncEnumerable`.
