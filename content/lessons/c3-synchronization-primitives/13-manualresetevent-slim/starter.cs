using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly ManualResetEventSlim Gate = new(false);

    public static async Task<string> OpenAndCloseGateAsync()
    {
        // TODO: Set, short delay, Reset, return "toggled"
        return "not implemented";
    }
}
