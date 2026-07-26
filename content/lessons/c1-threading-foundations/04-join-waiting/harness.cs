using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        Solution.Run();

        // THE lesson check happens FIRST, before any waiting:
        // was the flag already set at the exact moment Run() returned?
        // (Missing Join cannot win this race: returning takes ~1ms, the
        // provided Work() needs 300ms.)
        bool flagWhenRunReturned = Solution.Flag;

        // Defensive only: if the learner's thread exists and was started,
        // give it a bounded chance to finish (max 2s) so the "did anything
        // happen" check below can never flake on a slow machine. On an
        // already-finished thread this returns instantly. (Never Join an
        // Unstarted thread - that throws.)
        if (Solution.Worker != null && Solution.Worker.ThreadState != ThreadState.Unstarted)
            Solution.Worker.Join(2000);
        bool flagEventually = Solution.Flag;

        var result = new HarnessResult();

        result.Add(
            name: "work-ran",
            passed: flagEventually,
            expected: "Work() runs and sets Flag to true",
            actual: $"Flag = {flagEventually}" +
                    (Solution.Worker == null ? ", and no thread was stored in Solution.Worker" : ""),
            message: "The work never happened. Did you create a thread that runs Work AND call Start() " +
                     "on it? A thread that is only created never runs.");

        result.Add(
            name: "thread-was-used",
            passed: Solution.Worker != null && Solution.Worker.ThreadState != ThreadState.Unstarted,
            expected: "Work() runs on a NEW thread that you store in Solution.Worker",
            actual: Solution.Worker == null
                    ? "Solution.Worker is null - no thread was stored"
                    : $"thread state = {Solution.Worker.ThreadState}",
            message: "No started thread was stored. Most likely cause: you called Work() directly, forgot " +
                     "Start(), or forgot to keep the thread in Solution.Worker. " +
                     "Use: Worker = new Thread(Work); Worker.Start();");

        result.Add(
            name: "flag-set-on-return",
            passed: flagWhenRunReturned,
            expected: "Flag is ALREADY true at the moment Run() returns - guaranteed, not by luck",
            actual: $"Flag was {flagWhenRunReturned} when Run() returned (it became {flagEventually} later)",
            message: "The work ran, but Run() returned BEFORE it finished - the flag was still false at " +
                     "that moment. Most likely cause: missing Worker.Join(). Join means 'pause my thread " +
                     "until that thread is completely done'. Add it as the last line of Run().");

        await Task.CompletedTask;
        return result;
    }
}
