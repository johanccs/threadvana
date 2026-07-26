using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>A slow computation the learner doesn't write — harness provides it.</summary>
    public static int ExpensiveCompute(int n)
    {
        Thread.Sleep(50 + n);
        return n * n;
    }

    /// <summary>Start three ExpensiveCompute calls in parallel, await all, return the sum.</summary>
    public static Task<int> ComputeSumAsync()
    {
        // TODO: Task.Run(() => ExpensiveCompute(10)) etc., then await Task.WhenAll and sum
        return Task.FromResult(0);
    }
}
