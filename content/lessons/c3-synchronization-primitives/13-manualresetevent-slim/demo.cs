using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly ManualResetEventSlim _gate = new(false);

    public static async Task RunAsync()
    {
        var t1 = Task.Run(() => { Trace.Log("thread-start", "Waiter 1 waiting"); _gate.Wait(); Trace.Log("work-end", "Waiter 1 passed"); });
        var t2 = Task.Run(() => { Trace.Log("thread-start", "Waiter 2 waiting"); _gate.Wait(); Trace.Log("work-end", "Waiter 2 passed"); });
        await Task.Delay(500);
        Trace.Log("message", "Set() — gate opens for both");
        _gate.Set();
        await Task.WhenAll(t1, t2);
        _gate.Reset();
        var t3 = Task.Run(() => { _gate.Wait(); Trace.Log("work-end", "Waiter 3 passed after Reset+Set"); });
        await Task.Delay(200);
        _gate.Set();
        await t3;
        _gate.Dispose();
    }
}
