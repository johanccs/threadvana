using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var outcome = await Solution.TryFetchAllAsync();
        result.Add(
            name: "reports-correct-error-count",
            passed: outcome == "error:1",
            expected: "\"error:1\" — exactly one fetch (id \"b\") should fail",
            actual: $"\"{outcome}\"",
            message: outcome != "error:1"
                ? outcome == "ok"
                    ? "Did you catch the exception? At least one fetch (id b) always fails."
                    : "Check that you count the inner exceptions correctly — Task.WhenAll's Exception.InnerExceptions has the count."
                : "");

        return result;
    }
}
