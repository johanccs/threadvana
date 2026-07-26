using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static volatile bool _stop;

    public static async Task RunAsync()
    {
        Trace.Log("message", "Spinning on volatile flag — another thread sets it after 500ms");

        var spinner = Task.Run(() =>
        {
            Trace.Log("thread-start", "Spinner waiting for _stop");
            var spins = 0;
            while (!_stop)
            {
                spins++;
                Thread.SpinWait(100);
            }
            Trace.Log("work-end", $"Stopped after ~500ms (spins: {spins})");
        });

        var setter = Task.Run(async () =>
        {
            await Task.Delay(500);
            _stop = true;
            Trace.Log("message", "_stop set to true");
        });

        await Task.WhenAll(spinner, setter);
    }
}
