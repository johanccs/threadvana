using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task SlowWorkAsync(CancellationToken token)
    {
        for (var i = 0; i < 20; i++)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(100, token);
        }
    }

    public static async Task<string> ProcessWithTimeoutAsync(int timeoutMs)
    {
        using var cts = new CancellationTokenSource(timeoutMs);
        try
        {
            await SlowWorkAsync(cts.Token);
            return "finished";
        }
        catch (OperationCanceledException)
        {
            return "cancelled";
        }
    }
}
