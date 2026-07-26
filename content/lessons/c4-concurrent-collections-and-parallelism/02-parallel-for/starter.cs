using System;
using System.Threading;

public static class Solution
{
    public static long ElapsedMs = 0;

    public static void Run()
    {
        // TODO: replace this sequential loop with Parallel.For.
        //       Measure elapsed time and store in ElapsedMs.
        for (var i = 0; i < 100; i++)
        {
            SlowSquare(i);
        }
    }

    private static void SlowSquare(int n)
    {
        // Simulates CPU work (~2 ms).
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < 2) { /* burn CPU */ }
    }
}
