using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);

        // Run twice to increase the chance of catching the race.
        Solution.Run();
        var lost1 = Solution.Lost;
        Solution.Run();
        var lost2 = Solution.Lost;

        var result = new HarnessResult();

        result.Add(
            name: "race-observed",
            passed: lost1 > 0 || lost2 > 0,
            expected: "the race lost at least some increments in 1 of 2 runs",
            actual: $"run 1 lost {lost1}, run 2 lost {lost2}",
            message: "No lost increments detected — but this race SHOULD lose some. " +
                     "Did you set Lost = 100000 - Counter after the threads finish AND " +
                     "reset Counter = 0 at the start of Run()?");

        result.Add(
            name: "lost-calculated",
            passed: lost1 >= 0 && lost2 >= 0 && (lost1 != 0 || lost2 != 0),
            expected: "Lost was set (not left at zero)",
            actual: $"Lost: {lost1}, {lost2}",
            message: "Calculate Lost = 100000 - Counter. Expected total is 100000, Counter is the actual.");

        return result;
    }
}
