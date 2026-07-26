using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static int _plainCounter = 0;
    private static int _atomicCounter = 0;

    public static async Task RunAsync()
    {
        const int incrementsPerWorker = 100_000;
        Trace.Log("message", "Same workload, two counters: plain vs Interlocked.");

        // Round 1: the plain, racy counter from last lesson.
        var plainA = new Thread(() => HammerPlain("plain-A", incrementsPerWorker));
        plainA.Name = "plain-A";
        var plainB = new Thread(() => HammerPlain("plain-B", incrementsPerWorker));
        plainB.Name = "plain-B";

        Trace.Log("thread-start", "Round 1: plain counter++");
        plainA.Start();
        plainB.Start();
        plainA.Join();
        plainB.Join();
        Trace.Log("message",
            $"Plain counter: expected 200,000, got {_plainCounter} (run again - it changes!).");

        // Round 2: identical workload, one Interlocked.Increment per lap.
        var atomicA = new Thread(() => HammerAtomic("atomic-A", incrementsPerWorker));
        atomicA.Name = "atomic-A";
        var atomicB = new Thread(() => HammerAtomic("atomic-B", incrementsPerWorker));
        atomicB.Name = "atomic-B";

        Trace.Log("thread-start", "Round 2: Interlocked.Increment");
        atomicA.Start();
        atomicB.Start();
        atomicA.Join();
        atomicB.Join();
        Trace.Log("message",
            $"Interlocked counter: expected 200,000, got {_atomicCounter} (exact, every time).");

        Trace.Log("thread-end", "Main thread ends");
        await Task.CompletedTask;
    }

    private static void HammerPlain(string name, int times)
    {
        Trace.Log("work-start", name + " hammering (plain ++)");
        for (int i = 0; i < times; i++)
            _plainCounter++;
        Trace.Log("work-end", name + " done");
    }

    private static void HammerAtomic(string name, int times)
    {
        Trace.Log("work-start", name + " hammering (Interlocked)");
        for (int i = 0; i < times; i++)
            Interlocked.Increment(ref _atomicCounter); // one indivisible step
        Trace.Log("work-end", name + " done");
    }
}
