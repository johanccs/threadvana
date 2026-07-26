using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var cpuResult = await Solution.ClassifyWorkloadAsync(true);
        var ioResult = await Solution.ClassifyWorkloadAsync(false);
        result.Add(
            name: "classifies-correctly",
            passed: cpuResult == "15" && ioResult == "5",
            expected: "CPU path: sum=15, I/O path: count=5",
            actual: $"sum={cpuResult}, count={ioResult}",
            message: cpuResult != "15" || ioResult != "5" ? "Check Parallel.ForEach for CPU, Task.WhenAll for I/O." : "");
        return result;
    }
}
