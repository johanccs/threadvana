using System;
using System.Threading;

public static class Solution
{
    // The shared, unprotected counter. Racy on purpose in this lesson!
    public static int SharedCounter = 0;

    public const int ThreadCount = 6;
    public const int IncrementsPerThread = 100_000;

    // Returns the final value of SharedCounter after ALL threads finish.
    public static int RunRace()
    {
        // TODO: 1. Reset SharedCounter to 0.
        //       2. Start ThreadCount threads. Each one loops
        //          IncrementsPerThread times doing SharedCounter++;
        //       3. Join EVERY thread, then return SharedCounter.
        return 0;
    }
}
