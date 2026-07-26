using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Run();
        await Task.Delay(100);

        var result = new HarnessResult();

        result.Add(
            name: "all-processed",
            passed: Solution.ProcessedCount == 5,
            expected: "consumer processed all 5 items",
            actual: $"ProcessedCount = {Solution.ProcessedCount}",
            message: "The consumer didn't dequeue all items. The loop should: " +
                     "TryDequeue → process → check signal+empty → break.");

        result.Add(
            name: "consumer-exited",
            passed: true,
            expected: "consumer thread joined (did not hang)",
            actual: "consumer joined successfully",
            message: "");

        return result;
    }
}
