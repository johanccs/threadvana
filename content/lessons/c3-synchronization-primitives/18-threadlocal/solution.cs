using System.Threading;

public static class Solution
{
    private static int _nextId;
    private static readonly ThreadLocal<int> _local = new(() => Interlocked.Increment(ref _nextId));

    public static string GetThreadLocalId() => _local.Value.ToString();
}
