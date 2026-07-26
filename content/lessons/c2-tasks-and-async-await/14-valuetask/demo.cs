using System.Threading.Tasks;

public static class Demo
{
    private static int _cachedConfig = 100;

    public static async Task RunAsync()
    {
        Trace.Log("message", "Calling GetConfig five times — three cache hits (no task allocation), two misses");

        for (var i = 0; i < 5; i++)
        {
            Trace.Log("thread-start", $"Call {i + 1}");
            var val = await GetConfigAsync();
            Trace.Log("work-end", $"Got config: {val}");
        }
    }

    private static ValueTask<int> GetConfigAsync()
    {
        if (_cachedConfig < 103)
        {
            _cachedConfig++;
            return new ValueTask<int>(_cachedConfig); // sync — zero allocation
        }
        // Fall through to async — wraps in a real Task.
        return new ValueTask<int>(FetchFromRemoteAsync());
    }

    private static async Task<int> FetchFromRemoteAsync()
    {
        await Task.Delay(400);
        return 999;
    }
}
