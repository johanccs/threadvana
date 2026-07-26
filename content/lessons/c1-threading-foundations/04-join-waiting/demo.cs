using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("thread-start", "Main thread starts");

        var worker = new Thread(() =>
        {
            Trace.Log("thread-start", "Worker starts");
            Trace.Log("work-start", "Worker does its job (600ms)");
            Thread.Sleep(600); // pretend to work
            Trace.Log("work-end", "Worker finished its job");
            Trace.Log("thread-end", "Worker ends");
        });
        worker.Name = "worker";

        worker.Start();

        // Main has its own little job to do first.
        Trace.Log("work-start", "Main does its own work first");
        Thread.Sleep(200);
        Trace.Log("work-end", "Main done with its own work");

        // IsAlive: a quick peek - is the worker still going? (no waiting)
        Trace.Log("message", "IsAlive = " + worker.IsAlive + " - the worker is still busy");

        // Join(timeout): wait, but give up after 150ms.
        Trace.Log("wait-start", "Main waits - but only up to 150ms (Join with timeout)");
        bool finishedInTime = worker.Join(150);
        Trace.Log("wait-end", "Join(150) returned " + finishedInTime + " - gave up for now");

        // Join(): wait as long as it takes.
        Trace.Log("wait-start", "Main waits as long as needed (Join)");
        worker.Join();
        Trace.Log("wait-end", "Worker is done - main continues");

        Trace.Log("message", "IsAlive = " + worker.IsAlive + " - the worker has finished");
        Trace.Log("thread-end", "Main thread ends");

        await Task.CompletedTask;
    }
}
