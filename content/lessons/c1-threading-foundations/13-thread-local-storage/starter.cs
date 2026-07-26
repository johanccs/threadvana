using System;
using System.Threading;

public static class Solution
{
    // This counter is shared — both threads fight over it.
    // TODO: add [ThreadStatic] so every thread has its OWN count.
    public static int Counter = 0;

    // Filled in by Run() — each thread writes its final count here.
    public static int[] Results = new int[2];

    public static void Run()
    {
        var t0 = new Thread(() => Worker(0));
        var t1 = new Thread(() => Worker(1));
        t0.Start(); t1.Start();
        t1.Join();  t0.Join();
    }

    public static void Increment() => Counter++;

    private static void Worker(int index)
    {
        Increment();
        Results[index] = Counter;
    }
}
