using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    // A pretend job that takes 400ms, traced so you can see it on the timeline.
    private static void Job(string label)
    {
        Trace.Log("work-start", label + " starts");
        Thread.Sleep(400); // pretend to work
        Trace.Log("work-end", label + " done");
    }

    public static async Task RunAsync()
    {
        // ---- Part 1: SEQUENTIAL - one worker does both jobs, one after another.
        Trace.Log("message", "Part 1: ONE thread does both jobs, one after another");
        Trace.Log("thread-start", "Main thread starts");
        var oneWorker = Stopwatch.StartNew();
        Job("Job A");
        Job("Job B");
        oneWorker.Stop();

        // ---- Part 2: PARALLEL - two workers, one job each, at the same time.
        Trace.Log("message", "Part 2: TWO threads, one job each, at the same time");
        var workerA = new Thread(() =>
        {
            Trace.Log("thread-start", "Worker A starts");
            Job("Job A");
            Trace.Log("thread-end", "Worker A ends");
        });
        workerA.Name = "worker-A";

        var workerB = new Thread(() =>
        {
            Trace.Log("thread-start", "Worker B starts");
            Job("Job B");
            Trace.Log("thread-end", "Worker B ends");
        });
        workerB.Name = "worker-B";

        var twoWorkers = Stopwatch.StartNew();
        workerA.Start();
        workerB.Start(); // BOTH are running before anyone waits

        Trace.Log("wait-start", "Main waits for both workers (Join)");
        workerA.Join();
        workerB.Join();
        twoWorkers.Stop();
        Trace.Log("wait-end", "Both workers done");

        Trace.Log("message", "Same work! One worker took " + oneWorker.ElapsedMilliseconds +
                             "ms, two workers took " + twoWorkers.ElapsedMilliseconds + "ms");
        Trace.Log("thread-end", "Main thread ends");

        await Task.CompletedTask;
    }
}
