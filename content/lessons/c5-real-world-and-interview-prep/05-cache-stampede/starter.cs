using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public static class Solution
{
    private static readonly ConcurrentDictionary<string, Lazy<Task<string>>> _cache = new();

    public static Task<string> FetchFromSourceAsync(string key)
        => Task.FromResult($"value-{key}");

    public static async Task<string> GetValueAsync(string key)
    {
        var lazy = _cache.GetOrAdd(key, _ => new Lazy<Task<string>>(() => FetchFromSourceAsync(key)));
        return await lazy.Value;
    }
}
