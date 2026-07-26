using System;
using System.Threading;

public static class Solution
{
    // Each thread reserves about 1 MB of memory just for its stack
    // (its own "desk"). This never changes in this exercise.
    public const int StackMbPerThread = 1;

    // YOUR ANSWER: the LARGEST number of threads whose total stack
    // estimate fits the 512 MB budget. (0 = not answered yet.)
    public static int ThreadCount = 0;

    // Provided: the stack-memory estimate for a given number of threads.
    public static int EstimateStackMemoryMb(int threadCount)
    {
        return threadCount * StackMbPerThread;
    }

    // Provided: the code that WILL use your number when the app runs.
    // (The checker only validates your number - it does not run this.)
    public static void SpawnWorkers()
    {
        for (int i = 0; i < ThreadCount; i++)
        {
            var t = new Thread(() => Thread.Sleep(1000));
            t.Start();
        }
    }

    public static void Run()
    {
        // Nothing to run here - this exercise is about the NUMBER.
        // Set ThreadCount above to the largest value whose stack estimate
        // stays within the 512 MB budget.
    }
}
