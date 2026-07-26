using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        const int maxPerSecond = 5;

        var limiter = new Solution.RateLimiter(maxPerSecond);

        // Let the timer accumulate a few tokens so the test isn't all zeros.
        await Task.Delay(400);

        // 10 concurrent callers, each tries 10 times with a small gap — 100 total attempts.
        // The limiter starts empty so the first few seconds are ramp-up.
        var passed = 0;
        var tasks = new Task[10];
        for (var t = 0; t < 10; t++)
        {
            tasks[t] = Task.Run(async () =>
            {
                for (var i = 0; i < 10; i++)
                {
                    if (await limiter.TryActionAsync())
                        Interlocked.Increment(ref passed);
                    await Task.Delay(30);
                }
            });
        }

        await Task.WhenAll(tasks);

        var ok = passed > 0 && passed <= maxPerSecond * 4; // At most 4 seconds of capacity.
        result.Add(
            name: "respects-rate-limit",
            passed: ok,
            expected: $"Between 1 and {maxPerSecond * 4} actions should pass",
            actual: $"{passed} actions passed",
            message: ok ? ""
                : passed == 0
                    ? "No actions passed — the limiter is too strict. Check your token refill logic."
                    : $"{passed - maxPerSecond * 4} too many actions got through. The limiter is leaking.");

        return result;
    }
}
