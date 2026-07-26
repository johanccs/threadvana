using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "async method starts on this thread");

        Thread.Sleep(100);
        Trace.Log("work-end", "sync work done (same thread - no await yet)");

        Trace.Log("async-suspend", "about to await - THREAD IS RELEASED");
        Trace.Log("work-start", "Task.Delay running (main thread FREED - can do other work)");

        await Task.Delay(400);

        Trace.Log("work-end", "Task.Delay finished");
        Trace.Log("async-resume", "continuation picked up (may be different thread!)");

        Thread.Sleep(100);
        Trace.Log("work-end", "method finished");
    }
}