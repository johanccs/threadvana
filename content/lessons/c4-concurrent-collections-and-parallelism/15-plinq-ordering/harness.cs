using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.EvenCount = 0;
        await Solution.ProcessWithForAll();
        result.Add("for-all-works", Solution.EvenCount == 2, "2 even numbers", $"{Solution.EvenCount}",
            Solution.EvenCount != 2 ? "Use .ForAll() with Interlocked.Add" : "");
        return result;
    }
}

