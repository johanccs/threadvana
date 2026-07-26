using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        // Wrapping a timer callback as a Task.
        Trace.Log("message", "Wrapping a 1-second timer as a Task via TaskCompletionSource");

        var tcs = new TaskCompletionSource<string>();
        var timer = new System.Timers.Timer(1000) { AutoReset = false };
        timer.Elapsed += (_, _) =>
        {
            Trace.Log("work-end", "Timer fired — completing the TCS");
            tcs.TrySetResult("Timer finished!");
            timer.Dispose();
        };

        Trace.Log("thread-start", "Timer started — await the TCS...");
        timer.Start();
        var result = await tcs.Task;
        Trace.Log("message", $"Got result from TCS: {result}");
    }
}
