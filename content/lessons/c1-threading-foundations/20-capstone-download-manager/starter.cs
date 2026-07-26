using System;
using System.Collections.Generic;
using System.Threading;

public static class Solution
{
    public static readonly object Gate = new();
    public static readonly Queue<int> Queue = new();
    public static volatile bool ShouldStop = false;
    public static int ProcessedCount = 0;

    public static void Run()
    {
        lock (Gate) Queue.Clear();
        ShouldStop = false;
        ProcessedCount = 0;

        var worker1 = new Thread(() =>
        {
            // TODO: fill in the worker loop here.
            while (true)
            {
                // TODO: lock, dequeue or break, then process.
            }
        });
        var worker2 = new Thread(() =>
        {
            while (true)
            {
                // TODO: same loop as worker 1.
            }
        });

        worker1.Start(); worker2.Start();

        // Producer (provided)
        for (var i = 1; i <= 10; i++)
        {
            lock (Gate) Queue.Enqueue(i);
            Thread.Sleep(20);
        }

        ShouldStop = true;

        worker1.Join(); worker2.Join();
    }

    public static void ProcessItem(int item) => Interlocked.Increment(ref ProcessedCount);
}
