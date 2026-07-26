using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly int[] Data = { 1, 2, 3, 4, 5 };

    public static Task<int> SumSquaresAsync()
    {
        var sum = 0;
        Parallel.ForEach(Data, item => Interlocked.Add(ref sum, item * item));
        return Task.FromResult(sum);
    }
}
