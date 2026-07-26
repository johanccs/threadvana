Write `Solution.FillAndDrainBag()`:

1. `Solution.Items` is a `ConcurrentBag<int>`.
2. Add the numbers 1 through 4 to it.
3. Take 4 items from it using `TryTake`.
4. Return the count of items successfully taken as a string (e.g. `"4"`).

## Hints
1. `for (var i = 1; i <= 4; i++) Items.Add(i);`
2. `while (Items.TryTake(out _)) count++;`
3. Return `count.ToString()`.
