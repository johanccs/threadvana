using System;
using System.Linq;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Run();
        await Task.Delay(200);

        var result = new HarnessResult();

        var count = Solution.Counts.Count;
        var allCorrect = Enumerable.Range(0, 500).All(i => Solution.Counts.TryGetValue(i, out var v) && v == 1);

        result.Add(
            name: "all-items-stored",
            passed: count == 500 && allCorrect,
            expected: "the dictionary holds 500 entries for keys 0..499",
            actual: $"Count = {count}, all correct: {allCorrect}",
            message: "The normal Dictionary breaks under concurrent writes. " +
                     "Switch to ConcurrentDictionary and use TryAdd.");

        return result;
    }
}
