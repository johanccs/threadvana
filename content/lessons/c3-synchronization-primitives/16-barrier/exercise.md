Write `Solution.RunPhasesAsync()`:

1. Create a `Barrier(2)`.
2. Start 2 workers, each calling `barrier.SignalAndWait()` 3 times.
3. After both finish, return `"phased"`.

## Hints
1. `var barrier = new Barrier(2);`
2. Each worker: `for (var i = 0; i < 3; i++) { await Task.Delay(50); barrier.SignalAndWait(); }`
3. `Task.WhenAll` the two workers, then return.
