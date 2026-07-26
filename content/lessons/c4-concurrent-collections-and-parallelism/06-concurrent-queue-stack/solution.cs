using System.Collections.Concurrent;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly ConcurrentQueue<string> InputQueue = new(new[] { "a", "b", "c" });
    public static readonly ConcurrentStack<string> OutputStack = new();

    public static Task<string> ProcessItemsAsync()
    {
        while (InputQueue.TryDequeue(out var item))
            OutputStack.Push(item);
        return Task.FromResult("done");
    }
}
