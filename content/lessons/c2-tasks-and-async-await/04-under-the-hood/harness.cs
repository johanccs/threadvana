using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Run();

        var result = new HarnessResult();

        result.Add(
            name: "correct-pauses",
            passed: Solution.Pauses == 2,
            expected: "the method pauses twice (2 awaits)",
            actual: $"Pauses = {Solution.Pauses}",
            message: "Count the await keywords. Each one is a potential pause point. MakeToastAsync has 2 awaits.");

        result.Add(
            name: "first-to-run",
            passed: Solution.FirstToRun == "StartToasting",
            expected: "StartToasting runs first (sync code before first await)",
            actual: $"FirstToRun = '{Solution.FirstToRun}'",
            message: "Async methods start synchronously! Everything BEFORE the first await runs immediately on the calling thread. The first await comes AFTER StartToasting().");

        result.Add(
            name: "after-first-await",
            passed: Solution.AfterFirstAwait == "AddJam",
            expected: "after the first await finishes, AddJamAsync runs next",
            actual: $"AfterFirstAwait = '{Solution.AfterFirstAwait}'",
            message: "After await SpreadButterAsync(), the state machine jumps to the NEXT line — which is await AddJamAsync().");

        return result;
    }
}
