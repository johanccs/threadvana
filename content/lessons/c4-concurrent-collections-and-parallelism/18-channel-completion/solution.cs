using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> CompleteAndDrainAsync()
    {
        var ch = Channel.CreateUnbounded<string>();
        var items = new List<string>();
        var reader = Task.Run(async () =>
        {
            await foreach (var item in ch.Reader.ReadAllAsync())
                items.Add(item);
        });
        var writer = Task.Run(async () =>
        {
            await ch.Writer.WriteAsync("hello");
            await ch.Writer.WriteAsync("world");
            ch.Writer.Complete();
        });
        await Task.WhenAll(writer, reader);
        return string.Join(" ", items);
    }
}
