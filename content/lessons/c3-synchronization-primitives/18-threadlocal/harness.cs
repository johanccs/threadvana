using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var ids = new ConcurrentBag<string>();
        var tasks = new Task[4];
        for (var i = 0; i < 4; i++)
            tasks[i] = Task.Run(() => ids.Add(Solution.GetThreadLocalId()));
        await Task.WhenAll(tasks);
        var distinct = ids.Distinct().Count();
        result.Add(
            name: "unique-per-thread",
            passed: distinct >= 2,
            expected: "Different threads should get different ids",
            actual: $"{distinct} distinct ids from 4 threads",
            message: distinct < 2 ? "ThreadLocal not giving per-thread values." : "");
        return result;
    }
}
