using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var cores = Environment.ProcessorCount;
        Trace.Log("message", $"Machine has {cores} logical cores — capping at {cores - 1}");
        var opts = new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, cores - 1) };
        var tasks = new Task[6];
        for (var i = 0; i < 6; i++)
        {
            var idx = i;
            tasks[i] = Task.Run(() =>
            {
                Trace.Log("thread-start", $"Worker {idx}");
                Thread.Sleep(200);
            });
        }
        await Task.WhenAll(tasks);
        Trace.Log("message", $"Capped parallelism to {opts.MaxDegreeOfParallelism} — demo illustrates the concept");
    }
}
