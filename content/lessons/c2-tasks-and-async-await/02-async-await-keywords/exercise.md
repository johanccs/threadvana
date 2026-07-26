Fix a method that makes coffee the WRONG way.

`Solution.MakeCoffeeAsync()` currently uses `.Result` and `.Wait()` â   it parks
a thread at the counter while the water boils. It works, but it wastes a worker.
Your job is to rewrite it the right way.

Two helpers are provided (do not change them). They record each step into
`Solution.Log` so the checker can see the order things happened:

- `BoilWaterAsync()` â   takes a moment, then logs `"water boiled"` and returns `true`.
- `PourCoffee()` â   logs `"coffee poured"`.

Rewrite `Solution.MakeCoffeeAsync()` so that it:

1. Has the signature `public static async Task<string> MakeCoffeeAsync()`.
2. `await`s `BoilWaterAsync()` first â   pause the method, not a thread.
3. Then `await`s `PourCoffee()` â   pouring must happen AFTER the water is boiled.
4. Returns the string `"coffee ready"`.

We check the returned string, that boiling really finished before pouring, and
that your method uses a real `await` instead of parking a thread.

## Hints
1. Change the signature to `public static async Task<string> MakeCoffeeAsync()` â   `async` is what unlocks `await` inside.
2. The pattern is: `await BoilWaterAsync();` then `await PourCoffee();` then `return "coffee ready";` â   three lines, in that order.
3. Delete every `.Result` and `.Wait()`. If the water is not boiled before the pour, the `await` on line one is missing.