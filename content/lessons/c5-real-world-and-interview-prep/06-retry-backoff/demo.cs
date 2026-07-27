using System;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? call fails — transient error");
        var attempt = 0;
        while (attempt < 3)
        {
            try
            {
                await UnreliableCallAsync();
                Trace.Log("async-resume", "? call succeeded");
                break;
            }
            catch
            {
                attempt++;
                if (attempt >= 3) { Trace.Log("message", "all retries exhausted"); break; }
                var delay = 100 * (int)Math.Pow(2, attempt);
                Trace.Log("async-suspend", $"? attempt {attempt} failed — backing off {delay}ms");
                await Task.Delay(delay);
            }
        }
        Trace.Log("message", "Retry with exponential backoff: 100ms ? 200ms ? 400ms.");
    }

    private static int _calls;
    private static async Task UnreliableCallAsync() { _calls++; if (_calls < 3) throw new Exception(); }
}