using System.Collections.Generic;
using System.Threading;

public static class Solution
{
    private static readonly Dictionary<string, string> _cache = new();
    private static readonly ReaderWriterLockSlim _rwl = new();

    public static string GetCacheValue(string key)
    {
        // TODO: EnterReadLock, read, ExitReadLock
        _cache.TryGetValue(key, out var val);
        return val ?? "miss";
    }

    public static void SetCacheValue(string key, string val)
    {
        // TODO: EnterWriteLock, write, ExitWriteLock
        _cache[key] = val;
    }
}
