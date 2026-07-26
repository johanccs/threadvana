using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.CompleteAndDrainAsync();
        result.Add("drain-complete", answer == "hello world", "hello world", answer,
            answer != "hello world" ? "Write hello/world, Complete, drain with ReadAllAsync." : "");
        return result;
    }
}
