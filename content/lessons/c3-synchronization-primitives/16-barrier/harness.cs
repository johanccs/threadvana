using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var task = Solution.RunPhasesAsync();
        var finished = await Task.WhenAny(task, Task.Delay(5000));
        result.Add(
            name: "phases-complete-in-time",
            passed: finished == task && task.Result == "phased",
            expected: "\"phased\" within 5s",
            actual: finished == task ? $"\"{task.Result}\"" : "timed out",
            message: finished != task ? "Workers may be stuck — check SignalAndWait." : "");
        return result;
    }
}
