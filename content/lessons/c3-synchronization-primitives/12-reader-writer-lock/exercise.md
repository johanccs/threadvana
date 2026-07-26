Write `Solution.GetCacheValue(string key)` using a `ReaderWriterLockSlim`:

- Reads: use `EnterReadLock` / `ExitReadLock`. Return the cached value or `"miss"`.
- Writes: `SetCacheValue(string key, string val)` uses `EnterWriteLock` / `ExitWriteLock` to update.

`Solution._cache` is a `Dictionary<string,string>` already created. Make it thread-safe
with the RW lock.

## Hints
1. `_rwl.EnterReadLock(); try { return _cache.GetValueOrDefault(key, "miss"); } finally { _rwl.ExitReadLock(); }`
2. Writer: `EnterWriteLock` + `_cache[key] = val` + `ExitWriteLock`.
3. `ReaderWriterLockSlim` needs `using System.Threading;`.
