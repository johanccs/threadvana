using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int Counter;

    public static Task<string> RunWithMaxParallelismAsync()
    {
        var opts = new ParallelOptions { MaxDegreeOfParallelism = 2 };
        Parallel.For(0, 10, opts, _ => Interlocked.Increment(ref Counter));
        return Task.FromResult("done");
    }
}
