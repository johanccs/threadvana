using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.SetCacheValue("db", "connected");
        var tasks = new Task<string>[10];
        for (var i = 0; i < 10; i++)
            tasks[i] = Task.Run(() => Solution.GetCacheValue("db"));
        await Task.WhenAll(tasks);
        result.Add(
            name: "concurrent-reads-succeed",
            passed: tasks.All(t => t.Result == "connected"),
            expected: "10 concurrent reads should all return \"connected\"",
            actual: $"{tasks.Count(t => t.Result == "connected")}/10 got it",
            message: "");
        return result;
    }
}
