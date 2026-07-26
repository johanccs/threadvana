using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly AsyncLocal<string> _context = new();

    public static async Task RunAsync()
    {
        _context.Value = "request-id-42";
        Trace.Log("message", $"Set AsyncLocal to: {_context.Value}");
        await Task.Yield(); // forces a potential thread change
        Trace.Log("message", $"After await — AsyncLocal still: {_context.Value}");
    }
}
