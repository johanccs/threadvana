The starter creates a deadlock: two `bool` flags + two threads each acquiring them in
opposite orders with `SpinWait.SpinUntil`. `Solution.RunDeadlockFree()` should return
`"safe"` by restructuring the locking so the same result is computed WITHOUT deadlocking.

Option: acquire the lock objects in a consistent order. The lock objects are fields
`Solution.LockA` and `Solution.LockB`.

## Hints
1. The bug is that both threads lock A-then-B in opposite orders. Fix: same order everywhere.
2. You can rewrite the threads or introduce a single lock that gates both flags.
3. Return `"safe"` when both threads finish â   the check is just that your method returns.
