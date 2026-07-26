using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var high = new ConcurrentQueue<string>();
        var low = new ConcurrentQueue<string>();
        high.Enqueue("H1"); low.Enqueue("L1"); high.Enqueue("H2");
        while (high.TryDequeue(out var h)) { Trace.Log("work-start", $"Dequeued HIGH: {h}"); await Task.Delay(50); }
        while (low.TryDequeue(out var l)) { Trace.Log("work-end", $"Dequeued LOW: {l}"); await Task.Delay(50); }
    }
}
