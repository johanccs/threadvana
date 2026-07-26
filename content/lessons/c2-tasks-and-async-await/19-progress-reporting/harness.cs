using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var reported = new ConcurrentBag<int>();
        var progress = new Progress<int>(v => reported.Add(v));
        await Solution.RunWithProgressAsync(progress);
        var values = reported.ToArray();
        var passed = values.Length >= 6 && values.Contains(0) && values.Contains(100);
        result.Add(
            name: "reports-all-progress-steps",
            passed: passed,
            expected: "At least 6 reports including 0 and 100",
            actual: $"{values.Length} reports: [{string.Join(",", values)}]",
            message: passed ? "" : "Loop 0..5, call progress.Report(i * 20) each iteration.");
        return result;
    }
}
