using System;
using System.Threading.Channels;
using System.Threading.Tasks;

public static class Solution
{
    public static string LastMessage = "";

    public static async Task RunAsync()
    {
        var channel = Channel.CreateBounded<string>(1);

        var producer = Task.Run(async () =>
        {
            await channel.Writer.WriteAsync("hello");
            await channel.Writer.WriteAsync("world");
            await channel.Writer.WriteAsync("done");
            channel.Writer.Complete();
        });

        var consumer = Task.Run(async () =>
        {
            while (await channel.Reader.WaitToReadAsync())
            {
                var msg = await channel.Reader.ReadAsync();
                LastMessage = msg;
            }
        });

        await Task.WhenAll(producer, consumer);
    }
}
