Write `Solution.ProduceAndConsume()`:

1. Create a `BlockingCollection<int>` with capacity 5.
2. Producer: start a Task.Run that adds numbers 1..8, then calls `CompleteAdding()`.
3. Consumer: use `foreach (var item in bc.GetConsumingEnumerable())` to process items.
4. Wait for the consumer to finish, then return `"done"`.

## Hints
1. `var bc = new BlockingCollection<int>(5);`
2. Producer: `for (var i = 1; i <= 8; i++) bc.Add(i); bc.CompleteAdding();`
3. Consumer: `foreach (var x in bc.GetConsumingEnumerable()) { /* track */ }`
