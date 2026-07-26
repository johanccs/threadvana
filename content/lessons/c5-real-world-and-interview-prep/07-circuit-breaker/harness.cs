using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        // Fail twice (threshold=2), then circuit should be open.
        await Solution.CallWithCircuitBreakerAsync(() => throw new Exception(), 2);
        await Solution.CallWithCircuitBreakerAsync(() => throw new Exception(), 2);
        var status = await Solution.CallWithCircuitBreakerAsync(() => Task.FromResult("ok"), 2);
        result.Add("circuit-opens", status == "open", "open", status,
            status != "open" ? "After 2 failures with threshold=2, circuit should be open." : "");
        return result;
    }
}
