using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();

        // Hold the lock first so TryEnter sees competition.
        var holder = Task.Run(() => { lock (Solution.Gate) Thread.Sleep(1000); });
        await Task.Delay(50);

        var answer = await Solution.TryEnterWithTimeoutAsync();
        result.Add(
            name: "returns-timeout",
            passed: answer == "timeout",
            expected: "\"timeout\" when the lock is already held",
            actual: $"\"{answer}\"",
            message: answer != "timeout"
                ? "The lock is held by another thread — TryEnter with 500ms against a 1000ms hold should time out."
                : "");

        await holder;
        return result;
    }
}
