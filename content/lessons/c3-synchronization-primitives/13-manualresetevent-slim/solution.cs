using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly ManualResetEventSlim Gate = new(false);

    public static async Task<string> OpenAndCloseGateAsync()
    {
        Gate.Set();
        await Task.Delay(100);
        Gate.Reset();
        return "toggled";
    }
}
