using System;
using System.Collections.Concurrent;
using System.Threading;

public static class Solution
{
    public static readonly ConcurrentQueue<int> Queue = new();
    public static readonly ManualResetEventSlim Signal = new(false);
    public static int ProcessedCount = 0;

    public static void Run()
    {
        ProcessedCount = 0;
        Signal.Reset();
        while (Queue.TryDequeue(out _)) { } // clear

        var consumer = new Thread(() =>
        {
            // TODO: dequeue loop — dequeue, process, or break when signal + empty.
        });
        consumer.Name = "consumer";
        consumer.Start();

        // Producer (provided)
        for (var i = 1; i <= 5; i++)
        {
            Queue.Enqueue(i);
            Thread.Sleep(20);
        }
        Signal.Set();

        consumer.Join();
    }
}
