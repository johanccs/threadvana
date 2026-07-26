using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    // Pretend "is the program still running?" switch. The volatile keyword
    // makes sure both threads always see the latest value.
    private static volatile bool _programRunning = true;

    public static async Task RunAsync()
    {
        Trace.Log("thread-start", "Main thread (foreground) starts");

        // A background worker with a LONG job: 10 steps.
        var background = new Thread(() =>
        {
            Trace.Log("thread-start", "Background worker starts its 10-step job");
            for (int step = 1; step <= 10; step++)
            {
                if (!_programRunning)
                {
                    // In a real program the PROCESS itself would end here and
                    // this thread would vanish - no goodbye, no finally block.
                    Trace.Log("thread-end", "Background worker CUT OFF at step " + (step - 1) + " of 10!");
                    return;
                }
                Trace.Log("work-start", "Background step " + step + " of 10");
                Thread.Sleep(150); // pretend to work
                Trace.Log("work-end", "Step " + step + " done");
            }
            Trace.Log("thread-end", "Background worker finished all 10 steps");
        });
        background.IsBackground = true; // this thread must not hold the program open
        background.Name = "background-worker";

        background.Start();

        // Meanwhile the "program" (main) only has a little work left.
        Trace.Log("work-start", "Main finishes the program's last bit of work");
        Thread.Sleep(500);
        Trace.Log("work-end", "Main's work is done");

        _programRunning = false; // "the last foreground thread is about to end"
        Trace.Log("message", "Last foreground thread ends - the program exits NOW");
        Trace.Log("thread-end", "Main thread ends (program over)");

        // Demo-host housekeeping only: make sure the worker noticed the ending
        // before we return. In a REAL program there is nobody left to Join -
        // the background thread is just gone.
        background.Join();

        await Task.CompletedTask;
    }
}
