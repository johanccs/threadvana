using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        // Measure how long the learner's Run() takes on the wall clock.
        Exception runError = null;
        var clock = Stopwatch.StartNew();
        try
        {
            Solution.Run();
        }
        catch (Exception ex)
        {
            runError = ex; // e.g. Join() on a thread that was never Started
        }
        clock.Stop();

        // Fair chance: if threads were started but not Joined, give them a
        // moment to finish anyway so we can still see whether the jobs ran.
        await Task.Delay(700);

        var result = new HarnessResult();

        result.Add(
            name: "both-jobs-ran",
            passed: runError == null && Solution.JobARan && Solution.JobBRan,
            expected: "JobA and JobB both run (both flags set)",
            actual: $"JobARan = {Solution.JobARan}, JobBRan = {Solution.JobBRan}" +
                    (runError != null ? $", and Run() threw {runError.GetType().Name}" : ""),
            message: "At least one job never ran. Most likely cause: a thread was created but never " +
                     "Started, so its job never happened. Check that you call Start() on BOTH threads.");

        result.Add(
            name: "waited-for-both",
            passed: runError == null && clock.ElapsedMilliseconds >= 300,
            expected: "Run() returns only after both jobs are done (Join both threads)",
            actual: $"Run() returned after {clock.ElapsedMilliseconds}ms, but each job needs ~400ms",
            message: "Run() came back before the jobs could possibly be done. If you started threads, " +
                     "you forgot Join(). Start both threads, then Join BOTH before Run() ends.");

        result.Add(
            name: "faster-than-sequential",
            passed: runError == null && clock.ElapsedMilliseconds < 700,
            expected: "under 700ms total - two threads at the same time beat the ~800ms of one-after-another",
            actual: $"Run() took {clock.ElapsedMilliseconds}ms",
            message: "That is too slow - it looks one-after-another. Most likely cause: you called JobA() " +
                     "and JobB() directly, or you Joined the first thread before Starting the second. " +
                     "Start BOTH threads first, then Join both.");

        return result;
    }
}
