using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        // Slim version — fast, async, shows the semaphore viz.
        using var slim = new SemaphoreSlim(2, 2);
        Trace.Log("message", "SemaphoreSlim(2,2) — at most 2 workers inside");

        var tasks = new Task[5];
        for (var i = 0; i < 5; i++)
        {
            var idx = i + 1;
            tasks[i] = Task.Run(async () =>
            {
                Trace.Log("pool-queued", $"Worker {idx} waiting");
                await slim.WaitAsync();
                try
                {
                    Trace.Log("pool-dequeued", $"Worker {idx} entered");
                    await Task.Delay(400 + idx * 50);
                    Trace.Log("work-end", $"Worker {idx} leaving");
                }
                finally { slim.Release(); }
            });
        }
        await Task.WhenAll(tasks);
        Trace.Log("message", "All workers done — 2 at a time, zero kernel transitions");
    }
}
