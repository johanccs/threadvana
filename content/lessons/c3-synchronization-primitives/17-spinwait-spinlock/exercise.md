Write `Solution.IncrementWithSpinLock()`:

1. The shared counter `Solution.Counter` is an int. Increment it under a `SpinLock`.
2. Use the pattern: `bool taken = false; try { spin.Enter(ref taken); Counter++; } finally { if (taken) spin.Exit(); }`
3. Return `"incremented"`.

`Solution._spin` is already created for you.

## Hints
1. `_spin.Enter(ref lockTaken)` tries to acquire the spin lock.
2. Always `try/finally { if (lockTaken) _spin.Exit(); }`.
3. The short critical section makes SpinLock appropriate — just a single increment.
