using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Slow worker — loops 20 times, checking the token each iteration. Don't change.</summary>
    public static async Task SlowWorkAsync(CancellationToken token)
    {
        for (var i = 0; i < 20; i++)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(100, token);
        }
    }

    /// <summary>Run SlowWorkAsync with a timeout. Return "cancelled" or "finished".</summary>
    public static async Task<string> ProcessWithTimeoutAsync(int timeoutMs)
    {
        // TODO: create CancellationTokenSource with the timeout, wrap the call in try/catch
        return "not implemented";
    }
}
