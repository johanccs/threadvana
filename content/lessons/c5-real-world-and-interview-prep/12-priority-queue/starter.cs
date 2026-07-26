using System.Collections.Concurrent;
using System.Threading.Tasks;

public static class Solution
{
    private static readonly ConcurrentQueue<string> High = new();
    private static readonly ConcurrentQueue<string> Medium = new();
    private static readonly ConcurrentQueue<string> Low = new();

    public static Task EnqueueAsync(int priority, string work)
    {
        var q = priority switch { 3 => High, 2 => Medium, _ => Low };
        q.Enqueue(work);
        return Task.CompletedTask;
    }

    public static Task<string> DequeueAsync()
    {
        if (High.TryDequeue(out var h)) return Task.FromResult(h);
        if (Medium.TryDequeue(out var m)) return Task.FromResult(m);
        Low.TryDequeue(out var l);
        return Task.FromResult(l ?? "");
    }
}
