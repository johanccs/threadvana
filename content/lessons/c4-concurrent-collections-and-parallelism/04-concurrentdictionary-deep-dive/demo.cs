using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var dict = new ConcurrentDictionary<string, int>();
        int sideEffectCalls = 0;

        Trace.Log("work-start", "Ten threads racing GetOrAdd on the same key");
        var tasks = new Task[10];
        for (var i = 0; i < 10; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                Trace.Log("thread-start", $"Worker enter");
                dict.GetOrAdd("pizza", _ =>
                {
                    Interlocked.Increment(ref sideEffectCalls);
                    Trace.Log("work-start", $"Factory invoked (call #{sideEffectCalls})");
                    Task.Delay(200).Wait();
                    Trace.Log("work-end", "Factory done");
                    return 99;
                });
            });
        }
        await Task.WhenAll(tasks);

        Trace.Log("message", $"GetOrAdd called the factory {sideEffectCalls} time(s) — but only one value ({dict["pizza"]}) is in the dictionary");

        // Lazy<T> fixes it
        Trace.Log("work-start", "Now with Lazy<T> wrapper — at most one factory runs");
        int lazyCalls = 0;
        var safeDict = new ConcurrentDictionary<string, Lazy<string>>();
        var lazyWorkers = new Task[10];
        for (var i = 0; i < 10; i++)
        {
            lazyWorkers[i] = Task.Run(() =>
            {
                var lazy = safeDict.GetOrAdd("safe-key", _ => new Lazy<string>(() =>
                {
                    Interlocked.Increment(ref lazyCalls);
                    Trace.Log("work-start", $"Inside Lazy (call #{lazyCalls})");
                    Task.Delay(300).Wait();
                    return "only-once";
                }));
                // Discard computed result — we just care about call count.
                _ = lazy.Value;
            });
        }
        await Task.WhenAll(lazyWorkers);
        Trace.Log("message", $"Lazy factory ran {lazyCalls} time(s)");
    }
}
