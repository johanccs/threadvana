using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.PipeDataAsync();
        result.Add("sum-is-21", answer == "21", "21 (1+2+3+4+5+6)", answer,
            answer != "21" ? "Pipe 1..6 through a bounded channel, sum, return." : "");
        return result;
    }
}
