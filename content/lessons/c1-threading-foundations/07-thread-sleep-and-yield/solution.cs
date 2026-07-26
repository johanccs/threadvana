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
        for (int i = 0; i < 2000; i++)
        {
            Count = i + 1; // pretend-work, one round at a time

            // THE fix: every 100th round, offer the CPU to other threads.
            // (i is 0-based, so i % 100 == 99 fires at rounds 100, 200, ...)
            if (i % 100 == 99)
                Pause();
        }
        // Result: all 2000 rounds done, 20 polite pauses taken.
    }
}
