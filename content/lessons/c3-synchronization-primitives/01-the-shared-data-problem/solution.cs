using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    // The checker reads this after each Run(). It must end at EXACTLY 2000.
    public static int Counter = 0;

    // PROVIDED - the "bathroom key". Both threads must lock on THIS SAME object.
    public static readonly object Gate = new object();

    // PROVIDED - starts two threads, each incrementing Counter 1000 times,
    // then waits for both to finish.
    public static void Run()
    {
        Counter = 0;

        Thread a = new Thread(Increment1000Times);
        Thread b = new Thread(Increment1000Times);
        a.Name = "worker-A";
        b.Name = "worker-B";

        a.Start();
        b.Start();
        a.Join();
        b.Join();
    }

    private static void Increment1000Times()
    {
        for (int i = 0; i < 1000; i++)
        {
            // THE FIX: one shared bathroom key. Only ONE thread may be inside
            // the lock at a time, so read + add + write can no longer interleave.
            lock (Solution.Gate)
            {
                int temp = Counter;   // READ
                Thread.Yield();       // even if the other thread jumps in, it WAITS at the lock
                temp = temp + 1;      // ADD
                Counter = temp;       // WRITE
            }
        }
    }
}