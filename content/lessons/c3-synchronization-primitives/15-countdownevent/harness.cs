using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.WaitForWorkersAsync();
        result.Add(
            name: "workers-signal-done",
            passed: answer == "done",
            expected: "\"done\" after all workers signal",
            actual: $"\"{answer}\"",
            message: answer != "done" ? "Check CountdownEvent(3), 3 Task.Run, Signal, Wait." : "");
        return result;
    }
}
