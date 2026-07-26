using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var task = Solution.ProduceAndConsume();
        var finished = await Task.WhenAny(task, Task.Delay(10000));
        result.Add(
            name: "pipeline-completes",
            passed: finished == task && task.Result == "done",
            expected: "\"done\" within 10s",
            actual: finished == task ? $"\"{task.Result}\"" : "timed out",
            message: finished != task ? "Producer may be blocked — check capacity and CompleteAdding." : "");
        return result;
    }
}
