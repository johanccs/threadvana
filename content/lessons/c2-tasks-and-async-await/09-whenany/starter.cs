using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>A slow method that honours cancellation. Don't change.</summary>
    public static async Task<string> RealWorkAsync(CancellationToken token)
    {
        await Task.Delay(2000, token);
        return "done";
    }

    /// <summary>Race RealWorkAsync against a timeout — return the result or "timeout".</summary>
    public static async Task<string> RaceWithTimeoutAsync(int timeoutMs)
    {
        // TODO: Task.WhenAny with RealWorkAsync and Task.Delay
        return "not implemented";
    }
}
