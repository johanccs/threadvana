using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Lock cannot be awaited — demo: using SemaphoreSlim instead");
        var slim = new SemaphoreSlim(1, 1);
        await slim.WaitAsync();
        Trace.Log("work-start", "Async-safe 'lock' acquired");
        await Task.Delay(200);
        slim.Release();
        Trace.Log("work-end", "Released");
    }
}
