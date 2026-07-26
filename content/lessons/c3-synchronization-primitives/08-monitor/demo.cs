using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly object _gate = new();

    public static async Task RunAsync()
    {
        Trace.Log("message", "Two workers: one uses lock (blocks), one uses TryEnter (timeout)");

        var t1 = Task.Run(() =>
        {
            lock (_gate)
            {
                Trace.Log("work-start", "lock worker — holding for 800ms");
                Thread.Sleep(800);
                Trace.Log("work-end", "lock worker done");
            }
        });

        await Task.Delay(100);
        var t2 = Task.Run(() =>
        {
            Trace.Log("thread-start", "TryEnter worker — trying with 300ms timeout");
            if (Monitor.TryEnter(_gate, 300))
            {
                Trace.Log("work-end", "TryEnter got the lock");
                Monitor.Exit(_gate);
            }
            else
            {
                Trace.Log("message", "TryEnter gave up — lock was held");
            }
        });

        await Task.WhenAll(t1, t2);
    }
}
