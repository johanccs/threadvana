using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        await Solution.EnqueueAsync(1, "low");
        await Solution.EnqueueAsync(3, "high");
        var first = await Solution.DequeueAsync();
        result.Add("priority-order", first == "high", "high first", first, "");
        return result;
    }
}
