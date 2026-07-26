Fetch a number with a Task, then collect it with `await`.

`Solution.FetchNumber()` is provided â   it pretends to be a slow web call and
delivers the number 42 after a short wait. Do not change it.

Inside `Solution.RunAsync()`:

1. Run `FetchNumber()` on the thread pool with `Task.Run(...)`. This hands you
   a `Task<int>` â   a receipt for the number, not the number itself.
2. `await` that task to collect the number it delivers.
3. Store the number in `Solution.Result`.

We check that `Result` ends up holding 42 â   proof that the task really ran and
you collected its result with `await`.

## Hints
1. `Task.Run(() => FetchNumber())` starts the work on a pool thread and returns a `Task<int>` receipt right away.
2. `int number = await receipt;` pauses RunAsync (without blocking a thread) until the receipt delivers the number.
3. The signature `public static async Task RunAsync()` is already there â   `async` is what allows `await` inside.