using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        using var cts = new CancellationTokenSource();
        var answer = await Solution.CallWithLinkedCancellationAsync(cts.Token, 100);
        result.Add("timeout-wins", answer == "cancelled", "cancelled", answer,
            answer != "cancelled" ? "The 100ms timeout should fire before the 5000ms call." : "");
        return result;
    }
}
