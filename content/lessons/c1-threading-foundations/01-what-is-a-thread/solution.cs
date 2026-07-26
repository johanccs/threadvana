using System;
using System.Threading;

public static class Solution
{
    // The checker reads this after Run() returns.
    // Your new thread must store its own thread id here.
    public static int WorkerThreadId = 0;

    public static void Run()
    {
        var worker = new Thread(() =>
        {
            // This line runs ON the new thread, so this is the new thread's id.
            WorkerThreadId = Environment.CurrentManagedThreadId;
        });

        worker.Start();
        worker.Join();
    }
}
