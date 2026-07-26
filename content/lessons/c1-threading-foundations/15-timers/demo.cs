using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        // One-shot timer
        Trace.Log("message", "Setting a one-shot timer: fire in 800 ms");
        using var oneShot = new Timer(_ =>
        {
            Trace.Log("pool-dequeued", "One-shot timer fires!");
            Trace.Log("work-start", "Doing quick one-shot work");
            Thread.Sleep(100);
            Trace.Log("work-end", "One-shot work done");
            Trace.Log("thread-end", "One-shot timer thread");
        }, null, dueTime: 800, period: Timeout.Infinite);

        // Repeating timer. The variable is declared BEFORE the timer is created:
        // the callback stops the timer from inside itself, and a lambda cannot use
        // a variable that is declared by the very same statement.
        var ticks = 0;
        Trace.Log("message", "Starting a repeating timer every 300 ms");
        Timer repeat = null;
        repeat = new Timer(_ =>
        {
            ticks++;
            Trace.Log("pool-dequeued", $"Repeating tick {ticks}");
            Trace.Log("work-start", $"Tick {ticks} work");
            Thread.Sleep(80);
            Trace.Log("work-end", $"Tick {ticks} done");
            if (ticks >= 3)
            {
                Trace.Log("message", "Stopping the repeating timer");
                repeat.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }, null, dueTime: 0, period: 300);

        // Wait for the repeating timer to finish (~900 ms total).
        while (ticks < 3) await Task.Delay(50);
        repeat.Dispose();

        Trace.Log("message", "All timers done");
        await Task.CompletedTask;
    }
}
