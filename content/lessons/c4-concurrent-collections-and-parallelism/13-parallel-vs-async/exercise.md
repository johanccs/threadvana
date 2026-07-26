Write `Solution.ClassifyWorkloadAsync(bool cpuBound)`:

If `cpuBound` is true: use `Parallel.ForEach` with `Solution.Data` (int[5]) to compute
the sum (use Interlocked.Add) and return it as a string.

If `cpuBound` is false: use `Task.WhenAll` with async tasks doing `Task.Delay(50)` to
count items and return the count as a string.

## Hints
1. CPU path: `Parallel.ForEach(Data, item => Interlocked.Add(ref sum, item));`
2. I/O path: `var tasks = Data.Select(async _ => { await Task.Delay(50); count++; }); await Task.WhenAll(tasks);`
