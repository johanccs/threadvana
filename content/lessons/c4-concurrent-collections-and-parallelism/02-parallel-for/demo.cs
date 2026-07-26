using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        const int n = 5_000_000;
        var results = new long[n];
        var sw = Stopwatch.StartNew();

        // Sequential
        for (var i = 0; i < n; i++)
            results[i] = (long)i * i;
        Trace.Log("message", $"Sequential: {sw.ElapsedMilliseconds} ms");

        // Parallel
        sw.Restart();
        Parallel.For(0, n, i =>
        {
            results[i] = (long)i * i;
        });
        Trace.Log("message", $"Parallel.For: {sw.ElapsedMilliseconds} ms");

        Trace.Log("message", "Compare the times — Parallel is faster on multiple cores.");
        await Task.CompletedTask;
    }
}
