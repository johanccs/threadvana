using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "Two workers: await (thread freed) vs .Result (thread BLOCKED)");

        var good = Task.Run(async () =>
        {
            Trace.Log("work-start", "await worker started");
            Trace.Log("async-suspend", "about to await - thread freed");
            await Task.Delay(600);
            Trace.Log("async-resume", "continuation resumed");
            Trace.Log("work-end", "await worker done");
        });

        await Task.Delay(100);

        var bad = Task.Run(() =>
        {
            Trace.Log("work-start", ".Result worker started");
            Trace.Log("result-blocking", "calling .Result - THREAD BLOCKED");
            var r = SlowMethod().GetAwaiter().GetResult();
            Trace.Log("result-unblocked", ".Result returned");
            Trace.Log("work-end", ".Result worker finally done");
        });

        await Task.WhenAll(good, bad);
        Trace.Log("message", "Done - .Result hogged its thread, await freed it");
    }

    private static async Task<string> SlowMethod() { await Task.Delay(500); return "data"; }
}