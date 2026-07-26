using System;
using System.Threading;

public static class Solution
{
    [ThreadStatic]
    public static int Counter = 0;

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
