using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var clock = Stopwatch.StartNew();
        Solution.Run();
        clock.Stop();

        // Fair chance: queued-but-not-awaited work gets a moment to finish,
        // so the checks below reflect WHAT ran, not timing luck.
        await Task.Delay(700);

        int ones = CountOccurrences(1), twos = CountOccurrences(2), threes = CountOccurrences(3);
        int distinctThreads = DistinctThreadCount();

        var result = new HarnessResult();

        result.Add(
            name: "work-was-queued",
            passed: Solution.Processed.Count > 0,
            expected: "at least one order was processed (work reached the pool)",
            actual: $"Processed count = {Solution.Processed.Count}",
            message: "Nothing was processed. Did you call ThreadPool.QueueUserWorkItem for the orders? " +
                     "ProcessOrder only runs when you hand it to the pool - it does not run by itself.");

        result.Add(
            name: "all-orders-processed",
            passed: ones == 1 && twos == 1 && threes == 1,
            expected: "orders 1, 2 and 3, each processed exactly once",
            actual: $"order 1: {ones}x, order 2: {twos}x, order 3: {threes}x",
            message: "Some orders are missing or doubled. Queue THREE separate work items - one for " +
                     "ProcessOrder(1), one for ProcessOrder(2), one for ProcessOrder(3).");

        result.Add(
            name: "waited-for-completion",
            passed: clock.ElapsedMilliseconds >= 100,
            expected: "Run() returns only after all orders are done (Done.Wait())",
            actual: $"Run() returned after {clock.ElapsedMilliseconds}ms, but processing takes ~150ms per order",
            message: "Run() came back before the pool could possibly be done. Most likely cause: missing " +
                     "Done.Wait(). Pool threads cannot be Joined - the CountdownEvent is how you wait for them.");

        result.Add(
            name: "pool-reused-workers",
            passed: Solution.Processed.Count > 0 && Solution.AllFromPool && distinctThreads < 4,
            expected: "the work ran on the pool's own workers (fewer than 4 distinct threads for 3 jobs)",
            actual: $"{distinctThreads} distinct thread id(s) did the work, all from the pool: {Solution.AllFromPool}",
            message: "The work did not run on the pool's reusable workers. Most likely cause: you used " +
                     "new Thread, or called ProcessOrder(...) directly. Use ThreadPool.QueueUserWorkItem - " +
                     "the pool lends you its workers instead of you hiring new ones.");

        return result;
    }

    private static int CountOccurrences(int orderId)
    {
        int n = 0;
        foreach (int id in Solution.Processed)
            if (id == orderId) n++;
        return n;
    }

    private static int DistinctThreadCount()
    {
        var seen = new HashSet<int>();
        foreach (int tid in Solution.ThreadIds)
            seen.Add(tid);
        return seen.Count;
    }
}
