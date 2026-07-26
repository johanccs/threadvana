using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? 3 fetchers racing — Network A (fast), B (medium), C (slow)");

        var fast = FakeFetchAsync(300);
        var medium = FakeFetchAsync(600);
        var slow = FakeFetchAsync(900);

        Trace.Log("async-suspend", "? Task.WhenAny — awaits the FIRST to finish");
        var winner = await Task.WhenAny(fast, medium, slow);
        Trace.Log("async-resume", "? fastest finished — WhenAny returns immediately");

        Trace.Log("message", "WhenAny: respond to the fastest result. The other two are cancelled (or we could await them if needed).");
    }

    private static async Task FakeFetchAsync(int ms) { await Task.Delay(ms); return; }
}