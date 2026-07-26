using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Run();

        var result = new HarnessResult();

        result.Add(
            name: "s1-file-watcher",
            passed: Solution.S1 == "thread",
            expected: "for a permanent background worker, use 'thread'",
            actual: $"S1 = '{Solution.S1}'",
            message: "A file watcher that runs for the app's whole life should be a dedicated Thread. " +
                     "The thread pool is for short tasks — this one never stops.");

        result.Add(
            name: "s2-many-api-calls",
            passed: Solution.S2 == "async",
            expected: "for 500 API (I/O) calls, use 'async'",
            actual: $"S2 = '{Solution.S2}'",
            message: "Network calls are I/O-bound. Use async/await with Task.WhenAll. " +
                     "Never Task.Run for I/O — that just burns pool threads that sit idle.");

        result.Add(
            name: "s3-heavy-cpu-offload",
            passed: Solution.S3 == "pool",
            expected: "offload heavy CPU work to 'pool' (Task.Run)",
            actual: $"S3 = '{Solution.S3}'",
            message: "Heavy CPU work that must not freeze the UI should be offloaded " +
                     "via Task.Run or ThreadPool. async/await alone does not give you parallelism.");

        return result;
    }
}
