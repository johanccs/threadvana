using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Counter = 0;
        using var cts = new CancellationTokenSource(200);
        await Solution.RunUntilCancelledAsync(cts.Token);
        result.Add("loop-ran", Solution.Counter > 0, "counter > 0", $"{Solution.Counter}",
            Solution.Counter == 0 ? "The loop should increment before the token fires." : "");
        return result;
    }
}
