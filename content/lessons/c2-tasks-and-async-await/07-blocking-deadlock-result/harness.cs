using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var sumTask = Task.Run(async () => await Solution.FetchSumAsync());
        var finished = await Task.WhenAny(sumTask, Task.Delay(10_000));
        if (finished != sumTask)
        {
            result.Add(
                name: "returns-in-time",
                passed: false,
                expected: "FetchSumAsync should finish within 10 seconds",
                actual: "It timed out",
                message: "The method is hanging — check that you replaced every .Result with await.");
            return result;
        }

        var sum = await sumTask;
        result.Add(
            name: "correct-sum",
            passed: sum == 100,
            expected: "Sum should be 42 + 58 = 100",
            actual: $"Sum is {sum}",
            message: sum != 100 ? "The answer should be 100 — FetchA returns 42, FetchB returns 58." : "");

        return result;
    }
}
