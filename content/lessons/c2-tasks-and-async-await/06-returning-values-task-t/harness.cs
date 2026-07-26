using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();

        var sumTask = Solution.ComputeSumAsync();
        var finished = await Task.WhenAny(sumTask, Task.Delay(10_000));
        if (finished != sumTask)
        {
            result.Add(
                name: "returns-in-time",
                passed: false,
                expected: "ComputeSumAsync should finish within 10 seconds",
                actual: "It did not finish",
                message: "The method is hanging — check that you awaited all three tasks and returned the sum.");
            return result;
        }

        var sum = await sumTask; // Should be 10² + 20² + 30² = 100 + 400 + 900 = 1400
        var expected = 10 * 10 + 20 * 20 + 30 * 30;
        result.Add(
            name: "sum-is-correct",
            passed: sum == expected,
            expected: $"Sum should be {expected}",
            actual: $"Sum is {sum}",
            message: sum != expected
                ? $"Check that you passed 10, 20, 30 to ExpensiveCompute and summed all three results. Got {sum}."
                : "");

        return result;
    }
}
