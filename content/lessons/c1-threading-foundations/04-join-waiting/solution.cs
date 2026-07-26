using System;
using System.Threading;

public static class Solution
{
    // The checker reads this IMMEDIATELY when Run() returns - no grace period.
    public static bool Flag = false;

    // Store your thread here so the checker can inspect it.
    public static Thread Worker = null;

    // Provided: a job that takes 300ms, then sets the flag.
    public static void Work()
    {
        Thread.Sleep(300); // pretend to work
        Flag = true;
    }

    public static void Run()
    {
        // Create the worker and keep its handle where the checker can see it.
        Worker = new Thread(Work);

        Worker.Start();

        // Pause here until the worker is completely done.
        // After this line, Flag is guaranteed to be true.
        Worker.Join();
    }
}
