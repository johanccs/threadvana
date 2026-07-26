using System;
using System.Threading;

public static class Solution
{
    public static int Counter = 0;
    public static int Lost = 0;

    public static void Run()
    {
        Counter = 0;

        var t1 = new Thread(() =>
        {
            for (var i = 0; i < 50_000; i++) { Counter++; }
        });
        var t2 = new Thread(() =>
        {
            for (var i = 0; i < 50_000; i++) { Counter++; }
        });

        t1.Start(); t2.Start();
        t1.Join(); t2.Join();

        Lost = 100_000 - Counter;
    }
}
