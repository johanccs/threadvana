using System;
using System.Threading;

public static class Solution
{
    // The checker reads these after PoliteWork() returns.
    public static int Count = 0;
    public static int PauseCount = 0;

    // Provided for reference: the IMPOLITE original - a tight loop,
    // 2000 rounds, never once offering the CPU to anyone else.
    public static void BusyWork()
    {
        for (int i = 0; i < 2000; i++)
        {
            Count = i + 1; // pretend-work, one round at a time
        }
    }

    // Provided helper: take a tiny polite pause AND count that it happened.
    public static void Pause()
    {
        PauseCount++;
        Thread.Yield(); // offer the CPU to someone else (Sleep(0) works too)
    }

    public static void PoliteWork()
    {
        // The same loop as BusyWork - but YOU make it polite.
        for (int i = 0; i < 2000; i++)
        {
            Count = i + 1; // pretend-work, one round at a time

            // TODO: every 100 iterations, call Pause() so other threads get
            //       a turn. Keep all 2000 rounds - only ADD the pauses.
            //       (Hint: i % 100 == 99 is true on every 100th round.)
        }
    }
}
