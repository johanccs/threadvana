using System;
using System.Collections.Concurrent;
using System.Threading;

public static class Solution
{
    // Provided: for each task id, the id of the thread that handled it.
    // (ConcurrentDictionary = a dictionary several threads can safely
    // write to at the same time - much more about those in Category 4.)
    public static readonly ConcurrentDictionary<int, int> TaskThreadIds =
        new ConcurrentDictionary<int, int>();

    // Provided: starts at 8 - each finished task signals it once.
    public static readonly CountdownEvent Done = new CountdownEvent(8);

    // Guards the countdown so a task can never over-signal it.
    private static int _completions = 0;

    // Provided: one small task - work ~100ms, record your worker, signal.
    public static void DoTask(int id)
    {
        Thread.Sleep(100); // pretend to work
        TaskThreadIds[id] = Environment.CurrentManagedThreadId;
        if (Interlocked.Increment(ref _completions) <= 8)
            Done.Signal(); // never signaled more than 8 times
    }

    // For this exercise we cap the shared pool at 4 workers, so the reuse
    // shows up on any machine. (This runs once, automatically, before
    // anything else. Real apps rarely touch these settings - the next
    // lesson explains when tuning helps.)
    static Solution()
    {
        ThreadPool.SetMinThreads(1, 1);
        ThreadPool.SetMaxThreads(4, 4);
        ThreadPool.SetMinThreads(4, 4);
    }

    public static void Run()
    {
        for (int i = 1; i <= 8; i++)
        {
            int mine = i; // each task gets its OWN copy (the capture trap!)
            ThreadPool.QueueUserWorkItem(_ => DoTask(mine));
        }

        // Pool threads cannot be Joined - the countdown is how we wait.
        Done.Wait();
    }
}
