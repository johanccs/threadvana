using System.Linq;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly int[] Data = Enumerable.Range(1, 10).ToArray();

    public static Task<int> ComputeParallelSumAsync()
        => Task.FromResult(Data.AsParallel().Select(n => n * n).Sum());
}
