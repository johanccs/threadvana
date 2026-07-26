using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();

        // Short timeout — should return "timeout".
        var shortResult = await Solution.RaceWithTimeoutAsync(300);
        result.Add(
            name: "short-timeout-cancels",
            passed: shortResult == "timeout",
            expected: "\"timeout\" when timeout is shorter than the 2-second work",
            actual: $"\"{shortResult}\"",
            message: shortResult != "timeout"
                ? "Real work takes 2 s; giving it a 300ms timeout should return \"timeout\". Did you check which task won?"
                : "");

        // Long timeout — should return "done".
        var longResult = await Solution.RaceWithTimeoutAsync(5000);
        result.Add(
            name: "long-timeout-finishes",
            passed: longResult == "done",
            expected: "\"done\" when timeout is longer than the work",
            actual: $"\"{longResult}\"",
            message: longResult != "done"
                ? "The real work should finish when given enough time. Check that you are returning RealWorkAsync's result."
                : "");

        return result;
    }
}
