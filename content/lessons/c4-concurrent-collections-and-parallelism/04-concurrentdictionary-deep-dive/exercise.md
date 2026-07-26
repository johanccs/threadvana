A `ConcurrentDictionary<string,int>` called `Solution.Scores` is already created for you.

Your job:

1. In `Solution.RecordScore(string player, int points)`, use `AddOrUpdate` to safely
   add the player if they are new, or add `points` to their existing total if they
   already have a score. The signature is:
   `dict.AddOrUpdate(key, addValueFactory, updateValueFactory)` where both factories
   receive (`key`) and (`key, existingValue`) respectively.
2. In `Solution.GetScore(string player)`, return the player's score (0 if not found)
   using `TryGetValue`.

The checker calls `RecordScore` from 20 threads for 3 players with random points,
then verifies the totals are exactly right â   no lost updates, no double-counts.

## Hints

1. `AddOrUpdate` is atomic per key: `(key, _ => points, (_, old) => old + points)`.
2. `TryGetValue` takes an `out` parameter for the result, returns `true`/`false`.
3. No `lock` needed â   the exercise is about using the dictionary's built-in methods.
