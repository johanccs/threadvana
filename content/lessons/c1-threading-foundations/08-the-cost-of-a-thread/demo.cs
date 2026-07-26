using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("thread-start", "Main thread starts");
        Trace.Log("message", "Hiring 200 workers... each one brings its own ~1 MB desk (stack)");

        var workers = new List<Thread>();
        for (int i = 0; i < 200; i++)
        {
            // These workers never log anything, so they never even appear on
            // the timeline. They only COST - that is the whole point.
            var t = new Thread(() =>
            {
                Thread.Sleep(600); // idle - but its ~1 MB stack is reserved
            });
            workers.Add(t);
            t.Start();
        }

        Trace.Log("message", "All 200 started - together they reserved about 200 x 1 MB = 200 MB of stack!");

        Trace.Log("wait-start", "Main waits for all 200 idle workers (Join)");
        foreach (var t in workers) t.Join();
        Trace.Log("wait-end", "All 200 workers done");

        Trace.Log("message", "200 MB and 200 hires for 600ms of waiting. Next lesson: the cheaper way.");
        Trace.Log("thread-end", "Main thread ends");

        await Task.CompletedTask;
    }
}
