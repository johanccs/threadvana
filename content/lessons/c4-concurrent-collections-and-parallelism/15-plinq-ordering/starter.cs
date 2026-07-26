using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly int[] Items = { 1, 2, 3, 4, 5 };
    public static int EvenCount;

    public static Task<string> ProcessWithForAll()
    {
        Items.AsParallel().Where(n => n % 2 == 0).ForAll(_ => Interlocked.Add(ref EvenCount, 1));
        return Task.FromResult("done");
    }
}
