using System.Collections.Concurrent;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> ProduceAndConsume()
    {
        var bc = new BlockingCollection<int>(5);
        var consumer = Task.Run(() =>
        {
            var count = 0;
            foreach (var _ in bc.GetConsumingEnumerable()) count++;
        });
        var producer = Task.Run(() =>
        {
            for (var i = 1; i <= 8; i++) bc.Add(i);
            bc.CompleteAdding();
        });
        await Task.WhenAll(producer, consumer);
        return "done";
    }
}
