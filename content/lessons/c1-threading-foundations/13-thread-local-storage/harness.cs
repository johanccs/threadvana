using System;
using System.Linq;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Run();
        await Task.Delay(200);

        // Reset for retries.
        var results = Solution.Results.ToArray();
        Solution.Results = new int[2];
        Solution.Counter = 0;

        var result = new HarnessResult();

        result.Add(
            name: "something-ran",
            passed: results.Any(r => r != 0),
            expected: "at least one thread stored a non-zero count",
            actual: $"results = [{results[0]}, {results[1]}]",
            message: "Neither thread stored a value. Did they start and run?");

        result.Add(
            name: "independent-counters",
            passed: results[0] == 1 && results[1] == 1,
            expected: "each thread sees count = 1 (its own copy, incremented once)",
            actual: $"results = [{results[0]}, {results[1]}]",
            message: "The threads saw the same counter — meaning it is still shared. " +
                     "Add [ThreadStatic] before the Counter field so each thread has its own.");

        return result;
    }
}
