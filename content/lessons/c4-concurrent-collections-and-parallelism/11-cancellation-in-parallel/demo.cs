using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        var opts = new ParallelOptions { CancellationToken = cts.Token };
        Trace.Log("message", "Parallel.ForEach with 500ms CancellationToken");
        try
        {
            await Task.Run(() =>
                Parallel.ForEach(Enumerable.Range(0, 20), opts, (i, state) =>
                {
                    opts.CancellationToken.ThrowIfCancellationRequested();
                    Trace.Log("work-start", $"Item {i}");
                    Thread.Sleep(100);
                }));
        }
        catch (OperationCanceledException)
        {
            Trace.Log("message", "Parallel loop cancelled gracefully");
        }
    }
}
