Write `Solution.ProcessItemsAsync()`:

1. `Solution.InputQueue` is a `ConcurrentQueue<string>` pre-filled with 3 items.
2. Dequeue all items using `TryDequeue` and push them onto `Solution.OutputStack` (a `ConcurrentStack<string>`).
3. Return `"done"`.

## Hints
1. `while (InputQueue.TryDequeue(out var item)) OutputStack.Push(item);`
2. Return `"done"`.
