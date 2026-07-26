using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? long operation starts — passes a CancellationToken");
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        Trace.Log("async-suspend", "? awaiting with cancellation — thread free");
        try { await Task.Delay(5000, cts.Token); }
        catch (OperationCanceledException)
        {
            Trace.Log("async-resume", "? cancelled after 2s — OperationCanceledException thrown");
        }
        Trace.Log("message", "CancellationToken: cooperative stop — the method checked and exited cleanly. No thread was killed.");
    }
}