using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        using var cts = new CancellationTokenSource(50);
        var answer = await Solution.RunCancellableLoopAsync(cts.Token);
        result.Add(
            name: "cancels-quickly",
            passed: answer == "cancelled",
            expected: "\"cancelled\" when token fires immediately",
            actual: $"\"{answer}\"",
            message: answer != "cancelled" ? "Catch OperationCanceledException and return \"cancelled\"." : "");
        return result;
    }
}
