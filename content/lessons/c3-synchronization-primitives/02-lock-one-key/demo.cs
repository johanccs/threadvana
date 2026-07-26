using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    // The ONE key. All three workers lock on this same object.
    private static readonly object _gate = new object();

    public static async Task RunAsync()
    {
        Trace.Log("thread-start", $"Main thread {Environment.CurrentManagedThreadId} starts");
        Trace.Log("message", "Three workers, ONE key - watch them take turns");

        // Create three workers that all want into the same locked section.
        var workers = new Thread[3];
        for (int i = 0; i < 3; i++)
        {
            string name = "worker-" + (i + 1); // each closure gets its own copy
            workers[i] = new Thread(() => VisitLockedSection(name));
            workers[i].Name = name;
        }

        foreach (var w in workers) w.Start();

        Trace.Log("wait-start", "Main waits for all workers (Join)");
        foreach (var w in workers) w.Join();
        Trace.Log("wait-end", "All workers took their turn");

        Trace.Log("thread-end", "Main thread ends");
        await Task.CompletedTask; // demo has nothing to await - threads were Joined
    }

    private static void VisitLockedSection(string name)
    {
        Trace.Log("thread-start", $"{name} starts");
        Trace.Log("wait-start", $"{name} WAITS for the key");

        lock (_gate) // take the key - or queue at the door until it comes back
        {
            Trace.Log("wait-end", $"{name} is next in line no more");
            Trace.Log("lock-acquire", $"{name} takes the key");
            Trace.Log("work-start", $"{name} is INSIDE the locked section");
            Thread.Sleep(400); // pretend: delicate work on shared data
            Trace.Log("work-end", $"{name} finished the delicate work");
            Trace.Log("lock-release", $"{name} hands the key back");
        }

        Trace.Log("thread-end", $"{name} ends");
    }
}