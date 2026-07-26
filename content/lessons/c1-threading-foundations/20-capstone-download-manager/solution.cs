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

        var worker1 = new Thread(() => WorkerLoop());
        var worker2 = new Thread(() => WorkerLoop());

        worker1.Start(); worker2.Start();

        for (var i = 1; i <= 10; i++)
        {
            lock (Gate) Queue.Enqueue(i);
            Thread.Sleep(20);
        }

        ShouldStop = true;
        worker1.Join(); worker2.Join();
    }

    private static void WorkerLoop()
    {
        while (true)
        {
            int item = -1;
            bool hasItem = false;

            lock (Gate)
            {
                if (Queue.Count > 0)
                {
                    item = Queue.Dequeue();
                    hasItem = true;
                }
                else if (ShouldStop) break;
            }

            if (hasItem) ProcessItem(item);
            else Thread.Sleep(1);
        }
    }

    public static void ProcessItem(int item) => Interlocked.Increment(ref ProcessedCount);
}
