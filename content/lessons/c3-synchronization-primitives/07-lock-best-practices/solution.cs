using System.Threading;

public static class Solution
{
    private static readonly object _gate = new();
    public static int Counter;

    public static void Increment() { lock (_gate) { Counter++; } }
    public static void Reset() { lock (_gate) { Counter = 0; } }
}
