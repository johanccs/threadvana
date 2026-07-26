using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Logged = 0;
        await Solution.WriteLogAsync("test1");
        await Solution.WriteLogAsync("test2");
        await Task.Delay(100);
        result.Add("logged", Solution.Logged >= 2, ">=2", $"{Solution.Logged}", "");
        return result;
    }
}
