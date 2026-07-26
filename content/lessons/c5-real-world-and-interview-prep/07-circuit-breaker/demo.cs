using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static int _failures;
    private static bool _open;

    public static async Task RunAsync()
    {
        Trace.Log("message", "Calling flaky service — circuit opens after 2 failures");
        for (var i = 0; i < 5; i++)
        {
            if (_open)
            {
                Trace.Log("message", $"Call {i}: Circuit OPEN — fast fail");
                await Task.Delay(100);
                continue;
            }
            try
            {
                await FlakyServiceAsync();
                Trace.Log("work-end", $"Call {i}: OK");
            }
            catch
            {
                _failures++;
                Trace.Log("work-start", $"Call {i}: FAIL");
                if (_failures >= 2) { _open = true; Trace.Log("message", "CIRCUIT OPEN"); }
            }
        }
    }

    private static int _callCount;
    private static Task FlakyServiceAsync()
    {
        _callCount++;
        if (_callCount <= 2) throw new InvalidOperationException();
        return Task.CompletedTask;
    }
}
