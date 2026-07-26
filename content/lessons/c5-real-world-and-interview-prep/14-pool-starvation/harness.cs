using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.StarveThePoolAsync();
        result.Add("starved", answer == "starved", "starved", answer, "");
        return result;
    }
}
