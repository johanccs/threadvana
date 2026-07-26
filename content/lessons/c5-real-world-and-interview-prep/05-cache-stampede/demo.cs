using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly ConcurrentDictionary<string, Lazy<Task<string>>> _cache = new();

    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? 5 callers hit the cache at once — all cache-miss");

        var tasks = new Task<string>[5];
        for (var i = 0; i < 5; i++) tasks[i] = Task.Run(() => GetOrFetchAsync("key"));

        Trace.Log("async-suspend", "? all 5 callers wait on the SAME Lazy<Task> — only ONE fetch runs");
        await Task.WhenAll(tasks);

        Trace.Log("async-resume", "? Lazy.Value delivered to all 5 — single-flight complete");
        Trace.Log("message", "Single-flight pattern: Lazy<Task<T>> ensures exactly one fetch, N awaiters.");
    }

    private static async Task<string> GetOrFetchAsync(string key)
    {
        var lazy = _cache.GetOrAdd(key, _ => new Lazy<Task<string>>(async () =>
        {
            Trace.Log("work-start", "fetching from DB (only once)");
            await Task.Delay(500);
            return "fresh-data";
        }));
        return await lazy.Value;
    }
}