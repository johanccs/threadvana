using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Sync-over-async: calling async from sync via Task.Run");
        var result = CallSync();
        Trace.Log("message", $"Got result: {result}");
    }

    private static string CallSync()
        => Task.Run(async () => await SlowAsync()).GetAwaiter().GetResult();

    private static async Task<string> SlowAsync()
    {
        Trace.Log("work-start", "Fetching...");
        await Task.Delay(300);
        return "data";
    }
}
