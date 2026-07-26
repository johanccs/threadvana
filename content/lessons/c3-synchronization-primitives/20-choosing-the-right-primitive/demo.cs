using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Decision map tour — each row illustrated quickly:");

        // Interlocked
        var c = 0; Interlocked.Increment(ref c);
        Trace.Log("message", $"Interlocked: counter = {c}");

        // lock
        var lk = new object(); lock (lk) { Trace.Log("work-start", "lock acquired"); }
        Trace.Log("work-end", "lock released");

        // SemaphoreSlim
        using var slim = new SemaphoreSlim(1);
        await slim.WaitAsync(); Trace.Log("work-start", "SemaphoreSlim entered");
        slim.Release(); Trace.Log("work-end", "SemaphoreSlim released");
    }
}
