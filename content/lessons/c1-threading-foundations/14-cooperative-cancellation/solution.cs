using System;
using System.Threading;

public static class Solution
{
    public static volatile bool StopRequested;

    public static long Count = 0;

    public static void Run()
    {
        var worker = new Thread(() =>
        {
            for (var i = 0; i < 1_000_000_000 && !StopRequested; i++)
            {
                Increment();
            }
        });
        worker.Name = "worker";
        worker.Start();

        Thread.Sleep(100);
        StopRequested = true;
        worker.Join();
    }

    public static void Increment() => Count++;
}
