using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        // Start clean (the counters are plain statics).
        Solution.InsideNow = 0;
        Solution.MaxInside = 0;
        Solution.Completed = 0;

        // Fire 6 calls at the same time and watch the counters.
        var calls = new Task[6];
        for (int i = 0; i < 6; i++)
            calls[i] = Solution.CallApiLimitedAsync();

        // Race the 6 calls against a deadline. If a Release() is missing, the
        // queue never moves and the calls never all finish - we report that
        // instead of hanging. (WhenAny: you met it in Task.WhenAll!)
        var all = Task.WhenAll(calls);
        var winner = await Task.WhenAny(all, Task.Delay(3000));
        bool allFinished = winner == all;

        var result = new HarnessResult();

        result.Add(
            name: "calls-started",
            passed: Solution.MaxInside > 0,
            expected: "at least one API call started (the counters saw it)",
            actual: $"MaxInside = {Solution.MaxInside}, Completed = {Solution.Completed}",
            message: "No call ever started. CallApiLimitedAsync() should call the provided CallApiAsync() - " +
                     "through your semaphore, with await Lot.WaitAsync() in front of it.");

        result.Add(
            name: "all-six-completed",
            passed: allFinished && Solution.Completed == 6,
            expected: "all 6 calls finish (Completed == 6)",
            actual: $"Completed = {Solution.Completed} of 6{(allFinished ? "" : " - and the rest were STILL QUEUED after 3 s")}",
            message: "Some calls never finished. Classic cause: a missing Release() - the first calls took the spaces " +
                     "and never gave them back, so the rest queued forever. Wrap the call in try/finally and put " +
                     "Lot.Release() in the finally.");

        result.Add(
            name: "at-most-two-inside",
            passed: Solution.MaxInside <= 2 && Solution.MaxInside > 0,
            expected: "never more than 2 calls inside at the same moment",
            actual: $"highest overlap seen: {Solution.MaxInside}",
            message: "Too many calls overlapped - nobody took a ticket at the entrance. Create the lot " +
                     "(public static SemaphoreSlim Lot = new SemaphoreSlim(2);) and AWAIT Lot.WaitAsync() " +
                     "before calling CallApiAsync().");

        result.Add(
            name: "really-parallel",
            passed: Solution.MaxInside >= 2,
            expected: "at least 2 calls overlapped at some point (a parking lot, not a queue of one)",
            actual: $"highest overlap seen: {Solution.MaxInside}",
            message: "The calls ran strictly one at a time - that is a LOCK, not a semaphore! " +
                     "Did you write new SemaphoreSlim(1)? Give the lot 2 spaces: new SemaphoreSlim(2).");

        return result;
    }
}