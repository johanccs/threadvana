using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly object Gate = new();

    public static async Task<string> TryEnterWithTimeoutAsync()
    {
        if (Monitor.TryEnter(Gate, 500))
        {
            try
            {
                await Task.Delay(200);
                return "acquired";
            }
            finally { Monitor.Exit(Gate); }
        }
        return "timeout";
    }
}
