using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("work-start", "? IAsyncEnumerable — producer yields items one at a time");

        await foreach (var reading in ReadSensorsAsync())
        {
            Trace.Log("async-resume", $"? consumer received: {reading}");
            await Task.Delay(80);
        }

        Trace.Log("message", "IAsyncEnumerable + await foreach: stream items as they arrive, no blocking, no buffering all at once.");
    }

    private static async IAsyncEnumerable<int> ReadSensorsAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        for (var i = 0; i < 4; i++)
        {
            Trace.Log("async-suspend", $"producer yielding next item...");
            await Task.Delay(300, ct);
            yield return i * 10 + 42;
        }
    }
}