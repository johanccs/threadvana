Write `Solution.TryAcquireSingleInstanceAsync()`:

1. Create a named `Mutex` with `new Mutex(false, @"Global\ThreadCraftExercise", out var createdNew)`.
2. If `createdNew` is `true`: return `"first"`.
3. If `createdNew` is `false`: return `"second"`.
4. Always `Dispose()` the mutex â   use a `using` block.

The harness runs your method twice in parallel â   one call should get "first", the other "second".

## Hints
1. `using var mutex = new Mutex(false, name, out var createdNew);`
2. If `createdNew`, this instance owns the mutex â   it is the first.
3. No need to call `WaitOne` or `Release` â   the `createdNew` flag is enough for this exercise.
