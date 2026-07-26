using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Counter = 0;
        var tasks = new Task[4];
        for (var i = 0; i < 4; i++)
            tasks[i] = Task.Run(() => Solution.IncrementWithSpinLock());
        await Task.WhenAll(tasks);
        result.Add(
            name: "counter-is-4",
            passed: Solution.Counter == 4,
            expected: "4 increments → Counter = 4",
            actual: $"Counter = {Solution.Counter}",
            message: Solution.Counter != 4 ? "SpinLock not serialising increments correctly." : "");
        return result;
    }
}

