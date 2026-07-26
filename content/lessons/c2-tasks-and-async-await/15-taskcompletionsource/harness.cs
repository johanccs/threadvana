using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var signal = Solution.WaitForSignalAsync();

        // Should not be completed yet.
        await Task.Delay(50);
        result.Add(
            name: "not-completed-before-trigger",
            passed: !signal.IsCompleted,
            expected: "Task should NOT be completed before Trigger() is called",
            actual: signal.IsCompleted ? "Already completed" : "Not yet completed",
            message: signal.IsCompleted
                ? "The Task is completing immediately — it should wait for Trigger()."
                : "");

        Solution.Trigger();
        await Task.WhenAny(signal, Task.Delay(5000));
        result.Add(
            name: "completes-after-trigger",
            passed: signal.IsCompleted,
            expected: "Task should complete after Trigger()",
            actual: signal.IsCompleted ? "Completed" : "Still waiting",
            message: signal.IsCompleted
                ? ""
                : "Trigger() was called but the task didn't complete — check TrySetResult.");

        return result;
    }
}
