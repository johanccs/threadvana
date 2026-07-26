using System;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Part 1: exception AFTER await — captured in the Task");
        var task = BuggyAfterAwait();
        try
        {
            await task; // throws here
        }
        catch (Exception ex)
        {
            Trace.Log("message", $"Caught at await point: {ex.GetType().Name}");
        }

        Trace.Log("message", "Part 2: WhenAll with two faulted tasks — await throws only the first");
        var t1 = Task.Run(() => throw new InvalidOperationException("E1"));
        var t2 = Task.Run(() => throw new ArgumentException("E2"));
        var both = Task.WhenAll(t1, t2);
        try
        {
            await both;
        }
        catch (Exception ex)
        {
            Trace.Log("message", $"await threw: {ex.GetType().Name}: {ex.Message}");
            Trace.Log("message", $"Total exceptions in WhenAll: {both.Exception?.InnerExceptions.Count ?? 0}");
        }
    }

    private static async Task BuggyAfterAwait()
    {
        await Task.Delay(100);
        throw new InvalidOperationException("Boom after await");
    }
}
