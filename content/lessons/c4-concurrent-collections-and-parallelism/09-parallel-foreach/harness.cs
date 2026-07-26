using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var sum = await Solution.SumSquaresAsync();
        result.Add(
            name: "sum-of-squares",
            passed: sum == 55,
            expected: "1²+2²+3²+4²+5² = 55",
            actual: $"Sum = {sum}",
            message: sum != 55 ? "Use Parallel.ForEach with Interlocked.Add to compute the sum." : "");
        return result;
    }
}
