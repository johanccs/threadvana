Write `Solution.HandleFaultedPLINQAsync()`:

1. Run a PLINQ query on `Solution.Data` (int[] 1..10) that throws `InvalidOperationException` for items > 5.
2. Catch `AggregateException` and return the count of inner exceptions as a string.
3. If no exception: return `"0"`.

## Hints
1. `try { Data.AsParallel().Select(n => { if (n>5) throw new InvalidOperationException(); return n; }).ToArray(); }`
2. `catch (AggregateException ae) { return ae.InnerExceptions.Count.ToString(); }`
