Write `Solution.CompleteAndDrainAsync()`:

1. Create an unbounded `Channel<string>`.
2. Writer: write "hello", "world", then call `Complete()`.
3. Reader: `await foreach` to collect items into a list.
4. Return items joined with a space: `"hello world"`.

## Hints
1. `var ch = Channel.CreateUnbounded<string>();`
2. After writing, `ch.Writer.Complete();`
3. `await foreach (var item in ch.Reader.ReadAllAsync()) items.Add(item);`
4. Return `string.Join(" ", items)`.
