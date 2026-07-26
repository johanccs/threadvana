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
    // then waits for both to finish. Do not change the threads -
    // your job is INSIDE Increment1000Times(): protect the increment.
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
            // "Counter++" is really these three steps (read, add, write).
            // Two threads interleave them - and increments vanish.
            // TODO: wrap ALL THREE counter lines in  lock (Solution.Gate) { ... }
            int temp = Counter;   // READ
            Thread.Yield();       // lets the other thread jump in (in real code this happens by chance!)
            temp = temp + 1;      // ADD
            Counter = temp;       // WRITE
        }
    }
}