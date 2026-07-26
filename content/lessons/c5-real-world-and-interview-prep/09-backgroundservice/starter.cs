using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int Counter;

    public static async Task<string> RunUntilCancelledAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Interlocked.Increment(ref Counter);
            try { await Task.Delay(50, token); } catch (OperationCanceledException) { break; }
        }
        return "stopped";
    }
}
