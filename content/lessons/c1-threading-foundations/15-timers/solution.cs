using System;
using System.Threading;

public static class Solution
{
    public static bool Done = false;

    public static void Run()
    {
        using var timer = new Timer(
            _ => { Done = true; },
            state: null,
            dueTime: 300,
            period: Timeout.Infinite);

        while (!Done) Thread.Sleep(10);
    }
}
