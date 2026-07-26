using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.RunCount = 0;
        Solution.WorkerName = "";
        Solution.Run();
        await Task.Delay(200);

        var result = new HarnessResult();

        result.Add(
            name: "name-set",
            passed: Solution.WorkerName == "data-worker",
            expected: "the thread was named 'data-worker'",
            actual: $"WorkerName = '{Solution.WorkerName}'",
            message: "Name the thread before starting it: worker.Name = \"data-worker\";");

        result.Add(
            name: "work-done",
            passed: Solution.RunCount == 100,
            expected: "the worker ran 100 iterations",
            actual: $"RunCount = {Solution.RunCount}",
            message: "Was the worker started AND joined? The provided DoWork() does 100 increments.");

        return result;
    }
}
