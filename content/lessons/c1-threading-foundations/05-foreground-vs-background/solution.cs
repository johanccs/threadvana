using System;
using System.Threading;

public static class Solution
{
    // The checker reads this when Run() returns.
    public static int Counter = 0;

    // Provided: counts to 10, one tick every 50ms (about 500ms in total).
    public static void CountToTen()
    {
        for (int i = 1; i <= 10; i++)
        {
            Thread.Sleep(50); // pretend to work
            Counter = i;
        }
    }

    public static void Run()
    {
        // A new thread is FOREGROUND by default - but foreground only stops
        // the PROCESS from exiting early. It does not make Run() wait.
        var worker = new Thread(CountToTen);

        worker.Start();

        // THE fix: Run() pauses here until the counting is completely done.
        // After this line, Counter is guaranteed to be 10.
        worker.Join();
    }
}
