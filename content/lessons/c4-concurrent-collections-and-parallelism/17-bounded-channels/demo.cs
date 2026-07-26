using System.Threading.Channels;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var channel = Channel.CreateBounded<int>(3);
        var writer = Task.Run(async () =>
        {
            for (var i = 0; i < 8; i++)
            {
                Trace.Log("work-start", $"Writing {i} (buffer wait if full)");
                await channel.Writer.WriteAsync(i);
                Trace.Log("work-end", $"Wrote {i}");
            }
            channel.Writer.Complete();
        });
        var reader = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Trace.Log("thread-start", $"Reading {item}...");
                await Task.Delay(300);
            }
        });
        await Task.WhenAll(writer, reader);
    }
}
