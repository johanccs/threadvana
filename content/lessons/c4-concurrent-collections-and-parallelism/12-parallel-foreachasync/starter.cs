using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly int[] Items = { 1, 2, 3, 4 };
    public static int Processed;

    public static async Task<string> ProcessItemsAsync()
    {
        var opts = new ParallelOptions { MaxDegreeOfParallelism = 2 };
        await Parallel.ForEachAsync(Items, opts, async (item, ct) =>
        {
            Interlocked.Increment(ref Processed);
            await Task.Delay(100, ct);
        });
        return "done";
    }
}
