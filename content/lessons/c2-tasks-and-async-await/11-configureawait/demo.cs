using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? method starts on calling thread");

        Trace.Log("async-suspend", "? await WITHOUT ConfigureAwait(false) — context captured");
        await Task.Delay(300);
        Trace.Log("async-resume", "? resumed (could return to original context)");

        Trace.Log("async-suspend", "? await WITH ConfigureAwait(false) — NO context capture");
        await Task.Delay(300).ConfigureAwait(false);
        Trace.Log("async-resume", "? resumed on pool thread (different thread)");

        Trace.Log("message", "ConfigureAwait(false): continuation runs on ANY pool thread — avoids deadlocks in library code.");
    }
}