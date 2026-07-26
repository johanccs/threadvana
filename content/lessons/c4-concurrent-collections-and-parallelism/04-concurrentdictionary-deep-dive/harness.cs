using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();

        // Clear any starter residue.
        Solution.Scores.Clear();

        // 20 threads, 3 players, each thread adds random points 100 times.
        var rng = new ThreadLocal<Random>(() => new Random());
        var names = new[] { "Alice", "Bob", "Charlie" };
        // Compute expected totals locally (single-threaded).
        var expected = new ConcurrentDictionary<string, int>();
        var threads = new Thread[20];
        for (var t = 0; t < threads.Length; t++)
        {
            threads[t] = new Thread(() =>
            {
                var rand = rng.Value!;
                for (var i = 0; i < 100; i++)
                {
                    var name = names[rand.Next(names.Length)];
                    var pts = rand.Next(1, 11);
                    Solution.RecordScore(name, pts);
                    expected.AddOrUpdate(name, _ => pts, (_, old) => old + pts);
                }
            });
            threads[t].Start();
        }
        for (var t = 0; t < threads.Length; t++) threads[t].Join();

        var allGood = true;
        foreach (var kvp in expected)
        {
            var got = Solution.GetScore(kvp.Key);
            if (got != kvp.Value)
            {
                allGood = false;
                result.Add(
                    name: $"score-correct-{kvp.Key.ToLower()}",
                    passed: false,
                    expected: $"{kvp.Key} should have {kvp.Value} points",
                    actual: $"{kvp.Key} has {got} points",
                    message: $"Some RecordScore or GetScore calls for {kvp.Key} are not working atomically. Did you use AddOrUpdate and TryGetValue correctly?");
            }
        }

        if (allGood)
        {
            result.Add(
                name: "all-scores-correct",
                passed: true,
                expected: "All players got exactly their expected totals",
                actual: "All correct",
                message: "");
        }

        return result;
    }
}
