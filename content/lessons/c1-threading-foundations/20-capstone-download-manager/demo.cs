using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var queue = new Queue<int>();
        var shouldStop = false;
        var gate = new object();
        var processed = 0;

        // Start 4 workers.
        var workers = new Thread[4];
        for (var w = 0; w < 4; w++)
        {
            var workerNum = w;
            workers[w] = new Thread(() =>
            {
                Trace.Log("thread-start", $"Worker {workerNum + 1}");
                while (true)
                {
                    int item = -1;
                    bool hasItem = false;

                    lock (gate)
                    {
                        if (queue.Count > 0)
                        {
                            item = queue.Dequeue();
                            hasItem = true;
                        }
                        else if (shouldStop) break;
                    }

                    if (hasItem)
                    {
                        Trace.Log("work-start", $"Worker {workerNum + 1} processing item {item}");
                        Thread.Sleep(150);
                        Interlocked.Increment(ref processed);
                        Trace.Log("work-end", $"Worker {workerNum + 1} done item {item}");
                    }
                    else Thread.Sleep(1);
                }
                Trace.Log("thread-end", $"Worker {workerNum + 1} stops");
            });
            workers[w].Name = $"worker-{workerNum + 1}";
        }

        foreach (var w in workers) w.Start();

        // Producer: enqueue 12 items.
        for (var i = 1; i <= 12; i++)
        {
            Trace.Log("pool-queued", $"Enqueue item {i}");
            lock (gate) queue.Enqueue(i);
            Thread.Sleep(30);
        }

        // Signal stop.
        Trace.Log("message", "Producer done — signalling stop");
        shouldStop = true;

        foreach (var w in workers) w.Join();
        Trace.Log("message", $"All done: {processed}/12 items processed");
        await Task.CompletedTask;
    }
}
