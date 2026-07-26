using System.Threading.Channels;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var channel = Channel.CreateUnbounded<string>();
        var writer = Task.Run(async () =>
        {
            for (var i = 0; i < 4; i++)
            {
                await channel.Writer.WriteAsync($"msg-{i}");
                await Task.Delay(100);
            }
            channel.Writer.Complete();
            Trace.Log("message", "Writer completed — reader will drain and stop");
        });
        await foreach (var item in channel.Reader.ReadAllAsync())
            Trace.Log("work-end", $"Read: {item}");
    }
}
