using System;
using System.Threading;

public static class Solution
{
    public static readonly object LockA = new(), LockB = new();
    public static bool Deadlocked = false;

    public static void Run()
    {
        Deadlocked = false;

        var t1 = new Thread(() =>
        {
            lock (LockA)
            {
                Thread.Sleep(50);
                lock (LockB)
                {
                    Thread.Sleep(20);
                }
            }
        });

        var t2 = new Thread(() =>
        {
            lock (LockA) // Fixed: locks A first, then B — same order as t1.
            {
                Thread.Sleep(50);
                lock (LockB)
                {
                    Thread.Sleep(20);
                }
            }
        });

        t1.Start(); t2.Start();
        if (!t1.Join(2000)) { Deadlocked = true; }
        if (!t2.Join(2000)) { Deadlocked = true; }
    }
}
