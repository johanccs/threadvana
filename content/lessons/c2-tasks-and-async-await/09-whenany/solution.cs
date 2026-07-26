using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> RealWorkAsync(CancellationToken token)
    {
        await Task.Delay(2000, token);
        return "done";
    }

    public static async Task<string> RaceWithTimeoutAsync(int timeoutMs)
    {
        using var cts = new CancellationTokenSource();
        var work = RealWorkAsync(cts.Token);
        var delay = Task.Delay(timeoutMs);

        var winner = await Task.WhenAny(work, delay);
        if (winner == delay)
        {
            cts.Cancel();
            return "timeout";
        }

        return await work;
    }
}
