using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static int _value;

    public static async Task RunAsync()
    {
        Trace.Log("message", "4 threads CAS-updating a shared value — multiply by 2 each time");
        var tasks = new Task[4];
        for (var t = 0; t < 4; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                var current = Volatile.Read(ref _value);
                while (true)
                {
                    var next = current * 2;
                    var original = Interlocked.CompareExchange(ref _value, next, current);
                    if (original == current) break;
                    current = original;
                }
                Trace.Log("work-end", $"CAS success — value now {Volatile.Read(ref _value)}");
            });
        }
        _value = 1; // trigger start
        await Task.WhenAll(tasks);
        Trace.Log("message", $"Final: {_value} — 1×2×2×2×2 = 16");
    }
}
