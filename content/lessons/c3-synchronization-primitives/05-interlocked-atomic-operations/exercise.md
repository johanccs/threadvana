Fix the counter — without a lock.

`Solution.Count` is a plain `int`. The checker will fire 8 threads at it,
each calling `Solution.AddOne()` 50,000 times, and expects EXACTLY 400,000
at the end. No `lock` allowed — you will not need one.

Your job:

1. Rewrite `AddOne()` to add 1 atomically:
   `Interlocked.Increment(ref Count);`
2. Rewrite `ResetTo(int value)` to swap the value atomically:
   `Interlocked.Exchange(ref Count, value);`

## Hints
1. Interlocked lives in System.Threading and always takes the field with ref.
2. Increment(ref x) is x++ as one uninterruptible step; Exchange(ref x, v) is x = v as one step.
3. Still losing increments? Look for a leftover plain Count++ somewhere in your method.
