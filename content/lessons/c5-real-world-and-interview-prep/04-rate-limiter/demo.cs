using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var limiter = new DemoRateLimiter(tokensPerSecond: 5, maxTokens: 8);
        var tasks = new Task<bool>[20];

        for (var i = 0; i < 20; i++)
        {
            var idx = i + 1;
            tasks[i] = Task.Run(async () =>
            {
                Trace.Log("thread-start", $"Requester {idx}");
                await Task.Delay(50 * idx); // staggered arrival
                var ok = await limiter.TryConsumeAsync();
                Trace.Log(ok ? "work-end" : "wait-start", ok ? $"Req {idx} PASSED" : $"Req {idx} queued");
                return ok;
            });
        }

        await Task.WhenAll(tasks);
        var passed = 0;
        foreach (var t in tasks) if (t.Result) passed++;
        Trace.Log("message", $"{passed} of {tasks.Length} requests passed the rate limiter");
    }
}

// A tiny token bucket for the demo (same pattern as the lesson).
internal sealed class DemoRateLimiter
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Timer _timer;
    private volatile int _tokens;

    public DemoRateLimiter(int tokensPerSecond, int maxTokens)
    {
        _tokens = maxTokens;
        var interval = (int)(1000.0 / tokensPerSecond);
        _timer = new Timer(_ => Interlocked.Add(ref _tokens, 1), null, interval, interval);
    }

    public async Task<bool> TryConsumeAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (Volatile.Read(ref _tokens) <= 0) return false;
            Interlocked.Decrement(ref _tokens);
            return true;
        }
        finally { _gate.Release(); }
    }
}
