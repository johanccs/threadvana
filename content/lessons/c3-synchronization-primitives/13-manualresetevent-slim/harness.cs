using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Gate.Reset();
        var w1 = Task.Run(() => { Solution.Gate.Wait(); return "w1"; });
        var w2 = Task.Run(() => { Solution.Gate.Wait(); return "w2"; });
        var toggle = Solution.OpenAndCloseGateAsync();
        var finished = await Task.WhenAny(Task.WhenAll(w1, w2), Task.Delay(5000));
        result.Add(
            name: "waiters-released",
            passed: finished.IsCompleted && w1.Result == "w1" && w2.Result == "w2",
            expected: "Both waiters should pass after Set()",
            actual: finished.IsCompleted ? "released" : "timed out",
            message: !finished.IsCompleted ? "Did you call Gate.Set()?" : "");
        await toggle;
        return result;
    }
}
