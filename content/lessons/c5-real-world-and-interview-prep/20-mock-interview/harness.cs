using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.ReadyForInterview();
        result.Add("ready", answer == "ready", "ready", answer, "");
        return result;
    }
}
