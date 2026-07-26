using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var task = Solution.RunDeadlockFree();
        var finished = await Task.WhenAny(task, Task.Delay(10_000));
        result.Add(
            name: "no-deadlock",
            passed: finished == task && task.Result == "safe",
            expected: "Should return \"safe\" without deadlocking",
            actual: finished == task ? $"\"{task.Result}\"" : "timed out (deadlock?)",
            message: finished != task
                ? "The method timed out — are both threads stuck in circular lock wait? Lock in the same order."
                : task.Result != "safe"
                    ? "Return \"safe\" when the work finishes."
                    : "");
        return result;
    }
}
