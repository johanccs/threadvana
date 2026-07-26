using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly object Gate = new();

    /// <summary>Try to enter Gate with a 500ms timeout.</summary>
    public static async Task<string> TryEnterWithTimeoutAsync()
    {
        // TODO: use Monitor.TryEnter, not lock
        return "not implemented";
    }
}
