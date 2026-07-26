using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.CheckContextAsync();
        result.Add(
            name: "detects-console-context",
            passed: answer == "none none",
            expected: "\"none none\" — the sandbox is a console app, context is always null",
            actual: $"\"{answer}\"",
            message: answer != "none none"
                ? "In this sandbox, SynchronizationContext.Current is null before and after await — check your logic."
                : "");
        return result;
    }
}
