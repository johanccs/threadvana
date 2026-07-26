using System;
using System.Collections.Concurrent;
using System.Threading;

public static class Solution
{
    public static readonly ConcurrentDictionary<int, int> Counts = new();

    public static void Run()
    {
        Counts.Clear();

        var t1 = new Thread(() =>
        {
            for (var i = 0; i < 500; i++) Counts.TryAdd(i, 1);
        });
        var t2 = new Thread(() =>
        {
            for (var i = 0; i < 500; i++) Counts.TryAdd(i, 1);
        });

        t1.Start(); t2.Start();
        t1.Join(); t2.Join();
    }
}
