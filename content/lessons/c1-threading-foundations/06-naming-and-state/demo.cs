using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("thread-start", "Main thread starts");

        var dataWorker = new Thread(() =>
        {
            Trace.Log("thread-start", "data-worker starts");
            Trace.Log("work-start", "data-worker crunches numbers");
            Thread.Sleep(400); // pretend to work
            Trace.Log("work-end", "data-worker done crunching");
            Trace.Log("thread-end", "data-worker ends");
        });
        dataWorker.Name = "data-worker"; // the swimlane gets this label!

        var logger = new Thread(() =>
        {
            Trace.Log("thread-start", "logger starts");
            Trace.Log("work-start", "logger writes a line");
            Thread.Sleep(150); // pretend to work
            Trace.Log("work-end", "logger done writing");
            Trace.Log("thread-end", "logger ends");
        });
        logger.Name = "logger"; // a name can only be set ONCE - set it here

        dataWorker.Start();
        logger.Start();

        // Peeks, not waits: read name + state from the outside.
        // (Run the demo twice - these peeks can honestly say different things!)
        Trace.Log("message", "Peek: " + dataWorker.Name + " IsAlive = " + dataWorker.IsAlive);
        Trace.Log("message", "Peek: logger state = " + logger.ThreadState);

        Trace.Log("wait-start", "Main waits for both named workers (Join)");
        dataWorker.Join();
        logger.Join();
        Trace.Log("wait-end", "Both workers done");

        // After a Join, the state is deterministic: Stopped.
        Trace.Log("message", "After Join: data-worker state = " + dataWorker.ThreadState);
        Trace.Log("thread-end", "Main thread ends");

        await Task.CompletedTask;
    }
}
