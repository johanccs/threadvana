using System.Diagnostics;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var sw = Stopwatch.StartNew();
        var tasks = new Task[6];
        for (var i = 0; i < 6; i++)
            tasks[i] = Task.Run(async () => await Solution.ProcessAsync(i));
        await Task.WhenAll(tasks);
        sw.Stop();

        // 6 tasks × 200ms ÷ max 3 concurrent ⇒ at least ~400ms
        result.Add(
            name: "max-concurrency-respected",
            passed: sw.ElapsedMilliseconds >= 350,
            expected: "At least ~400ms (6 × 200ms ÷ 3 max concurrency)",
            actual: $"{sw.ElapsedMilliseconds}ms",
            message: sw.ElapsedMilliseconds < 350
                ? $"Too fast ({sw.ElapsedMilliseconds}ms) — semaphore should cap at 3."
                : "");
        return result;
    }
}

