using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var sum = await Solution.ComputeParallelSumAsync();
        result.Add("plinq-sum", sum == 385, "385 (1²+...+10²)", $"{sum}",
            sum != 385 ? "Use .AsParallel().Select(n=>n*n).Sum()" : "");
        return result;
    }
}

