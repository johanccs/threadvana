using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var shouldStop = false;

        var worker = new Thread(() =>
        {
            Trace.Log("thread-start", "Worker starts");
            for (var i = 0; i < 10 && !shouldStop; i++)
            {
                Trace.Log("work-start", $"Working... (cycle {i + 1}/10)");
                Thread.Sleep(200);
                Trace.Log("work-end", $"Finished cycle {i + 1}");

                // The worker checks the flag after every cycle.
                if (i == 3) // already checked next iteration — the main will signal
                    Trace.Log("message", "Worker checks: should I stop? Not yet.");
            }
            Trace.Log("message", "Worker sees the stop flag — cleaning up");
            Trace.Log("thread-end", "Worker ends cleanly");
        });
        worker.Name = "worker";
        Trace.Log("thread-start", "Main thread starts");
        worker.Start();

        Trace.Log("message", "Main thread lets the worker run a bit...");
        Thread.Sleep(700);

        Trace.Log("message", "Main sets the stop flag");
        Volatile.Write(ref shouldStop, true);

        Trace.Log("wait-start", "Main waits for clean exit (Join)");
        worker.Join();
        Trace.Log("wait-end", "Worker finished");

        Trace.Log("thread-end", "Main thread ends");
        await Task.CompletedTask;
    }
}
