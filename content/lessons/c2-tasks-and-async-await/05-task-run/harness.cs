using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Done = new CountdownEvent(3);

        // Wait up to 10 s for ShoutAllAsync — it should start three tasks
        // and then block on Done until the third Shout signals.
        var shoutTask = Solution.ShoutAllAsync();
        var finished = await Task.WhenAny(shoutTask, Task.Delay(10_000));
        if (finished != shoutTask)
        {
            result.Add(
                name: "completes-in-time",
                passed: false,
                expected: "ShoutAllAsync should finish after all three waiters shout",
                actual: "ShoutAllAsync did not finish within 10 seconds",
                message: "ShoutAllAsync is hanging — check that Done.Wait() is called AFTER all three Task.Run calls have been started.");
            return result;
        }

        // If ShoutAllAsync threw, let it surface.
        await shoutTask;

        var waited = Solution.Done.Wait(0);
        result.Add(
            name: "all-three-done",
            passed: waited,
            expected: "All three waiters should have signalled Done",
            actual: waited ? "Done counted down to zero" : "Done did not reach zero",
            message: waited
                ? ""
                : "Did at least one Task.Run call not call Shout? Each waiter should run Shout exactly once via Task.Run, which calls Done.Signal().");

        return result;
    }
}
