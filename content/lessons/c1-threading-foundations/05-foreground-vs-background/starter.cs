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
        // This is BROKEN on purpose: the worker is background and nobody
        // waits for it. In a real program it would be cut off mid-count!
        var worker = new Thread(CountToTen);
        worker.IsBackground = true;

        worker.Start();
        // ...and Run() returns immediately, while Counter is still 0.

        // TODO: fix this so the counting is GUARANTEED finished (Counter == 10)
        //       before Run() returns. One well-placed line is enough.
    }
}
