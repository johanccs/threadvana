using System.Collections.Concurrent;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly ConcurrentBag<int> Items = new();

    public static Task<string> FillAndDrainBag()
    {
        for (var i = 1; i <= 4; i++) Items.Add(i);
        var count = 0;
        while (Items.TryTake(out _)) count++;
        return Task.FromResult(count.ToString());
    }
}
