Write `Solution.TryEnterWithTimeoutAsync()`:

1. Acquire `Solution.Gate` using `Monitor.TryEnter` with a 500ms timeout.
2. If acquired: do some work (sleep 200ms), release, and return `"acquired"`.
3. If timeout: return `"timeout"`.

`Solution.Gate` is already defined as `public static readonly object Gate = new();`

## Hints
1. `Monitor.TryEnter(Gate, 500)` returns `true` if got the lock.
2. Always release with `Monitor.Exit(Gate)` inside `try/finally`.
3. No `lock` keyword — this exercise is specifically about Monitor.
