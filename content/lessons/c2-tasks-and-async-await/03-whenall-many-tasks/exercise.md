Cook the whole breakfast at once.

Three helpers are provided (do not change them). Each takes ~500 ms and then
records its item in `Solution.Finished`:

- `BoilEggsAsync()` records `"eggs"`
- `FryBaconAsync()` records `"bacon"`
- `ToastBreadAsync()` records `"toast"`

The starter cooks them ONE BY ONE — about 1.5 s, and the toast is cold before
the eggs are done. Rewrite `Solution.MakeBreakfastAsync()`:

1. Start ALL THREE first — call each helper and keep the `Task` it returns.
   (Calling an async method already starts the work; you just get a receipt.)
2. `await Task.WhenAll(eggs, bacon, toast);` — wait for the combined receipt.

We check that all three items are recorded AND that breakfast is ready in
under 900 ms — impossible unless everything cooked at the same time.

## Hints
1. `Task eggs = BoilEggsAsync();` starts the eggs AND hands you the receipt — the work runs while your code continues.
2. `Task.WhenAll(a, b, c)` returns one task that finishes when all three finish — `await` that one.
3. If your first line is `await BoilEggsAsync();`, the kitchen stalls until the eggs are done. Start everything before you await anything.