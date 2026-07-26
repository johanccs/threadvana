using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Count = 0;

        Solution.Run();
        await Task.Delay(100);

        var result = new HarnessResult();

        result.Add(
            name: "stopped-early",
            passed: Solution.Count > 0 && Solution.Count < 200_000_000,
            expected: "the worker stopped before reaching 200 million (the flag worked)",
            actual: $"Count = {Solution.Count}",
            message: "The worker never stopped (or never started). Add a volatile bool flag " +
                     "to the loop condition, and set it from the main thread.");

        result.Add(
            name: "did-some-work",
            passed: Solution.Count > 0,
            expected: "the worker ran at least some iterations before stopping",
            actual: $"Count = {Solution.Count}",
            message: "The worker did not even start. Check that you call worker.Start().");

        // Reset for good hygiene.
        Solution.Count = 0;

        return result;
    }
}
