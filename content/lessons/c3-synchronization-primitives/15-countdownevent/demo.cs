using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly CountdownEvent _done = new(5);

    public static async Task RunAsync()
    {
        Trace.Log("message", "5 workers, coordinator waits on CountdownEvent");
        var workers = new Task[5];
        for (var i = 0; i < 5; i++)
        {
            var idx = i;
            workers[i] = Task.Run(() =>
            {
                Trace.Log("work-start", $"Worker {idx}");
                Thread.Sleep(100 + idx * 50);
                Trace.Log("work-end", $"Worker {idx} signals");
                _done.Signal();
            });
        }
        var coordinator = Task.Run(() =>
        {
            _done.Wait();
            Trace.Log("message", "All workers done — coordinator released");
        });
        await Task.WhenAll(workers);
        await coordinator;
    }
}
