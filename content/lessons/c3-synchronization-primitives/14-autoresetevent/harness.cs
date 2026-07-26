using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var waiter = Task.Run(() => { Solution.Evt.WaitOne(); return "pass"; });
        await Task.Delay(50);
        await Solution.SignalAndWaitAsync();
        var finished = await Task.WhenAny(waiter, Task.Delay(3000));
        result.Add(
            name: "waiter-released",
            passed: finished == waiter && waiter.Result == "pass",
            expected: "Waiter should pass after Set()",
            actual: finished == waiter ? "pass" : "timed out",
            message: finished != waiter ? "Did you call Evt.Set()?" : "");
        return result;
    }
}
