using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? Mark method as async Task — compiler builds state machine");
        Thread.Sleep(80);
        Trace.Log("work-end", "synchronous code runs normally");

        Trace.Log("async-suspend", "? await keyword — method PAUSES, thread is RELEASED");
        await Task.Delay(500);
        Trace.Log("async-resume", "? awaited task finished — state machine RESUMES");

        Trace.Log("work-start", "? code after await continues");
        Thread.Sleep(80);
        Trace.Log("message", "async + await: method pauses at await, thread returns to pool, continuation picks up later — no blocking.");
    }
}