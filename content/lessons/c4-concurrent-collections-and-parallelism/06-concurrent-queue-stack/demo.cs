using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var q = new ConcurrentQueue<string>();
        Trace.Log("message", "2 producers → ConcurrentQueue → 1 consumer");
        var prod1 = Task.Run(() => { for (var i = 0; i < 3; i++) { q.Enqueue($"A{i}"); Thread.Sleep(50); } });
        var prod2 = Task.Run(() => { for (var i = 0; i < 3; i++) { q.Enqueue($"B{i}"); Thread.Sleep(60); } });
        await Task.WhenAll(prod1, prod2);
        while (q.TryDequeue(out var item))
            Trace.Log("work-end", $"Dequeued: {item}");
    }
}
