using System.Collections.Concurrent;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly ConcurrentBag<int> Items = new();

    public static Task<string> FillAndDrainBag()
    {
        // TODO: add 1..4, take all, return count
        return Task.FromResult("0");
    }
}
