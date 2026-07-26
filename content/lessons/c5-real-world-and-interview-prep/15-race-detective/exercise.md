Fix `Solution.Transfer(int amount)` â   it increments `Solution.Balance` in a racy way. Add a `lock` around the read-modify-write. Return `"fixed"` after a test transfer of 1.

## Hints
1. `private static readonly object _gate = new();`
2. `lock(_gate) { Balance += amount; }`
