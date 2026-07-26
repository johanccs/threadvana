using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly object _gate = new();

    public static async Task RunAsync()
    {
        var greedy = new Thread(() =>
        {
            Trace.Log("thread-start", "Greedy thread starts");
            for (var i = 0; i < 8; i++)
            {
                lock (_gate)
                {
                    Trace.Log("lock-acquire", $"Greedy lock {i + 1}/8");
                    Thread.Sleep(120); // holds the lock
                    Trace.Log("lock-release", $"Greedy releases {i + 1}/8");
                }
                // Brief gap — but the greedy thread races right back in before
                // the polite workers get a chance.
            }
            Trace.Log("thread-end", "Greedy thread done");
        });
        greedy.Name = "greedy-thread";

        var polite1 = new Thread(() =>
        {
            Trace.Log("thread-start", "Polite worker 1 starts");
            Trace.Log("wait-start", "Waiting for lock");
            lock (_gate)
            {
                Trace.Log("lock-acquire", "Polite worker 1 got the lock (finally!)");
                Trace.Log("wait-end", "Wait over");
                Thread.Sleep(50);
                Trace.Log("lock-release", "Polite worker 1 releases");
            }
            Trace.Log("thread-end", "Polite worker 1 done");
        });
        polite1.Name = "polite-1";

        var polite2 = new Thread(() =>
        {
            Trace.Log("thread-start", "Polite worker 2 starts");
            Trace.Log("wait-start", "Waiting for lock");
            lock (_gate)
            {
                Trace.Log("lock-acquire", "Polite worker 2 got the lock");
                Trace.Log("wait-end", "Wait over");
                Thread.Sleep(50);
                Trace.Log("lock-release", "Polite worker 2 releases");
            }
            Trace.Log("thread-end", "Polite worker 2 done");
        });
        polite2.Name = "polite-2";

        polite1.Start(); polite2.Start();
        Thread.Sleep(50);
        greedy.Start();

        greedy.Join(); polite1.Join(); polite2.Join();
        await Task.CompletedTask;
    }
}
