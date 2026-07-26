using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        // ---- Part 1: THE TRAP - all threads capture the SAME loop variable.
        Trace.Log("message", "Part 1: the trap - three threads share one loop variable");
        var trapThreads = new List<Thread>();
        for (int i = 0; i < 3; i++)
        {
            var t = new Thread(() =>
            {
                Trace.Log("thread-start", "Trap worker starts");
                Thread.Sleep(150); // by the time we read i, the loop is long finished...
                Trace.Log("work-start", "Trap worker reads i and gets " + i); // ...so everyone sees 3!
                Trace.Log("work-end", "Trap worker done");
                Trace.Log("thread-end", "Trap worker ends");
            });
            // The name is built RIGHT NOW, so it is correct - only the
            // captured variable goes stale. Watch the lanes prove it!
            t.Name = "trap-" + i;
            trapThreads.Add(t);
            t.Start();
        }
        foreach (var t in trapThreads) t.Join();

        // ---- Part 2: THE FIX - each thread captures its OWN copy.
        Trace.Log("message", "Part 2: the fix - each thread gets its own copy");
        var fixedThreads = new List<Thread>();
        for (int i = 0; i < 3; i++)
        {
            int mine = i; // THE FIX: a fresh local, created new each loop round
            var t = new Thread(() =>
            {
                Trace.Log("thread-start", "Worker " + mine + " starts");
                Trace.Log("work-start", "Worker " + mine + " works with its own number");
                Thread.Sleep(150); // pretend to work
                Trace.Log("work-end", "Worker " + mine + " done");
                Trace.Log("thread-end", "Worker " + mine + " ends");
            });
            t.Name = "worker-" + mine;
            fixedThreads.Add(t);
            t.Start();
        }

        Trace.Log("wait-start", "Main waits for all three workers (Join)");
        foreach (var t in fixedThreads) t.Join();
        Trace.Log("wait-end", "All three workers finished - each kept its own number");

        await Task.CompletedTask;
    }
}
