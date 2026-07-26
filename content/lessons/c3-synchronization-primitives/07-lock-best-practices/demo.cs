using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static int _shared;
    private static readonly object _gate = new();

    public static async Task RunAsync()
    {
        // Good: private readonly lock object, short critical section.
        var tasks = new Task[4];
        for (var i = 0; i < 4; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                Trace.Log("thread-start", "Worker entering lock");
                lock (_gate)
                {
                    _shared++;
                    Trace.Log("work-start", $"Inside lock — _shared = {_shared}");
                    Task.Delay(100).Wait();
                }
                Trace.Log("work-end", "Worker leaving lock");
            });
        }
        await Task.WhenAll(tasks);
        Trace.Log("message", $"Final _shared = {_shared} (should be 4)");
    }
}
