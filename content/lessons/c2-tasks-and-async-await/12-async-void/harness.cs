using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var task = Solution.LaunchWorkers();
        if (task == Task.CompletedTask)
        {
            // If the return type wasn't changed from void, this returns Task.CompletedTask
            // which is not useful — but the harness casts it to Task anyway.
            result.Add(
                name: "returns-task",
                passed: false,
                expected: "LaunchWorkers should return a Task (change async void to async Task)",
                actual: "Returned completed Task — likely still async void",
                message: "Change the return type from async void to async Task and return the workers' Task.");
            return result;
        }

        var finished = await Task.WhenAny(task, Task.Delay(5000));
        if (finished != task)
        {
            result.Add(
                name: "completes-in-time",
                passed: false,
                expected: "LaunchWorkers should finish within 5 seconds",
                actual: "Timed out",
                message: "The workers are stuck — check that you are awaiting or returning their Task.");
            return result;
        }

        await task;
        result.Add(
            name: "launch-workers-completes",
            passed: true,
            expected: "Workers launch and complete",
            actual: "Workers completed",
            message: "");

        return result;
    }
}
