using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task BackgroundJobAsync(CancellationToken token)
    {
        for (var i = 0; i < 10; i++)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(200, token);
        }
    }

    public static async Task<string> RunWithTimeoutAsync(int timeoutMs)
    {
        using var cts = new CancellationTokenSource(timeoutMs);
        var job = BackgroundJobAsync(cts.Token);
        var winner = await Task.WhenAny(job, Task.Delay(timeoutMs));
        if (winner != job)
        {
            cts.Cancel();
            return "timeout";
        }
        return "completed";
    }
}
