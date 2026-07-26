using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Processed = 0;
        await Solution.ProcessItemsAsync();
        result.Add(
            name: "all-processed",
            passed: Solution.Processed == 4,
            expected: "4 items → Processed = 4",
            actual: $"Processed = {Solution.Processed}",
            message: Solution.Processed != 4 ? "Use Parallel.ForEachAsync and Interlocked.Increment." : "");
        return result;
    }
}
