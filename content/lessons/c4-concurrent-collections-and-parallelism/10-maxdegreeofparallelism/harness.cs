using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Counter = 0;
        await Solution.RunWithMaxParallelismAsync();
        result.Add(
            name: "counter-is-10",
            passed: Solution.Counter == 10,
            expected: "10 iterations → Counter = 10",
            actual: $"Counter = {Solution.Counter}",
            message: Solution.Counter != 10 ? "Use Parallel.For with MaxDegreeOfParallelism=2 and Interlocked.Increment." : "");
        return result;
    }
}
