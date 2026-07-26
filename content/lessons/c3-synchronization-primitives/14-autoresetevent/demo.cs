using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var auto = new AutoResetEvent(false);
        Trace.Log("message", "8 consumers, 4 Set() calls — exactly 4 pass");
        var tasks = new Task[8];
        for (var i = 0; i < 8; i++)
        {
            var idx = i;
            tasks[i] = Task.Run(() =>
            {
                Trace.Log("thread-start", $"Consumer {idx} waiting for ticket");
                auto.WaitOne();
                Trace.Log("work-end", $"Consumer {idx} got ticket!");
            });
        }
        for (var i = 0; i < 4; i++)
        {
            await Task.Delay(300);
            Trace.Log("message", $"Producer: Set() #{i + 1}");
            auto.Set();
        }
        await Task.WhenAll(tasks.Take(4).ToArray());
        auto.Dispose();
        Trace.Log("message", "Remaining 4 consumers still waiting — demo ends here");
    }
}
