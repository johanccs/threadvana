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
        SharedCounter = 0;

        // Start all six racers and keep them so we can Join every one.
        var racers = new Thread[ThreadCount];
        for (int t = 0; t < ThreadCount; t++)
        {
            racers[t] = new Thread(() =>
            {
                for (int i = 0; i < IncrementsPerThread; i++)
                    SharedCounter++; // plain and racy - the bug IS the assignment
            });
            racers[t].Start();
        }

        // Wait for every racer, so the total is final when we return.
        foreach (var racer in racers)
            racer.Join();

        return SharedCounter;
    }
}
