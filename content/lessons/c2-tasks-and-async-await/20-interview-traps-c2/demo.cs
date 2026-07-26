using System;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Tour of c2 traps in action:");

        // Trap 1: .Result blocks while await breezes past
        Trace.Log("work-start", ".Result blocking demo");
        var blocker = Task.Run(() => { Task.Delay(300).Wait(); Trace.Log("work-end", ".Result worker done"); });
        var awaiter = Task.Run(async () => { await Task.Delay(100); Trace.Log("work-end", "await worker done"); });
        await Task.WhenAll(awaiter, blocker);
        Trace.Log("message", "Both finished — but .Result hogged a thread the whole time");
    }
}

