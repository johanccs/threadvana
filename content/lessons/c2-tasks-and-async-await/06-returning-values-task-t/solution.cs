using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int ExpensiveCompute(int n)
    {
        Thread.Sleep(50 + n);
        return n * n;
    }

    public static async Task<int> ComputeSumAsync()
    {
        var t1 = Task.Run(() => ExpensiveCompute(10));
        var t2 = Task.Run(() => ExpensiveCompute(20));
        var t3 = Task.Run(() => ExpensiveCompute(30));

        int[] results = await Task.WhenAll(t1, t2, t3);
        return results[0] + results[1] + results[2];
    }
}
