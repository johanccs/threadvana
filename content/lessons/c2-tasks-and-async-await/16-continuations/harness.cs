using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.ChainWithContinueWithAsync();
        result.Add(
            name: "returns-doubled-value",
            passed: answer == 20,
            expected: "ContinueWith should double 10 → 20",
            actual: $"Got {answer}",
            message: answer != 20
                ? "Chain 10 through ContinueWith(t => t.Result * 2) and return the resulting Task<int>."
                : "");
        return result;
    }
}
