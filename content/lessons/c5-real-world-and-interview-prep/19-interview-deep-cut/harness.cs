using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.Answer();
        result.Add("review", answer == "ok", "ok", answer, "");
        return result;
    }
}
