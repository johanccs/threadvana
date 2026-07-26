using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        // Make sure the pool is capped at 4 workers for this exercise,
        // even if the static constructor in Solution was edited out.
        ThreadPool.SetMinThreads(1, 1);
        ThreadPool.SetMaxThreads(4, 4);
        ThreadPool.SetMinThreads(4, 4);

        Solution.Run();

        // Fair chance: queued-but-not-awaited tasks get a moment to finish.
        await Task.Delay(700);

        int recorded = Solution.TaskThreadIds.Count;
        bool allEight = recorded == 8;
        for (int id = 1; id <= 8; id++)
            if (!Solution.TaskThreadIds.ContainsKey(id)) allEight = false;

        var seen = new HashSet<int>();
        foreach (var pair in Solution.TaskThreadIds) seen.Add(pair.Value);
        int distinct = seen.Count;

        var result = new HarnessResult();

        result.Add(
            name: "tasks-started",
            passed: recorded > 0,
            expected: "at least one task ran (something was handed to the pool)",
            actual: $"tasks recorded = {recorded}",
            message: "Nothing ran. Did you call ThreadPool.QueueUserWorkItem for the tasks? " +
                     "DoTask only runs when you hand it to the pool.");

        result.Add(
            name: "all-eight-done",
            passed: allEight,
            expected: "all 8 tasks ran (ids 1-8, each exactly once)",
            actual: $"recorded ids = {recorded} of 8",
            message: "Some task ids are missing. If one number (like 9) seems to have replaced the " +
                     "others, your tasks captured the loop variable - give each task its own copy: " +
                     "int mine = i; inside the loop, then DoTask(mine).");

        result.Add(
            name: "pool-reused-workers",
            passed: distinct >= 1 && distinct <= 4,
            expected: "between 1 and 4 distinct workers did all 8 tasks (the pool reuses!)",
            actual: $"{distinct} distinct thread id(s) did the work",
            message: "Expected 1-4 borrowed workers but saw a different crowd. If nothing ran, fix that " +
                     "first; if every task got its OWN new thread, you are hiring instead of borrowing - " +
                     "queue the tasks with ThreadPool.QueueUserWorkItem.");

        return result;
    }
}
