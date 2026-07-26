using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("thread-start", "Main thread starts");

        // Worker 1: takes a real timed break - watch the grey span.
        var sleepy = new Thread(() =>
        {
            Trace.Log("thread-start", "sleepy starts");
            Trace.Log("work-start", "sleepy works a little");
            Thread.Sleep(100); // pretend to work
            Trace.Log("work-end", "first bit done");

            // SLEEP: a timed coffee break - the lane shows a grey wait span.
            Trace.Log("wait-start", "sleepy takes a 300ms coffee break (Sleep)");
            Thread.Sleep(300);
            Trace.Log("wait-end", "sleepy is back from the break");

            Trace.Log("work-start", "sleepy finishes the job");
            Thread.Sleep(100); // pretend to work
            Trace.Log("work-end", "sleepy done");
            Trace.Log("thread-end", "sleepy ends");
        });
        sleepy.Name = "sleepy";

        // Worker 2: crunches in rounds and Yields politely between them.
        var polite = new Thread(() =>
        {
            Trace.Log("thread-start", "polite starts");
            for (int round = 1; round <= 3; round++)
            {
                Trace.Log("work-start", "polite crunches, round " + round);
                double total = 0;
                for (int i = 0; i < 300000; i++)
                    total += Math.Sqrt(i); // pretend number crunching (real CPU work)
                Trace.Log("work-end", "round " + round + " done (crunched " + (int)total + ")");

                Trace.Log("message", "polite says: you go ahead of me! (Yield, round " + round + ")");
                Thread.Yield(); // a polite offer - the OS may simply ignore it
            }
            Trace.Log("thread-end", "polite ends");
        });
        polite.Name = "polite";

        sleepy.Start();
        polite.Start();

        Trace.Log("wait-start", "Main waits for both (Join)");
        sleepy.Join();
        polite.Join();
        Trace.Log("wait-end", "Both done");

        Trace.Log("thread-end", "Main thread ends");
        await Task.CompletedTask;
    }
}
