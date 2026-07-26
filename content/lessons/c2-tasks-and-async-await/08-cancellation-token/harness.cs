using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();

        // Test 1: short timeout → should cancel
        var cancelled = await Solution.ProcessWithTimeoutAsync(200);
        result.Add(
            name: "short-timeout-cancels",
            passed: cancelled == "cancelled",
            expected: "\"cancelled\" when timeout is shorter than the work (200ms vs 2000ms)",
            actual: $"\"{cancelled}\"",
            message: cancelled != "cancelled"
                ? $"SlowWorkAsync takes ~2000ms, but your timeout was only 200ms. Should return \"cancelled\"."
                : "");

        // Test 2: long timeout → should finish
        var finished = await Solution.ProcessWithTimeoutAsync(5000);
        result.Add(
            name: "long-timeout-finishes",
            passed: finished == "finished",
            expected: "\"finished\" when timeout is longer than the work (5000ms > 2000ms)",
            actual: $"\"{finished}\"",
            message: finished != "finished"
                ? $"SlowWorkAsync takes ~2000ms, and you gave it a 5000ms timeout. Should return \"finished\"."
                : "");

        return result;
    }
}
