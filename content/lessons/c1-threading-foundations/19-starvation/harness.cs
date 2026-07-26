using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Run();
        await Task.Delay(50);

        var result = new HarnessResult();

        result.Add(
            name: "greedy-ran",
            passed: Solution.GreedyRuns == 5,
            expected: "the greedy worker completed 5 chunks of work while holding the lock",
            actual: $"GreedyRuns = {Solution.GreedyRuns}",
            message: "The greedy worker should always finish 5 chunks of work. Check that it's started and joined.");

        result.Add(
            name: "starved-long-enough",
            passed: Solution.StarvingWaitedMs >= Solution.MinWaitMs && Solution.MinWaitMs > 0,
            expected: $"the starving worker waited at least {Solution.MinWaitMs} ms",
            actual: $"waited {Solution.StarvingWaitedMs} ms, MinWaitMs = {Solution.MinWaitMs}",
            message: "The greedy worker holds the lock 5 × 50ms = 250ms minimum. Set MinWaitMs = 250.");

        result.Add(
            name: "starving-got-the-lock",
            passed: Solution.StarvingWaitedMs > 0,
            expected: "the starving worker eventually got the lock",
            actual: $"Starving waited {Solution.StarvingWaitedMs} ms",
            message: "The starving worker never got the lock. Are both threads joined?");

        return result;
    }
}
