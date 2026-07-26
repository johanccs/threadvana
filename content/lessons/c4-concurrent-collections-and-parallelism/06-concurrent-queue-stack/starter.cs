using System.Collections.Concurrent;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly ConcurrentQueue<string> InputQueue = new(new[] { "a", "b", "c" });
    public static readonly ConcurrentStack<string> OutputStack = new();

    public static Task<string> ProcessItemsAsync()
    {
        // TODO: dequeue all, push to stack, return "done"
        return Task.FromResult("not implemented");
    }
}
