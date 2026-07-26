using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.AcquireAndReleaseAsync();
        result.Add("async-lock", answer == "locked", "locked", answer, "");
        return result;
    }
}
