using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly object LockA = new();
    public static readonly object LockB = new();

    public static async Task<string> RunDeadlockFree()
    {
        var t1 = Task.Run(() =>
        {
            lock (LockA) { Thread.Sleep(50); lock (LockB) { /* work */ } }
        });
        var t2 = Task.Run(() =>
        {
            lock (LockA) { Thread.Sleep(50); lock (LockB) { /* work */ } }
        });
        await Task.WhenAll(t1, t2);
        return "safe";
    }
}
