using System;
using System.Threading;

public static class Solution
{
    // The checker reads these flags after Run() returns.
    public static bool JobARan = false;
    public static bool JobBRan = false;

    // Provided: a pretend job that takes about 400ms.
    public static void JobA()
    {
        Thread.Sleep(400); // pretend to work
        JobARan = true;
    }

    // Provided: another pretend job that takes about 400ms.
    public static void JobB()
    {
        Thread.Sleep(400); // pretend to work
        JobBRan = true;
    }

    public static void Run()
    {
        // TODO: 1. Create one thread that runs JobA and one that runs JobB.
        //       2. Call Start() on BOTH threads.
        //       3. Call Join() on BOTH threads, so Run() only returns
        //          when both jobs are done.
        //
        // Goal: total time under 700ms (one-after-another would be ~800ms).
    }
}
