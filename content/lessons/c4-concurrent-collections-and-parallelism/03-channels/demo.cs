using System;
using System.Threading.Channels;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var channel = Channel.CreateBounded<string>(1); // 1 slot — forces synchronization

        var producer = Task.Run(async () =>
        {
            for (var i = 1; i <= 5; i++)
            {
                var msg = $"message {i}";
                Trace.Log("pool-queued", $"Producer writing: {msg}");
                await channel.Writer.WriteAsync(msg);
                Trace.Log("message", $"Producer wrote: {msg}");
            }
            channel.Writer.Complete();
            Trace.Log("message", "Producer done — channel closed");
        });

        var consumer = Task.Run(async () =>
        {
            await Task.Delay(200); // consumer starts a bit later
            await foreach (var msg in channel.Reader.ReadAllAsync())
            {
                Trace.Log("pool-dequeued", $"Consumer reading...");
                Trace.Log("work-start", $"Processing: {msg}");
                await Task.Delay(300);
                Trace.Log("work-end", $"Done: {msg}");
            }
            Trace.Log("message", "Consumer done");
        });

        await Task.WhenAll(producer, consumer);
        await Task.CompletedTask;
    }
}
