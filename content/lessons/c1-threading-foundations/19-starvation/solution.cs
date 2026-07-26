using System;
using System.Threading;
using System.Diagnostics;

public static class Solution
{
    public static int GreedyRuns = 0;
    public static long StarvingWaitedMs = 0;
    public static int MinWaitMs = 250;

    private static readonly object _gate = new();

    public static void Run()
    {
        GreedyRuns = 0;
        StarvingWaitedMs = 0;

        // Signalled once the greedy worker actually holds the lock. Starting the
        // starving worker only AFTER that moment is what guarantees it has to
        // wait for the whole greedy run instead of slipping in first.
        var greedyHasLock = new ManualResetEventSlim(false);

        var starving = new Thread(() =>
        {
            var sw = Stopwatch.StartNew();
            lock (_gate)
            {
                StarvingWaitedMs = sw.ElapsedMilliseconds;
                // Got it — finally!
            }
        });
        starving.Name = "starving-worker";

        var greedy = new Thread(() =>
        {
            lock (_gate)
            {
                greedyHasLock.Set();
                Thread.Sleep(50); // give the starving worker a moment to start waiting
                for (var i = 0; i < 5; i++)
                {
                    GreedyRuns++;
                    Thread.Sleep(50); // 5 chunks of slow work — all inside the lock!
                }
            }
        });
        greedy.Name = "greedy-worker";

        greedy.Start();
        greedyHasLock.Wait();
        starving.Start();

        starving.Join();
        greedy.Join();
    }
}
