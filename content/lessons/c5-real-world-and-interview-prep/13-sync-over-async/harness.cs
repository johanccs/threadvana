using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = Solution.CallAsyncFromSync();
        result.Add("sync-call-works", answer == "fetched-data", "fetched-data", answer, "");
        return result;
    }
}
