using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var urls = new[] { "a.com/page1", "a.com/page2", "b.com/page1" };
        var channel = Channel.CreateBounded<string>(5);
        var concurrency = new SemaphoreSlim(2);

        var workers = new Task[3];
        for (var w = 0; w < 3; w++)
            workers[w] = Task.Run(async () =>
            {
                await foreach (var url in channel.Reader.ReadAllAsync())
                {
                    await concurrency.WaitAsync();
                    try
                    {
                        Trace.Log("work-start", $"Fetching {url}");
                        await Task.Delay(300);
                        Trace.Log("work-end", $"Done {url}");
                    }
                    finally { concurrency.Release(); }
                }
            });

        foreach (var url in urls) await channel.Writer.WriteAsync(url);
        channel.Writer.Complete();
        await Task.WhenAll(workers);
    }
}
