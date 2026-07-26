using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        // Broken: normal Dictionary
        Trace.Log("message", "Two threads adding to a NORMAL Dictionary...");
        var normal = new Dictionary<int, int>();
        var normalOk = true;
        var t1 = new Thread(() =>
        {
            for (var i = 0; i < 500; i++)
            {
                try { normal.Add(i, 1); }
                catch { normalOk = false; break; }
            }
        });
        var t2 = new Thread(() =>
        {
            for (var i = 500; i < 1000; i++)
            {
                try { normal.Add(i, 1); }
                catch { normalOk = false; break; }
            }
        });
        t1.Start(); t2.Start();
        t1.Join(); t2.Join();
        Trace.Log("message", $"Normal Dictionary: count={normal.Count}, ok={normalOk}");

        // Working: ConcurrentDictionary
        Trace.Log("message", "Now with ConcurrentDictionary...");
        var concurrent = new ConcurrentDictionary<int, int>();
        t1 = new Thread(() =>
        {
            for (var i = 0; i < 500; i++) concurrent.TryAdd(i, 1);
        });
        t2 = new Thread(() =>
        {
            for (var i = 500; i < 1000; i++) concurrent.TryAdd(i, 1);
        });
        t1.Start(); t2.Start();
        t1.Join(); t2.Join();
        Trace.Log("message", $"ConcurrentDictionary: count={concurrent.Count} (always 1000)");

        await Task.CompletedTask;
    }
}
