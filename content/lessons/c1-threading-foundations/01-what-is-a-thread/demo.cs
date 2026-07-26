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
            Trace.Log("work-start", "Worker is doing its job");
            Thread.Sleep(900); // pretend to work
            Trace.Log("work-end", "Worker finished its job");
            Trace.Log("thread-end", "Worker ends");
        });
        worker.Name = "worker";

        worker.Start();
        Trace.Log("message", "Main thread keeps going while the worker works");

        Trace.Log("work-start", "Main does its own work");
        Thread.Sleep(400);
        Trace.Log("work-end", "Main done with its own work");

        Trace.Log("wait-start", "Main waits for the worker (Join)");
        worker.Join();
        Trace.Log("wait-end", "Worker is done - main continues");

        Trace.Log("thread-end", "Main thread ends");
        await Task.CompletedTask;
    }
}
