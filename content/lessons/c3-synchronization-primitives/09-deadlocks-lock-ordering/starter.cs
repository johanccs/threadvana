using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly object LockA = new();
    public static readonly object LockB = new();

    /// <summary>Run two tasks that currently deadlock by locking in opposite orders.</summary>
    public static async Task<string> RunDeadlockFree()
    {
        var t1 = Task.Run(() =>
        {
            lock (LockA)
            {
                Thread.Sleep(50);
                lock (LockB) { /* work */ }
            }
        });
        var t2 = Task.Run(() =>
        {
            // BUG: opposite order causes deadlock
            lock (LockB)
            {
                Thread.Sleep(50);
                lock (LockA) { /* work */ }
            }
        });
        await Task.WhenAll(t1, t2);
        return "safe";
    }
}
