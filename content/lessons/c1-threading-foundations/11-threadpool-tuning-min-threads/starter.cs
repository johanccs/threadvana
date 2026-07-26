using System;
using System.Threading;

public static class Solution
{
    // TODO: set this to the number of workers you want ready.
    public static int Workers = 0;

    // TODO: flip this flag after calling SetMinThreads.
    public static bool UsedSetMinThreads = false;

    public static void Run()
    {
        // TODO: 1. Set Workers = 8.
        //       2. Call ThreadPool.SetMinThreads(Workers, Workers).
        //       3. Set UsedSetMinThreads = true.

        // Reset back to default so other lessons are not affected.
        // (This line is already here for you.)
        ThreadPool.SetMinThreads(1, 1);
    }
}
