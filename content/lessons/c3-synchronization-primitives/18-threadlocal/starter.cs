using System.Threading;

public static class Solution
{
    private static int _nextId;
    private static readonly ThreadLocal<int> _local = new(() => Interlocked.Increment(ref _nextId));

    /// <summary>Return the calling thread's unique id from ThreadLocal.</summary>
    public static string GetThreadLocalId() => _local.Value.ToString();
}
