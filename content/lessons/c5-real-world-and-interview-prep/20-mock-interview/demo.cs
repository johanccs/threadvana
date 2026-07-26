using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var map = new ConcurrentDictionary<string, string>();
        var counter = 0;
        map.TryAdd("home", "abc123");
        Interlocked.Increment(ref counter);
        Trace.Log("message", $"URL shortener simulation: {map.Count} entries, {counter} IDs generated");
    }
}
