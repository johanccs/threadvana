Impl async lock with SemaphoreSlim. Return `"locked"`.

## Hints
`await _sem.WaitAsync(); try { return "locked"; } finally { _sem.Release(); }`
