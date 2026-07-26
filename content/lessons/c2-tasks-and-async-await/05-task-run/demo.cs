using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        // Six quick tasks, pool capped at 2 workers — watch the queue drama.
        using var throttle = new SemaphoreSlim(2);
        var tasks = new Task[6];

        for (var i = 0; i < 6; i++)
        {
            var index = i + 1;
            tasks[i] = Task.Run(async () =>
            {
                Trace.Log("pool-queued", $"Task {index} waits for a worker");
                await throttle.WaitAsync();
                try
                {
                    Trace.Log("pool-dequeued", $"Task {index} got a worker");
                    await Task.Delay(400 + index * 80);
                    Trace.Log("work-end", $"Task {index} done");
                }
                finally
                {
                    throttle.Release();
                }
            });
        }

        await Task.WhenAll(tasks);
    }
}
