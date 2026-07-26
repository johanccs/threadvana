using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static SpinLock _spin = new();
    private static int _counter;

    public static async Task RunAsync()
    {
        Trace.Log("message", "4 workers incrementing a shared counter under SpinLock");
        var tasks = new Task[4];
        for (var i = 0; i < 4; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                var taken = false;
                try
                {
                    _spin.Enter(ref taken);
                    _counter++;
                    Trace.Log("work-start", $"SpinLock acquired, counter = {_counter}");
                    Thread.SpinWait(1000);
                }
                finally { if (taken) _spin.Exit(); }
                Trace.Log("work-end", "SpinLock released");
            });
        }
        await Task.WhenAll(tasks);
        Trace.Log("message", $"Final counter = {_counter}");
    }
}
