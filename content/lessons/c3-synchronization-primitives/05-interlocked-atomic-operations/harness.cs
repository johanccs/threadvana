using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();

        // --- Check atomic increment: 8 threads × 50,000 each = 400,000 ---
        Solution.Count = 0;
        var threads = new Thread[8];
        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                for (var j = 0; j < 50_000; j++) Solution.AddOne();
            });
            threads[i].Start();
        }
        for (var i = 0; i < threads.Length; i++) threads[i].Join();

        var final = Solution.Count;
        result.Add(
            name: "atomic-increment-no-race",
            passed: final == 400_000,
            expected: "Count should be 400,000 after 8 threads each call AddOne 50,000 times",
            actual: $"Count ended up as {final}",
            message: final != 400_000
                ? $"The counter lost {400_000 - final} increments — that is a race condition. Did you use Interlocked.Increment?"
                : "");

        // --- Check Exchange: reset to 42 should give 42 ---
        Solution.ResetTo(42);
        var afterReset = Solution.Count;
        result.Add(
            name: "exchange-works",
            passed: afterReset == 42,
            expected: "Count should be 42 after calling ResetTo(42)",
            actual: $"Count is {afterReset}",
            message: afterReset != 42
                ? "Interlocked.Exchange(ref Count, value) atomically sets the counter. Check your ResetTo method."
                : "");

        return result;
    }
}
