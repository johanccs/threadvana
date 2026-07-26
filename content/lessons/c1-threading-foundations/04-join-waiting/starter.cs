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
        // TODO: 1. Create a new Thread that runs Work.
        //       2. Store it in Solution.Worker.
        //       3. Start() it.
        //       4. Join() it, so Flag is GUARANTEED set before Run() returns.
    }
}
