using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>A slow job that honours cancellation. Don't change.</summary>
    public static async Task BackgroundJobAsync(CancellationToken token)
    {
        for (var i = 0; i < 10; i++)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(200, token);
        }
    }

    /// <summary>Run the job with a timeout — return "completed" or "timeout".</summary>
    public static async Task<string> RunWithTimeoutAsync(int timeoutMs)
    {
        // TODO: CancellationTokenSource, WhenAny, cancel on timeout
        return "not implemented";
    }
}
