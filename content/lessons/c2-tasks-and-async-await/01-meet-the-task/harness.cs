using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        // Awaiting RunAsync is itself the proof that it hands back a Task
        // we can await - no .Result / .Wait() tricks are needed to get here.
        await Solution.RunAsync();

        var result = new HarnessResult();

        result.Add(
            name: "something-happened",
            passed: Solution.Result != 0,
            expected: "Result holds the number FetchNumber() delivered",
            actual: $"Result = {Solution.Result}",
            message: "Result is still 0, so nothing was stored. Most likely cause: the task was never started or never awaited. " +
                     "Try: Result = await Task.Run(() => FetchNumber());");

        result.Add(
            name: "collected-the-42",
            passed: Solution.Result == 42,
            expected: "Result == 42, the number FetchNumber() delivers",
            actual: $"Result = {Solution.Result}",
            message: "Result is set, but not to 42. Store exactly what the await hands back - the int that comes OUT of the task, " +
                     "not the task itself and not a number you typed in.");

        return result;
    }
}