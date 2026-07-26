using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Simulating a BackgroundService — loop until token fires");
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var worker = Task.Run(() => RunLoopAsync(cts.Token));
        await Task.WhenAny(worker, Task.Delay(3000));
        Trace.Log("message", "Worker stopped gracefully");
    }

    private static async Task RunLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Trace.Log("work-start", "Processing...");
            await Task.Delay(400, token);
            Trace.Log("work-end", "Item processed");
        }
    }
}
