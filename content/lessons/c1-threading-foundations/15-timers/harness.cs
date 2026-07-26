using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Done = false;
        Solution.Run();

        var result = new HarnessResult();

        result.Add(
            name: "timer-fired",
            passed: Solution.Done,
            expected: "the timer fired and set Done = true",
            actual: $"Done = {Solution.Done}",
            message: "Your timer did not fire (or fire before Run() returned). " +
                     "Check that you: (a) created a Timer, (b) waited until Done is true.");

        result.Add(
            name: "run-returned",
            passed: true,
            expected: "Run() returned",
            actual: "Run() returned",
            message: "");

        Solution.Done = false;
        return result;
    }
}
