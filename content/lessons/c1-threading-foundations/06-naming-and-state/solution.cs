using System;
using System.Threading;

public static class Solution
{
    // The checker reads these after Run() returns.
    public static bool Ran = false;
    public static string WorkerName = null;

    // Provided: runs on YOUR thread and reports who it ran on.
    public static void Work()
    {
        Thread.Sleep(200); // pretend to work
        WorkerName = Thread.CurrentThread.Name; // null if the thread has no name!
        Ran = true;
    }

    public static void Run()
    {
        var worker = new Thread(Work);
        worker.Name = "data-worker"; // the badge - set ONCE, right after creating

        worker.Start();
        worker.Join(); // Run() returns only when Work is done
    }
}
