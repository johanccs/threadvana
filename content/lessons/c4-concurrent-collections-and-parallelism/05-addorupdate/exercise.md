Write two methods on `Solution.Scores` (a `ConcurrentDictionary<string,int>`):

- `RecordHit(string page)`: use `AddOrUpdate` to add 1 to the page's count.
- `GetHits(string page)`: return the count (0 if not found).

## Hints
1. `Scores.AddOrUpdate(page, _ => 1, (_, old) => old + 1);`
2. `Scores.TryGetValue(page, out var c) ? c : 0`
