using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int Counter;

    public static Task<string> RunWithMaxParallelismAsync()
    {
        // TODO: Parallel.For with MaxDegreeOfParallelism = 2
        return Task.FromResult("not implemented");
    }
}
