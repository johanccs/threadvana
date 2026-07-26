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
        while (Queue.TryDequeue(out _)) { }

        var consumer = new Thread(() =>
        {
            while (true)
            {
                if (Queue.TryDequeue(out var item))
                {
                    ProcessItem(item);
                }
                else if (Signal.IsSet && Queue.IsEmpty)
                    break;
                else
                    Thread.Sleep(1);
            }
        });
        consumer.Name = "consumer";
        consumer.Start();

        for (var i = 1; i <= 5; i++)
        {
            Queue.Enqueue(i);
            Thread.Sleep(20);
        }
        Signal.Set();
        consumer.Join();
    }

    private static void ProcessItem(int item) => Interlocked.Increment(ref ProcessedCount);
}
