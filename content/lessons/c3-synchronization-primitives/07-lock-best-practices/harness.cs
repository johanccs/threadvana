using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Reset();
        var threads = new Thread[20];
        for (var t = 0; t < 20; t++)
        {
            threads[t] = new Thread(() =>
            {
                for (var i = 0; i < 5000; i++) Solution.Increment();
            });
            threads[t].Start();
        }
        for (var t = 0; t < 20; t++) threads[t].Join();

        result.Add(
            name: "no-race",
            passed: Solution.Counter == 100_000,
            expected: "20 threads × 5000 = 100,000",
            actual: $"Counter = {Solution.Counter}",
            message: Solution.Counter != 100_000
                ? $"Lost {100_000 - Solution.Counter} increments — make sure Increment is inside a lock."
                : "");

        Solution.Reset();
        result.Add(
            name: "reset-works",
            passed: Solution.Counter == 0,
            expected: "Counter should be 0 after Reset()",
            actual: $"Counter = {Solution.Counter}",
            message: Solution.Counter != 0 ? "Reset() should set Counter to 0 inside a lock." : "");

        return result;
    }
}
