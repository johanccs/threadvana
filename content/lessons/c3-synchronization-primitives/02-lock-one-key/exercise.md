Make the bank transfer safe.

`Solution.Transfer(int amount)` moves money from `AccountA` to `AccountB`: it
CHECKS the balance, then SUBTRACTS from A and ADDS to B. The starter version
protects nothing — under load, two threads pass the check at the same time, the
balance goes NEGATIVE, and the bank's total money drifts.

Your job: wrap the WHOLE transfer — check, subtract, add — in one lock:

```csharp
lock (Solution.Gate)
{
    // the if-check with the subtract and add inside
}
```

`Solution.Gate` is provided — the one shared key. The checker hammers
`Transfer` from 4 threads × 250 times, over several rounds, and after every
round it verifies: the two balances still sum to 2000 AND `AccountA` never
went negative.

## Hints
1. Everything inside `if (AccountA >= amount) { ... }` is the critical section — the check AND both moves belong inside the same lock.
2. The syntax is `lock (Solution.Gate) { ... }` — put the whole if-block between the braces.
3. Locking only the subtract and the add is NOT enough — two threads can still both pass the balance check first. The check must be inside too.