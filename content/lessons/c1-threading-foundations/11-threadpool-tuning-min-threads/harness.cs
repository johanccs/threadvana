using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(50);
        Solution.Run();
        await Task.Delay(100);

        var result = new HarnessResult();

        result.Add(
            name: "called-set-min-threads",
            passed: Solution.UsedSetMinThreads,
            expected: "you called ThreadPool.SetMinThreads before the burst",
            actual: Solution.UsedSetMinThreads ? "yes" : "no",
            message: "Call ThreadPool.SetMinThreads(Workers, Workers) BEFORE the burst code. " +
                     "This pre-warms the pool with at least N workers ready.");

        result.Add(
            name: "set-correct-worker-count",
            passed: Solution.Workers == 8 && Solution.UsedSetMinThreads,
            expected: "Workers = 8 AND SetMinThreads was called",
            actual: $"Workers = {Solution.Workers}, SetMinThreads = {Solution.UsedSetMinThreads}",
            message: "Set Workers to 8. That's how many workers you tell the pool to keep ready.");

        // Reset for future lessons.
        ThreadPool.SetMinThreads(1, 1);
        return result;
    }
}

