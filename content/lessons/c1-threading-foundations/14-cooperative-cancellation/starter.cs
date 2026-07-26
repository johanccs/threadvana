using System;
using System.Threading;

public static class Solution
{
    // TODO: add a volatile bool flag here (call it StopRequested).

    public static long Count = 0;

    public static void Run()
    {
        var worker = new Thread(() =>
        {
            // TODO: add the stop check to the loop condition.
            for (var i = 0; i < 1_000_000_000; i++)
            {
                // Record each iteration — the harness checks this is far below 10^9.
                Increment();
            }
            Trace.Log("thread-end", "Worker ended cleanly");
        });
        worker.Name = "worker";
        worker.Start();

        // TODO: sleep 100 ms, set the stop flag, then Join.
    }

    public static void Increment() => Count++;
}
