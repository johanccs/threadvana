using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var queue = new ConcurrentQueue<string>();
        var signal = new ManualResetEventSlim(false);

        var consumer = new Thread(() =>
        {
            Trace.Log("thread-start", "Consumer starts");
            while (true)
            {
                if (queue.TryDequeue(out var work))
                {
                    Trace.Log("work-start", $"Processing '{work}'");
                    Thread.Sleep(150);
                    Trace.Log("work-end", $"Done '{work}'");
                }
                else if (signal.IsSet && queue.IsEmpty) break;
                else Thread.Sleep(5);
            }
            Trace.Log("thread-end", "Consumer stops");
        });
        consumer.Name = "consumer";
        consumer.Start();

        // Producer
        foreach (var item in new[] { "log-1", "log-2", "log-3", "done" })
        {
            Trace.Log("pool-queued", $"Enqueue '{item}'");
            queue.Enqueue(item);
            Thread.Sleep(100);
        }
        Trace.Log("message", "Producer done — signalling");
        signal.Set();

        consumer.Join();
        Trace.Log("message", "All work finished");
        await Task.CompletedTask;
    }
}
