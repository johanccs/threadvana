using System.Threading;

public static class Solution
{
    public static int Count;

    public static void AddOne() => Interlocked.Increment(ref Count);

    public static void ResetTo(int value) => Interlocked.Exchange(ref Count, value);
}
