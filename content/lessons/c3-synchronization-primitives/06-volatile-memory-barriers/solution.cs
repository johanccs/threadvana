using System.Threading.Tasks;

public static class Solution
{
    public static volatile bool IsRunning;

    public static async Task<string> WaitUntilStarted()
    {
        while (!IsRunning) { }
        return "started";
    }

    public static void SignalStart() => IsRunning = true;
}
