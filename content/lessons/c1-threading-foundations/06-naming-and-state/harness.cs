using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        Solution.Run();

        // Fair chance: if a thread was started but not Joined, give it a
        // moment to finish anyway so we can see what it recorded.
        await Task.Delay(500);

        var result = new HarnessResult();

        result.Add(
            name: "work-ran",
            passed: Solution.Ran,
            expected: "Work() runs (Ran becomes true)",
            actual: $"Ran = {Solution.Ran}",
            message: "Work() never ran. Did you create the thread AND call Start()? " +
                     "A thread that is only created never runs.");

        result.Add(
            name: "thread-has-a-name",
            passed: Solution.WorkerName != null,
            expected: "the worker thread has a name (Work recorded a non-null name)",
            actual: "recorded name = " +
                    (Solution.WorkerName == null ? "null (no name)" : "\"" + Solution.WorkerName + "\""),
            message: "The thread ran, but it had no name - Thread.CurrentThread.Name was null. " +
                     "Set worker.Name = \"data-worker\" right after creating the thread.");

        result.Add(
            name: "name-is-data-worker",
            passed: Solution.WorkerName == "data-worker",
            expected: "the thread's name is exactly \"data-worker\"",
            actual: "recorded name = " +
                    (Solution.WorkerName == null ? "null" : "\"" + Solution.WorkerName + "\""),
            message: "The name does not match exactly. Either it was never set (null), or the spelling " +
                     "differs - use exactly \"data-worker\" (all lowercase, with a hyphen). " +
                     "Debuggers and logs will thank you for exact names!");

        return result;
    }
}
