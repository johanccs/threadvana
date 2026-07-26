using System.Collections.Generic;
using System.Threading;

public static class Solution
{
    private static readonly Dictionary<string, string> _cache = new();
    private static readonly ReaderWriterLockSlim _rwl = new();

    public static string GetCacheValue(string key)
    {
        _rwl.EnterReadLock();
        try { _cache.TryGetValue(key, out var val); return val ?? "miss"; }
        finally { _rwl.ExitReadLock(); }
    }

    public static void SetCacheValue(string key, string val)
    {
        _rwl.EnterWriteLock();
        try { _cache[key] = val; }
        finally { _rwl.ExitWriteLock(); }
    }
}
