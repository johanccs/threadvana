using System;
using System.Threading;

public static class Solution
{
    // Each thread reserves about 1 MB of memory just for its stack
    // (its own "desk"). This never changes in this exercise.
    public const int StackMbPerThread = 1;

    // THE answer: 512 threads x 1 MB = 512 MB - exactly the budget.
    // One more thread (513 x 1 MB = 513 MB) would already be over.
    public static int ThreadCount = 512;

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
    }
}
