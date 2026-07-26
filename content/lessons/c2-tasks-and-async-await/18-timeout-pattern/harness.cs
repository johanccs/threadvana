using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();

        var shortResult = await Solution.RunWithTimeoutAsync(300);
        result.Add(
            name: "short-timeout-triggers",
            passed: shortResult == "timeout",
            expected: "\"timeout\" — 300ms is shorter than the 2000ms job",
            actual: $"\"{shortResult}\"",
            message: shortResult != "timeout"
                ? "With a 300ms timeout, the job should not finish — return \"timeout\"."
                : "");

        var longResult = await Solution.RunWithTimeoutAsync(5000);
        result.Add(
            name: "long-timeout-allows-completion",
            passed: longResult == "completed",
            expected: "\"completed\" — 5000ms gives the 2000ms job enough time",
            actual: $"\"{longResult}\"",
            message: longResult != "completed"
                ? "The job should finish when given enough time. Return \"completed\" when it wins the race."
                : "");

        return result;
    }
}
