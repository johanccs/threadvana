Write a `Solution.ComputeSumAsync()` method that:

1. Starts THREE `Task<int>` computations in parallel using `Task.Run` â   each
   should call `Solution.ExpensiveCompute(int n)` where `n` is 10, 20 and 30.
2. Awaits all three to finish with `Task.WhenAll`.
3. Returns the sum of the three results.

`ExpensiveCompute` is provided â   it simulates a slow calculation by sleeping
and returns `n * n`. Your job is only the async orchestration.

## Hints
1. `Task.Run(() => ExpensiveCompute(10))` returns a `Task<int>` â   store each in a variable.
2. `int[] results = await Task.WhenAll(t1, t2, t3)` gives you the array of returned values.
3. No `async` on `ComputeSumAsync`? You need it if you use `await`.
