using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly object _gateA = new();
    private static readonly object _gateB = new();

    public static async Task RunAsync()
    {
        Trace.Log("message", "Thread A: lock A then B. Thread B: lock A then B (consistent order — no deadlock).");

        var tasks = new Task[2];
        tasks[0] = Task.Run(() =>
        {
            Trace.Log("thread-start", "Thread A acquiring A...");
            if (Monitor.TryEnter(_gateA, 500))
            {
                try
                {
                    Trace.Log("work-start", "A got gateA");
                    Thread.Sleep(300);
                    if (Monitor.TryEnter(_gateB, 500))
                    {
                        Trace.Log("work-end", "A got both locks");
                        Monitor.Exit(_gateB);
                    }
                    else Trace.Log("message", "A timed out on B");
                }
                finally { Monitor.Exit(_gateA); }
            }
        });
        tasks[1] = Task.Run(() =>
        {
            Trace.Log("thread-start", "Thread B acquiring A...");
            if (Monitor.TryEnter(_gateA, 500))
            {
                try
                {
                    Trace.Log("work-start", "B got gateA");
                    Thread.Sleep(200);
                    if (Monitor.TryEnter(_gateB, 500))
                    {
                        Trace.Log("work-end", "B got both locks");
                        Monitor.Exit(_gateB);
                    }
                }
                finally { Monitor.Exit(_gateA); }
            }
        });
        await Task.WhenAll(tasks);
    }
}
