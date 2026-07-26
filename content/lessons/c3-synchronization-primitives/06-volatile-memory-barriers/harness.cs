using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.IsRunning = false;

        var waiter = Task.Run(() => Solution.WaitUntilStarted());
        await Task.Delay(100);
        Solution.SignalStart();

        var finished = await Task.WhenAny(waiter, Task.Delay(5000));
        if (finished != waiter)
        {
            result.Add(
                name: "detects-start",
                passed: false,
                expected: "WaitUntilStarted should see IsRunning within 5 seconds",
                actual: "Timed out",
                message: "The method is spinning forever — does the loop read the volatile flag?");
            return result;
        }

        var answer = await waiter;
        result.Add(
            name: "returns-started",
            passed: answer == "started",
            expected: "\"started\"",
            actual: $"\"{answer}\"",
            message: answer != "started" ? "Once the flag is true, return \"started\"." : "");
        return result;
    }
}
