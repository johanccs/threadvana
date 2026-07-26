using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? start slow operation (5s) with a 2s timeout");
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        var work = SlowTaskAsync(cts.Token);
        var delay = Task.Delay(2000);

        Trace.Log("async-suspend", "? Task.WhenAny — racing work vs timeout");
        var winner = await Task.WhenAny(work, delay);

        if (winner == delay)
        {
            cts.Cancel();
            Trace.Log("async-resume", "? timeout WON — CancellationToken fires");
            Trace.Log("message", "Timeout pattern: race work against Task.Delay + cancel on timeout. Work stops cooperatively.");
        }
    }

    private static async Task SlowTaskAsync(CancellationToken ct)
    {
        for (var i = 0; i < 10; i++) { ct.ThrowIfCancellationRequested(); await Task.Delay(500, ct); }
    }
}