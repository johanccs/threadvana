Write `Solution.PipeDataAsync()`:

1. Create a bounded `Channel<int>` with capacity 3.
2. Writer: write numbers 1..6, then call `Complete()`.
3. Reader: use `ReadAllAsync()` to sum all items.
4. Return the sum as a string.

## Hints
1. `var ch = Channel.CreateBounded<int>(3);`
2. `await ch.Writer.WriteAsync(i);` in a loop; `ch.Writer.Complete();` after.
3. `await foreach (var x in ch.Reader.ReadAllAsync()) sum += x;`
