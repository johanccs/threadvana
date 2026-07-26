using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly object LockA = new(), LockB = new();

    public static async Task RunAsync()
    {
        // This demonstrates the DEADLOCK pattern (but we avoid the actual deadlock
        // by adding a deterministic ordering — the lesson shows what happens WITHOUT it).
        Trace.Log("message", "Demonstrating the danger: Thread 1 locks A then B; Thread 2 locks B then A.");
        Trace.Log("message", "If both threads grab their first lock simultaneously — deadlock!");
        Trace.Log("message", "Fix: always lock A first, then B, everywhere. Consistent ordering = no deadlock.");

        var t1 = new Thread(() =>
        {
            Trace.Log("thread-start", "Thread 1");
            lock (LockA)
            {
                Trace.Log("lock-acquire", "Thread 1 got A");
                Thread.Sleep(100);
                lock (LockB)
                {
                    Trace.Log("lock-acquire", "Thread 1 got B");
                    Trace.Log("work-start", "Thread 1 does work");
                    Thread.Sleep(200);
                    Trace.Log("work-end", "Thread 1 done");
                    Trace.Log("lock-release", "Thread 1 releases B");
                }
                Trace.Log("lock-release", "Thread 1 releases A");
            }
            Trace.Log("thread-end", "Thread 1 ends");
        });
        t1.Name = "thread-1";

        var t2 = new Thread(() =>
        {
            Trace.Log("thread-start", "Thread 2");
            // SAFE: locks A then B — same order. No deadlock.
            lock (LockA)
            {
                Trace.Log("lock-acquire", "Thread 2 got A");
                Thread.Sleep(100);
                lock (LockB)
                {
                    Trace.Log("lock-acquire", "Thread 2 got B");
                    Trace.Log("work-start", "Thread 2 does work");
                    Thread.Sleep(200);
                    Trace.Log("work-end", "Thread 2 done");
                    Trace.Log("lock-release", "Thread 2 releases B");
                }
                Trace.Log("lock-release", "Thread 2 releases A");
            }
            Trace.Log("thread-end", "Thread 2 ends");
        });
        t2.Name = "thread-2";

        t1.Start(); t2.Start();
        t1.Join(); t2.Join();

        Trace.Log("message", "Both threads finished — no deadlock with consistent ordering.");
        await Task.CompletedTask;
    }
}
