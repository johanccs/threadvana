using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        const int n = 50_000_000;

        // 1. Inline (blocks)
        Trace.Log("work-start", "Inline sum (blocks main lane)");
        long sum = 0;
        for (var i = 0; i < n; i++) sum += i;
        Trace.Log("work-end", $"Inline done: sum = {sum}");

        // 2. Task.Run (offload to pool)
        Trace.Log("pool-queued", "Offloading sum to Task.Run");
        var tcs = new TaskCompletionSource<long>();
        Task.Run(() =>
        {
            Trace.Log("pool-dequeued", "Task.Run worker picks up the sum");
            Trace.Log("work-start", "Task.Run computing sum");
            long s = 0;
            for (var i = 0; i < n; i++) s += i;
            Trace.Log("work-end", $"Task.Run done: sum = {s}");
            tcs.SetResult(s);
        });
        Trace.Log("message", "Main lane is free — doing a quick nap");
        Thread.Sleep(100);
        Trace.Log("wait-start", "Main awaits the Task.Run result");
        var taskSum = await tcs.Task;
        Trace.Log("wait-end", $"Got result: sum = {taskSum}");

        // 3. Parallel.For (split across pool)
        Trace.Log("message", "Running Parallel.For (will use multiple pool workers)");
        long parallelSum = 0;
        var lockObj = new object();
        Parallel.For(0, n,
            () => 0L,
            (i, _, local) => local + i,
            local => { lock (lockObj) parallelSum += local; });
        Trace.Log("message", $"Parallel done: sum = {parallelSum}");

        await Task.CompletedTask;
    }
}
