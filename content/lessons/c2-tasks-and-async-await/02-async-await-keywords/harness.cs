using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        // Call the method and look at the receipt it hands back.
        Task<string> order = Solution.MakeCoffeeAsync();

        // A properly awaited method is still WORKING at this point (the water
        // takes ~300 ms), so its Task should NOT be finished yet. A blocking
        // .Result/.Wait() version has already parked a thread and completed
        // everything before returning.
        bool stillWorking = !order.IsCompleted;

        // Collect what the method eventually serves.
        string served = await order;

        // Fair chance: if the learner fired the helpers without awaiting them,
        // their log entries may land late. Give them a moment to arrive so the
        // order check reads the true story.
        await Task.Delay(600);

        var log = Solution.Log;
        int boiledAt = log.IndexOf("water boiled");
        int pouredAt = log.IndexOf("coffee poured");

        var result = new HarnessResult();

        result.Add(
            name: "something-happened",
            passed: log.Count > 0,
            expected: "MakeCoffeeAsync runs the helpers, which record steps in Solution.Log",
            actual: $"Log entries: {log.Count} ({string.Join(", ", log)})",
            message: "The log is empty, so no helper ever ran. Did you call BoilWaterAsync() and PourCoffee() inside MakeCoffeeAsync()?");

        result.Add(
            name: "says-coffee-ready",
            passed: served == "coffee ready",
            expected: "MakeCoffeeAsync returns the string \"coffee ready\"",
            actual: $"returned: \"{served ?? "(null)"}\"",
            message: "The method should hand back exactly \"coffee ready\". Check the spelling - and remember the string rides home inside Task<string>.");

        result.Add(
            name: "boiled-before-poured",
            passed: boiledAt >= 0 && pouredAt > boiledAt,
            expected: "\"water boiled\" appears in the log BEFORE \"coffee poured\"",
            actual: $"log order: {string.Join(" then ", log)}",
            message: "The coffee was poured before the water finished boiling. Most likely cause: BoilWaterAsync() was called WITHOUT await, so the method rushed ahead. " +
                     "Write: await BoilWaterAsync(); and only then await PourCoffee();");

        result.Add(
            name: "no-thread-parked",
            passed: stillWorking,
            expected: "the Task from MakeCoffeeAsync is still unfinished right after the call (proof a real await is inside)",
            actual: stillWorking ? "the Task was still running - a real await" : "the Task was already complete the moment it was returned",
            message: "Your method finished everything before it returned, which means a thread sat parked inside (.Result / .Wait()), or there is no await at all. " +
                     "Rewrite with await: the METHOD pauses while the water boils, but the thread stays free.");

        return result;
    }
}