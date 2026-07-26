using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    private static readonly SemaphoreSlim _sem = new(1, 1);

    public static async Task<string> AcquireAndReleaseAsync()
    {
        await _sem.WaitAsync();
        try { return "locked"; }
        finally { _sem.Release(); }
    }
}
