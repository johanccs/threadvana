using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Simulating a dump analysis workflow — mental model only");
        Trace.Log("message", "1. dotnet-dump collect -p <pid> → captures process state");
        Trace.Log("message", "2. clrthreads → list all managed threads + wait reasons");
        Trace.Log("message", "3. Find threads stuck in Monitor.Enter / WaitOne → deadlock candidates");
    }
}
