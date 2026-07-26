using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var mainThreadId = Environment.CurrentManagedThreadId;

        Solution.Run();

        // Fair chance: if the learner started a thread but forgot Join(),
        // give it a moment to finish anyway (Join is taught as good practice,
        // not checked here).
        await Task.Delay(500);

        var result = new HarnessResult();

        result.Add(
            name: "thread-ran",
            passed: Solution.WorkerThreadId != 0,
            expected: "WorkerThreadId is set by your new thread",
            actual: $"WorkerThreadId = {Solution.WorkerThreadId}",
            message: "Nothing set WorkerThreadId. Did you create the thread AND call Start()? " +
                     "A thread that is created but never started never runs.");

        result.Add(
            name: "really-a-new-thread",
            passed: Solution.WorkerThreadId != 0 && Solution.WorkerThreadId != mainThreadId,
            expected: "the work ran on a NEW thread (different id than the main one)",
            actual: $"main thread id = {mainThreadId}, worker id = {Solution.WorkerThreadId}",
            message: "The id matches the MAIN thread, so no new worker did the work. " +
                     "The assignment must happen INSIDE the new Thread's code.");

        return result;
    }
}
