using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int Value = 1;

    public static Task<string> AtomicMultiply(int factor)
    {
        var current = Volatile.Read(ref Value);
        while (true)
        {
            var next = current * factor;
            var original = Interlocked.CompareExchange(ref Value, next, current);
            if (original == current) break;
            current = original;
        }
        return Task.FromResult("done");
    }
}
