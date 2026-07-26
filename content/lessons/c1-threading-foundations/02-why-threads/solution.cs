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
        // Two workers, one job each - now the jobs run at the same time.
        var threadA = new Thread(JobA);
        var threadB = new Thread(JobB);

        threadA.Start();
        threadB.Start();   // BOTH running before anyone waits

        threadA.Join();
        threadB.Join();    // Run() returns only when both jobs are done
    }
}
