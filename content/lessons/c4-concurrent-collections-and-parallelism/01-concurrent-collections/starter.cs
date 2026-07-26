using System;
using System.Collections.Generic;
using System.Threading;

public static class Solution
{
    // TODO: change this to ConcurrentDictionary<int, int>.
    public static readonly Dictionary<int, int> Counts = new();

    public static void Run()
    {
        Counts.Clear();

        var t1 = new Thread(() =>
        {
            for (var i = 0; i < 500; i++)
            {
                Counts.Add(i, 1); // TODO: change to TryAdd
            }
        });
        var t2 = new Thread(() =>
        {
            for (var i = 0; i < 500; i++)
            {
                Counts.Add(i, 1);
            }
        });

        t1.Start(); t2.Start();
        t1.Join(); t2.Join();
    }
}
