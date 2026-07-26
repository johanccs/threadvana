using System.Threading.Channels;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> PipeDataAsync()
    {
        var ch = Channel.CreateBounded<int>(3);
        var reader = Task.Run(async () =>
        {
            var sum = 0;
            await foreach (var x in ch.Reader.ReadAllAsync()) sum += x;
            return sum;
        });
        var writer = Task.Run(async () =>
        {
            for (var i = 1; i <= 6; i++) await ch.Writer.WriteAsync(i);
            ch.Writer.Complete();
        });
        await Task.WhenAll(writer, reader);
        return reader.Result.ToString();
    }
}
