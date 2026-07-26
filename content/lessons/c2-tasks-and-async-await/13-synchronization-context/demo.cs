using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var ctx = SynchronizationContext.Current;
        Trace.Log("message", ctx is null
            ? "SynchronizationContext.Current is null — this is a console app, continuations run on the pool."
            : $"SynchronizationContext is {ctx.GetType().Name}");

        Trace.Log("thread-start", "Before await");
        await Task.Delay(200);
        Trace.Log("work-end", "After await — still null context, ran on the pool");

        Trace.Log("message", "In a WPF app this same code would capture the UI dispatcher as the context and marshal back to it.");
    }
}
