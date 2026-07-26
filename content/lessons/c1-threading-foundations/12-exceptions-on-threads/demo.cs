using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        // ---- Uncaught worker exception: don't actually let it go unhandled in a
        //      sandbox demo (it would kill the process). Show the SAME effect via a
        //      properly caught worker that reports its failure — the point is that
        //      the MAIN thread sees nothing of it unless we route the error. ----
        Trace.Log("thread-start", "Main thread starts");

        var workerError = "";
        var workerRan = false;

        var worker = new Thread(() =>
        {
            Trace.Log("thread-start", "Worker starts");
            try
            {
                Trace.Log("work-start", "Worker attempts risky work");
                throw new InvalidOperationException("the toaster caught fire!");
            }
            catch (Exception ex)
            {
                workerError = ex.Message;
                Trace.Log("message", $"Worker caught error: {workerError}");
            }
            workerRan = true;
            Trace.Log("thread-end", "Worker ends (after catching)");
        });
        worker.Name = "worker";
        worker.Start();

        Trace.Log("message", "Main thread keeps going — it has no idea the worker failed yet");
        Trace.Log("work-start", "Main does its own work");
        Thread.Sleep(500);
        Trace.Log("work-end", "Main done with its own work");

        Trace.Log("wait-start", "Main waits for worker (Join)");
        worker.Join();
        Trace.Log("wait-end", "Worker joined");

        Trace.Log("message", $"Main finally reads the error: '{workerError}'");
        Trace.Log("thread-end", "Main thread ends");
        await Task.CompletedTask;
    }
}
