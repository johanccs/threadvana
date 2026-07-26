using System;
using System.Threading;

public static class Solution
{
    public static int RunCount = 0;
    public static string WorkerName = "";

    public static void Run()
    {
        var worker = new Thread(() =>
        {
            WorkerName = Thread.CurrentThread.Name ?? "";
            DoWork();
        });
        worker.Name = "data-worker";
        worker.Start();
        worker.Join();
    }

    private static void DoWork()
    {
        for (var i = 0; i < 100; i++)
        {
            RunCount++;
            Thread.Sleep(1);
        }
    }
}
