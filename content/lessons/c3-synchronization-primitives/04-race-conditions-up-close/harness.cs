using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        int expected = Solution.ThreadCount * Solution.IncrementsPerThread;

        // Give the race up to 3 fair chances to show itself.
        int bestTotal = 0;
        int returnedAtBest = 0;
        int counterAfterReturnAtBest = 0;
        bool raceSeen = false;

        for (int attempt = 1; attempt <= 3; attempt++)
        {
            int returned = Solution.RunRace();
            int rightAfter = Solution.SharedCounter;

            if (rightAfter > bestTotal)
            {
                bestTotal = rightAfter;
                returnedAtBest = returned;
                counterAfterReturnAtBest = rightAfter;
            }

            // A run counts as "race seen" when it was fully joined AND short.
            if (returned == rightAfter && rightAfter < expected)
                raceSeen = true;
        }

        await Task.Delay(50); // fair chance for any forgotten-Join stragglers

        var result = new HarnessResult();

        result.Add(
            name: "threads-started",
            passed: bestTotal > 0,
            expected: "the counter moved at all (your threads really ran)",
            actual: $"best total across 3 runs: {bestTotal}",
            message: "The counter never moved. Did you create the threads AND call Start()? " +
                     "A thread that is only created never runs.");

        result.Add(
            name: "several-threads-ran",
            passed: bestTotal > Solution.IncrementsPerThread,
            expected: "more than one thread's worth of increments (proof several threads ran)",
            actual: $"best total: {bestTotal} - one thread alone would give exactly {Solution.IncrementsPerThread}",
            message: "Only about one thread's worth of increments landed. Start ALL ThreadCount threads in a loop. " +
                     "Also check you did not Join each thread right after Start() - that serializes them and kills the race.");

        result.Add(
            name: "joined-before-returning",
            passed: bestTotal > 0 && returnedAtBest == counterAfterReturnAtBest,
            expected: "RunRace returns only AFTER all threads finished (counter stands still at return)",
            actual: $"returned {returnedAtBest}, but the counter was already {counterAfterReturnAtBest} the moment after",
            message: "RunRace came back while workers were still incrementing. Most likely cause: a missing Join. " +
                     "Keep the threads in an array and Join EVERY one before returning.");

        result.Add(
            name: "race-fingerprint-seen",
            passed: raceSeen,
            expected: $"at least one fully-joined run below the expected {expected} - lost increments!",
            actual: raceSeen ? $"saw a torn total below {expected}" : $"every run hit exactly {expected}",
            message: "Your increments never collided - the code is TOO SAFE for this exercise. Here the bug IS the " +
                     "assignment: use plain SharedCounter++ (no lock, no Interlocked) so you can watch the race happen.");

        return result;
    }
}
