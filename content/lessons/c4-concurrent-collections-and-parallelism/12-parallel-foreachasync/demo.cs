using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var items = Enumerable.Range(1, 6);
        var opts = new ParallelOptions { MaxDegreeOfParallelism = 3 };
        Trace.Log("message", "Parallel.ForEachAsync — 6 items, max 3 concurrent");
        await Parallel.ForEachAsync(items, opts, async (item, ct) =>
        {
            Trace.Log("work-start", $"Processing {item}");
            await Task.Delay(200 + item * 30, ct);
            Trace.Log("work-end", $"Done {item}");
        });
    }
}
