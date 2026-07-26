using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Run();
        await Task.Delay(200);

        var result = new HarnessResult();

        result.Add(
            name: "all-items-processed",
            passed: Solution.ProcessedCount == 10,
            expected: "all 10 items were processed",
            actual: $"ProcessedCount = {Solution.ProcessedCount}",
            message: "Not all items were dequeued and processed. Check that your lock+dequeue " +
                     "logic actually dequeues items and calls ProcessItem.");

        result.Add(
            name: "threads-stopped",
            passed: Solution.ShouldStop, // was set by Run()
            expected: "the threads stopped cleanly (did not hang)",
            actual: "threads joined successfully",
            message: "");

        return result;
    }
}
