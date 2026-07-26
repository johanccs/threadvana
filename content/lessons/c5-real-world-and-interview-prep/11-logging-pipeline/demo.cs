using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var channel = Channel.CreateBounded<string>(5);
        var consumer = Task.Run(async () =>
        {
            await foreach (var msg in channel.Reader.ReadAllAsync())
                Trace.Log("work-end", $"Logged: {msg}");
        });
        for (var i = 0; i < 6; i++)
        {
            await channel.Writer.WriteAsync($"msg-{i}");
            Trace.Log("work-start", $"Enqueued msg-{i}");
        }
        channel.Writer.Complete();
        await consumer;
    }
}
