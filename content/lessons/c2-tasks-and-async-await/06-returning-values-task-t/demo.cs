using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? Task.Run returns Task<int> — a promise of a future int");
        var task = Task.Run(() =>
        {
            Thread.Sleep(300);
            return 42;
        });

        Trace.Log("async-suspend", "? await the Task<int> — thread released while computing");
        int result = await task;
        Trace.Log("async-resume", $"? await unwrapped the Task<int> ? got {result}");

        Trace.Log("message", $"Task<T> is a box that will contain a value. await opens the box — result: {result}");
    }
}