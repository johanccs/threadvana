using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> CheckContextAsync()
    {
        var before = SynchronizationContext.Current is null ? "none" : "captured";
        await Task.Delay(1);
        var after = SynchronizationContext.Current is null ? "none" : "captured";
        return $"{before} {after}";
    }
}
