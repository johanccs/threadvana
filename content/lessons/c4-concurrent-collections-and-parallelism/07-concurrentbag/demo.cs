using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var bag = new ConcurrentBag<int>();
        var tasks = new Task[4];
        for (var i = 0; i < 4; i++)
        {
            var idx = i;
            tasks[i] = Task.Run(() =>
            {
                bag.Add(idx * 10);
                Thread.Sleep(50);
                if (bag.TryTake(out var val))
                    Trace.Log("work-end", $"Thread {idx} took {val}");
            });
        }
        await Task.WhenAll(tasks);
    }
}
