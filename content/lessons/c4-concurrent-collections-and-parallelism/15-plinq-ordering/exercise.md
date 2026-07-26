Write `Solution.ProcessWithForAll()`:

1. Use PLINQ on `Solution.Items` (int[] 1..5) with `.AsParallel().Where(n => n % 2 == 0)`.
2. Use `.ForAll(n => Interlocked.Add(ref Solution.EvenCount, 1))`.
3. Return `"done"`.

## Hints
1. `Items.AsParallel().Where(n => n % 2 == 0).ForAll(n => Interlocked.Add(ref EvenCount, 1));`
2. Return `"done"`.
