using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var mutex = new Mutex(false, @"Local\ThreadCraftDemo");
        Trace.Log("message", "Created a named mutex — only one demo can run at a time");
        try
        {
            if (mutex.WaitOne(1000))
            {
                Trace.Log("work-start", "Mutex acquired");
                await Task.Delay(600);
                Trace.Log("work-end", "Releasing mutex");
                mutex.ReleaseMutex();
            }
            else
            {
                Trace.Log("message", "Could not acquire mutex within 1s (another instance?)");
            }
        }
        finally { mutex.Dispose(); }
    }
}
