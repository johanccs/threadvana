using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        // Races are flaky: a lucky unprotected run can occasionally total 2000.
        // So we run 5 times and require EVERY run to be exact. With a correct
        // lock every run is exact, always. Without it, at least one run loses
        // increments - and the totals below show it.
        const int runs = 5;
        int exactRuns = 0;
        var totals = new int[runs];

        for (int r = 0; r < runs; r++)
        {
            Solution.Run();
            totals[r] = Solution.Counter;
            if (Solution.Counter == 2000) exactRuns++;
        }

        var result = new HarnessResult();

        result.Add(
            name: "threads-ran",
            passed: totals[0] != 0,
            expected: "the two threads ran and incremented Counter",
            actual: $"Counter after first run = {totals[0]}",
            message: "Counter is still 0, so no increment ever happened. Did you accidentally delete the three lines " +
                     "inside the loop? Keep them - just wrap them in  lock (Solution.Gate) { ... }.");

        result.Add(
            name: "exact-every-time",
            passed: exactRuns == runs,
            expected: $"Counter == 2000 on ALL {runs} runs (2 threads x 1000 increments)",
            actual: $"totals: {string.Join(", ", totals)} ({exactRuns}/{runs} exact)",
            message: "Increments are being lost to the read-add-write race. Wrap ALL THREE counter lines in " +
                     " lock (Solution.Gate) { ... } so only one thread can read-add-write at a time - " +
                     "and make sure BOTH threads use the same Gate key.");

        await Task.CompletedTask; // harness shape: ValidateAsync is async
        return result;
    }
}