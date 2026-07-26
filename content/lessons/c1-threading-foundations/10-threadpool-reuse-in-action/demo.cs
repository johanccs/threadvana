using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly object _gate = new object();
    private static readonly HashSet<int> _workers = new HashSet<int>();

    public static async Task RunAsync()
    {
        Trace.Log("thread-start", "Main thread starts");

        // Cap the pool at 4 workers for the demo, so the reuse is visible
        // no matter how many CPU cores this machine has.
        // (Min lowered first, otherwise the Max call can refuse.)
        ThreadPool.SetMinThreads(1, 1);
        ThreadPool.SetMaxThreads(4, 4);
        ThreadPool.SetMinThreads(4, 4);
        Trace.Log("message", "Pool capped at 4 workers (demo only) - 10 small tasks coming up");

        using var done = new CountdownEvent(10);

        for (int taskId = 1; taskId <= 10; taskId++)
        {
            int id = taskId; // own copy per task (remember the trap!)
            Trace.Log("pool-queued", "Task " + id + " handed to the pool");
            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (_gate) _workers.Add(Environment.CurrentManagedThreadId);
                Trace.Log("pool-dequeued", "A pool worker picked up task " + id);
                Trace.Log("work-start", "Task " + id + " handled");
                Thread.Sleep(150); // pretend to work
                Trace.Log("work-end", "Task " + id + " done - worker goes back on call");
                done.Signal();
            });
        }

        Trace.Log("wait-start", "Main waits until all 10 tasks signal done");
        done.Wait();
        Trace.Log("wait-end", "All 10 tasks done");

        int distinct;
        lock (_gate) distinct = _workers.Count;
        Trace.Log("message", "10 tasks done by only " + distinct + " distinct workers - borrow and return!");
        Trace.Log("message", "Compare lesson 8: 200 dedicated threads (200 MB!) for even less work.");
        Trace.Log("thread-end", "Main thread ends");

        await Task.CompletedTask;
    }
}
