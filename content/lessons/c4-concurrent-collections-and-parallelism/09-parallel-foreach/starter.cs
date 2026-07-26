using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly int[] Data = { 1, 2, 3, 4, 5 };

    public static Task<int> SumSquaresAsync()
    {
        // TODO: Parallel.ForEach with Interlocked.Add
        return Task.FromResult(0);
    }
}
