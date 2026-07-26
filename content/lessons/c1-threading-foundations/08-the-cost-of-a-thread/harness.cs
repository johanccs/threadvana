using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        Solution.Run(); // nothing actually runs - this exercise is about the number

        int estimate = Solution.EstimateStackMemoryMb(Solution.ThreadCount);

        var result = new HarnessResult();

        result.Add(
            name: "you-picked-a-number",
            passed: Solution.ThreadCount > 0,
            expected: "ThreadCount is set to your answer",
            actual: $"ThreadCount = {Solution.ThreadCount}",
            message: "No answer yet. Set ThreadCount to the LARGEST number of threads whose stack " +
                     "estimate fits the 512 MB budget. Each thread costs ~1 MB of stack.");

        result.Add(
            name: "within-budget",
            passed: estimate <= 512,
            expected: "the stack estimate for your number is at most 512 MB",
            actual: $"EstimateStackMemoryMb({Solution.ThreadCount}) = {estimate} MB",
            message: "Over budget! Each thread reserves ~1 MB of stack, so N threads cost ~N MB. " +
                     "How many megabytes are in the budget - and how many threads is that at 1 MB each?");

        result.Add(
            name: "largest-possible",
            passed: Solution.ThreadCount == 512,
            expected: "the LARGEST count that fits: 512 (512 x 1 MB = 512 MB exactly)",
            actual: $"ThreadCount = {Solution.ThreadCount}",
            message: Solution.ThreadCount < 512
                ? "That fits, but it is not the LARGEST - you are leaving desks empty. " +
                  "512 MB budget, 1 MB per thread: how many fit exactly?"
                : "One thread too many tips it over: 513 x 1 MB = 513 MB, over budget. " +
                  "The largest that still fits is 512.");

        await Task.CompletedTask;
        return result;
    }
}
