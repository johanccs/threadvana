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
            name: "parallel-speedup",
            passed: Solution.ElapsedMs > 0 && Solution.ElapsedMs < 150,
            expected: "Parallel.For finishes 100 × 2ms works in under 150 ms",
            actual: $"ElapsedMs = {Solution.ElapsedMs}",
            message: "Replace the for loop with Parallel.For(0, 100, i => { SlowSquare(i); }) " +
                     "and measure the elapsed time with a Stopwatch.");

        result.Add(
            name: "measured-time",
            passed: Solution.ElapsedMs > 0,
            expected: "ElapsedMs was set (greater than 0)",
            actual: $"ElapsedMs = {Solution.ElapsedMs}",
            message: "Use Stopwatch.StartNew() around the loop and set ElapsedMs = sw.ElapsedMilliseconds.");

        return result;
    }
}
