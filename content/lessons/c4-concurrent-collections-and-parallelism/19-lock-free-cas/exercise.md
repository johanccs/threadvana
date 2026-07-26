Write `Solution.AtomicMultiply(int factor)`:

1. Read `Solution.Value` using `Volatile.Read`.
2. In a CAS loop, multiply by `factor` and try `Interlocked.CompareExchange`.
3. If CAS fails, retry with the latest value.
4. Return `"done"`.

## Hints
1. `int current = Volatile.Read(ref Value);`
2. `while (true) { int next = current * factor; int original = Interlocked.CompareExchange(ref Value, next, current); if (original == current) break; current = original; }`
