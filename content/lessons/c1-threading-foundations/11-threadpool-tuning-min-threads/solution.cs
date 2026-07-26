using System;
using System.Threading;

public static class Solution
{
    public static int Workers = 8;
    public static bool UsedSetMinThreads = false;

    public static void Run()
    {
        ThreadPool.SetMinThreads(Workers, Workers);
        UsedSetMinThreads = true;

        // Reset back to default so other lessons are not affected.
        ThreadPool.SetMinThreads(1, 1);
    }
}

