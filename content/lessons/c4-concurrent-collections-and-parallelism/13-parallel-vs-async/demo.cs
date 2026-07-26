using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "CPU-bound: Parallel.ForEach");
        await Task.Run(() => Parallel.ForEach(new[] { 1, 2, 3 }, i =>
        {
            Trace.Log("work-start", $"CPU work {i}"); Thread.Sleep(100);
        }));

        Trace.Log("message", "I/O-bound: Task.WhenAll with async");
        var tasks = new[] { 1, 2, 3 }.Select(async i =>
        {
            Trace.Log("work-start", $"I/O work {i}"); await Task.Delay(100);
        });
        await Task.WhenAll(tasks);
    }
}
