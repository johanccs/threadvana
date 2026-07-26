using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Volatile flag — another thread sets this. Don't change.</summary>
    public static volatile bool IsRunning;

    /// <summary>Busy-wait until IsRunning becomes true, then return "started".</summary>
    public static async Task<string> WaitUntilStarted()
    {
        // TODO: spin-read IsRunning until true
        return "not implemented";
    }

    /// <summary>Set the flag. Don't change.</summary>
    public static void SignalStart() => IsRunning = true;
}
