using System.Threading;

public static class Solution
{
    /// <summary>A shared counter — 8 threads will call AddOne on it at once.</summary>
    public static int Count;

    /// <summary>Add 1 to the counter atomically (use Interlocked).</summary>
    public static void AddOne()
    {
        // TODO: use Interlocked.Increment instead of plain Count++
        Count++;
    }

    /// <summary>Atomically set the counter to the given value.</summary>
    public static void ResetTo(int value)
    {
        // TODO: use Interlocked.Exchange
        Count = value;
    }
}
