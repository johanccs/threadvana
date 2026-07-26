using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly ThreadLocal<Random> _rng = new(() => new Random(Guid.NewGuid().GetHashCode()));

    public static async Task RunAsync()
    {
        var results = new int[4];
        var tasks = new Task[4];
        for (var i = 0; i < 4; i++)
        {
            var idx = i;
            tasks[i] = Task.Run(() =>
            {
                Trace.Log("work-start", $"Thread {idx} getting its own Random");
                var val = _rng.Value.Next(100);
                results[idx] = val;
                Trace.Log("work-end", $"Thread {idx} rolled {val}");
            });
        }
        await Task.WhenAll(tasks);
        Trace.Log("message", $"Values: {string.Join(", ", results)} — each thread had its own Random, no race");
    }
}
