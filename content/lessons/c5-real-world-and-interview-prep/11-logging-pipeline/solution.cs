using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int Logged;
    private static readonly Channel<string> _channel = Channel.CreateBounded<string>(100);

    static Solution()
    {
        Task.Run(async () =>
        {
            await foreach (var msg in _channel.Reader.ReadAllAsync())
                Interlocked.Increment(ref Logged);
        });
    }

    public static async Task<string> WriteLogAsync(string message)
    {
        await _channel.Writer.WriteAsync(message);
        return "done";
    }
}
