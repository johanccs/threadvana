using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        // PoliteWork() is fast and synchronous - just run it and count.
        Solution.PoliteWork();

        var result = new HarnessResult();

        result.Add(
            name: "work-started",
            passed: Solution.Count > 0,
            expected: "PoliteWork() runs the loop (Count starts going up)",
            actual: $"Count = {Solution.Count}",
            message: "The loop never ran. PoliteWork() must do the same 2000-round loop as BusyWork(), " +
                     "counting up as it goes. Is the loop still there?");

        result.Add(
            name: "work-finished",
            passed: Solution.Count == 2000,
            expected: "all 2000 iterations complete (Count == 2000)",
            actual: $"Count = {Solution.Count}",
            message: "The loop did not finish. Most likely cause: the loop bounds changed, or an early " +
                     "break or return snuck in. Keep the loop at 2000 rounds - only ADD pauses.");

        result.Add(
            name: "cpu-was-shared",
            passed: Solution.PauseCount >= 15,
            expected: "at least 15 polite pauses (one about every 100 iterations -> ~20 total)",
            actual: $"PauseCount = {Solution.PauseCount}",
            message: "Not enough pauses. Most likely cause: Pause() is missing, or it is called too " +
                     "rarely. Call Pause() every 100 iterations: if (i % 100 == 99) Pause();");

        await Task.CompletedTask;
        return result;
    }
}
