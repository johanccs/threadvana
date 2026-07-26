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
            // TODO: record the current thread's name into WorkerName
            WorkerName = Thread.CurrentThread.Name ?? "";
            DoWork();
        });

        // TODO: name the thread "data-worker"
        // TODO: start it
        // TODO: join it
    }

    // -- provided --

    private static void DoWork()
    {
        for (var i = 0; i < 100; i++)
        {
            RunCount++;
            Thread.Sleep(1);
        }
    }
}
