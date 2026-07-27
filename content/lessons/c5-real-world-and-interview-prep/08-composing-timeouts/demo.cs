using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? create linked token: user-cancel + 1s timeout");
        using var userCts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(userCts.Token, timeoutCts.Token);

        Trace.Log("async-suspend", "? awaiting with linked token — either cancel source stops it");
        try { await Task.Delay(5000, linked.Token); }
        catch (OperationCanceledException)
        {
            Trace.Log("async-resume", "? operation cancelled — timeout (1s) won the race");
        }
        Trace.Log("message", "Linked CancellationToken: any source cancels the operation. Used for timeout + user-cancel in real APIs.");
    }
}