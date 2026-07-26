using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Decision map tour — each tool in one line:");
        var dict = new System.Collections.Concurrent.ConcurrentDictionary<string,int>();
        dict.TryAdd("k", 1);
        Trace.Log("message", $"ConcurrentDict: {dict.Count}");
        var ch = System.Threading.Channels.Channel.CreateBounded<int>(2);
        Trace.Log("message", $"BoundedChannel: created cap {2}");
        var sum = 0;
        Parallel.ForEach(new[]{1,2,3}, n => Interlocked.Add(ref sum, n));
        Trace.Log("message", $"Parallel.ForEach sum: {sum}");
    }
}
