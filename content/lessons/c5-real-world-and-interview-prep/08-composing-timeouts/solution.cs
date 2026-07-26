using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task SlowApiAsync(CancellationToken token)
        => await Task.Delay(5000, token);

    public static async Task<string> CallWithLinkedCancellationAsync(CancellationToken userToken, int timeoutMs)
    {
        using var timeoutCts = new CancellationTokenSource(timeoutMs);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(userToken, timeoutCts.Token);
        try
        {
            await SlowApiAsync(linked.Token);
            return "ok";
        }
        catch (OperationCanceledException) { return "cancelled"; }
    }
}
