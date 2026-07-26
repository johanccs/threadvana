using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.SetAndReadContextAsync();
        result.Add(
            name: "context-survives-await",
            passed: answer == "hello",
            expected: "\"hello\" — AsyncLocal should carry the value across Task.Yield",
            actual: $"\"{answer}\"",
            message: answer != "hello" ? "Set Context.Value before the await and return it after." : "");
        return result;
    }
}
