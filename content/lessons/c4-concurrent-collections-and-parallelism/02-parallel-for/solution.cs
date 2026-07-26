using System;
using System.Diagnostics;
using System.Threading.Tasks;

public static class Solution
{
    public static long ElapsedMs = 0;

    public static void Run()
    {
        var sw = Stopwatch.StartNew();

        Parallel.For(0, 100, i =>
        {
            SlowSquare(i);
        });

        ElapsedMs = sw.ElapsedMilliseconds;
    }

    private static void SlowSquare(int n)
    {
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < 2) { }
    }
}
