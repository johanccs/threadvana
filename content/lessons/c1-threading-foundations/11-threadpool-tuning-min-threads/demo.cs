using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        const int count = 12;

        // ---- Phase 1: default ramp-up ----
        Trace.Log("message", $"Phase 1 — queueing {count} blocking tasks with default min threads");
        var phase1Running = 0;
        var phase1Started = 0;
        for (var i = 0; i < count; i++)
        {
            var id = i;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var start = Interlocked.Increment(ref phase1Running);
                if (start == 1)
                    Trace.Log("pool-dequeued", $"Worker picks task {id}");
                Trace.Log("work-start", $"Phase 1 task {id}");
                Thread.Sleep(100); // quick work
                Trace.Log("work-end", $"Phase 1 task {id}");
                Interlocked.Decrement(ref phase1Running);
            });
            Trace.Log("pool-queued", $"Task {id} queued (phase 1)");
            Interlocked.Increment(ref phase1Started);
        }

        // Wait until all phase 1 tasks are done.
        while (phase1Started < count || phase1Running > 0) await Task.Delay(50);

        // ---- Phase 2: warm pool ----
        Trace.Log("message", $"Phase 2 — SetMinThreads(12,12) then queue {count} tasks");
        ThreadPool.SetMinThreads(12, 12);
        var phase2Running = 0;
        var phase2Started = 0;
        for (var i = 0; i < count; i++)
        {
            var id = i;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var start = Interlocked.Increment(ref phase2Running);
                if (start == 1)
                    Trace.Log("pool-dequeued", $"Worker picks task {id}");
                Trace.Log("work-start", $"Phase 2 task {id}");
                Thread.Sleep(100);
                Trace.Log("work-end", $"Phase 2 task {id}");
                Interlocked.Decrement(ref phase2Running);
            });
            Trace.Log("pool-queued", $"Task {id} queued (phase 2)");
            Interlocked.Increment(ref phase2Started);
        }

        while (phase2Started < count || phase2Running > 0) await Task.Delay(50);

        // Clean up — reset to default so we don't affect other lessons.
        ThreadPool.SetMinThreads(1, 1);
        Trace.Log("message", "Demo done — min threads reset to 1.");
        await Task.CompletedTask;
    }
}
