using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        Solution.Run();

        // THE lesson check reads the counter IMMEDIATELY when Run() returns.
        // (An un-waited counter cannot win this race: the first tick needs
        // 50ms, returning from Run() takes ~1ms.)
        int counterOnReturn = Solution.Counter;

        // Grace (only when needed) so we can tell "never even started" apart
        // from "started, but nobody waited for it". The full count is
        // 10 ticks x 50ms = ~500ms; 1200ms is generous.
        if (Solution.Counter < 10)
            await Task.Delay(1200);
        int counterEventually = Solution.Counter;

        var result = new HarnessResult();

        result.Add(
            name: "counting-started",
            passed: counterEventually > 0,
            expected: "CountToTen() runs and Counter starts going up",
            actual: $"Counter reached {counterEventually}",
            message: "The counter never moved. Most likely cause: the thread was created but never " +
                     "Started. Check that Start() is still there after your changes.");

        result.Add(
            name: "finished-before-return",
            passed: counterOnReturn == 10,
            expected: "Counter is ALREADY 10 at the moment Run() returns",
            actual: $"Counter was {counterOnReturn} when Run() returned (it later reached {counterEventually})",
            message: "Run() returned before the counting finished. Most likely cause: nobody waited for " +
                     "the worker thread. Note that IsBackground only decides whether the PROCESS may exit " +
                     "- it never makes Run() wait (and removing it is not enough either). " +
                     "Add worker.Join() as the last line of Run().");

        return result;
    }
}
