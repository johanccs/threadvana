using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("thread-start", "Main thread starts");

        // For the demo we cap the pool at 2 workers, so you can SEE the
        // reuse: 6 tasks will be handled by just 2 borrowed workers.
        // (A real pool sizes itself - we shrink it here on purpose.)
        ThreadPool.SetMinThreads(2, 2);
        ThreadPool.SetMaxThreads(2, 2);
        Trace.Log("message", "Pool capped at 2 workers (demo only) - 6 tasks coming up");

        // A countdown so we know when ALL tasks are done. Pool threads are
        // background - we cannot Join them, so we wait on this instead.
        using var done = new CountdownEvent(6);

        for (int orderId = 1; orderId <= 6; orderId++)
        {
            int id = orderId; // each task gets its OWN copy (remember the trap!)
            Trace.Log("pool-queued", "Task " + id + " handed to the pool");
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Trace.Log("pool-dequeued", "A pool worker picked up task " + id);
                Trace.Log("work-start", "Task " + id + " is being handled");
                Thread.Sleep(300); // pretend to work
                Trace.Log("work-end", "Task " + id + " done - worker goes back on call");
                done.Signal(); // one more task finished
            });
        }

        Trace.Log("message", "All 6 tasks queued - watch how FEW workers do them all");

        Trace.Log("wait-start", "Main waits until all 6 tasks signal done");
        done.Wait(); // blocks until the countdown reaches zero
        Trace.Log("wait-end", "All 6 tasks done");

        Trace.Log("message", "6 tasks handled by only 2 workers - that is reuse!");
        Trace.Log("thread-end", "Main thread ends");

        await Task.CompletedTask;
    }
}
