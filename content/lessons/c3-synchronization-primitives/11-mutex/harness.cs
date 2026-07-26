using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var t1 = Task.Run(() => Solution.TryAcquireSingleInstanceAsync());
        var t2 = Task.Run(() => Solution.TryAcquireSingleInstanceAsync());
        await Task.WhenAll(t1, t2);
        var answers = new[] { t1.Result, t2.Result };
        var hasFirst = answers[0] == "first" || answers[1] == "first";
        var hasSecond = answers[0] == "second" || answers[1] == "second";
        result.Add(
            name: "detects-first-and-second",
            passed: hasFirst && hasSecond,
            expected: "One \"first\" and one \"second\"",
            actual: $"\"{answers[0]}\", \"{answers[1]}\"",
            message: !hasFirst || !hasSecond
                ? "Check the createdNew flag — one call should own the named mutex, the other not."
                : "");
        return result;
    }
}
