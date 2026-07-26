using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly int[] Data = { 1, 2, 3, 4, 5 };

    public static async Task<string> ClassifyWorkloadAsync(bool cpuBound)
    {
        if (cpuBound)
        {
            var sum = 0;
            Parallel.ForEach(Data, item => Interlocked.Add(ref sum, item));
            return sum.ToString();
        }
        var count = 0;
        var tasks = Data.Select(async _ => { await Task.Delay(50); Interlocked.Increment(ref count); });
        await Task.WhenAll(tasks);
        return count.ToString();
    }
}
