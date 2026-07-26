using System;
using System.Collections.Generic;
using System.Threading;

public static class Solution
{
    // Provided: recording helpers. The lock keeps the lists safe while
    // several pool workers record at the same time.
    private static readonly object _gate = new object();
    public static readonly List<int> Processed = new List<int>();
    public static readonly List<int> ThreadIds = new List<int>();

    // Set to false if any order is processed OUTSIDE the thread pool.
    public static bool AllFromPool = true;

    // Provided: starts at 3 - each finished order signals it once.
    // When it reaches zero, all orders are done. (Pool threads are
    // background - you cannot Join them; you Wait() on this instead.)
    public static readonly CountdownEvent Done = new CountdownEvent(3);

    // Provided: processes one order, records it, then signals.
    public static void ProcessOrder(int id)
    {
        Thread.Sleep(150); // pretend to process
        lock (_gate)
        {
            Processed.Add(id);
            ThreadIds.Add(Environment.CurrentManagedThreadId);
            if (!Thread.CurrentThread.IsThreadPoolThread)
                AllFromPool = false;
            if (!Done.IsSet)
                Done.Signal(); // inside the lock, so it can never over-signal
        }
    }

    public static void Run()
    {
        // TODO: 1. Hand all three orders to the pool:
        //          ThreadPool.QueueUserWorkItem(_ => ProcessOrder(1));
        //          ... and the same for orders 2 and 3.
        //       2. Wait until all three signal done: Done.Wait();
    }
}
