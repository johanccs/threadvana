using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        Trace.Log("message", $"Parallel.ForEach on {items.Length} items");
        var sum = 0;
        await Task.Run(() =>
            Parallel.ForEach(items, item =>
            {
                Trace.Log("work-start", $"Processing {item}");
                Thread.Sleep(item * 30);
                Interlocked.Add(ref sum, item);
                Trace.Log("work-end", $"Done {item}");
            }));
        Trace.Log("message", $"Sum = {sum} (expected 36)");
    }
}
