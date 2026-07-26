using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        // Time the whole breakfast. Three 500 ms items cooked one-by-one take
        // ~1.5 s; cooked at the same time they take ~0.5 s. A correct
        // MakeBreakfastAsync only completes when ALL items are recorded,
        // so reading the log right after the await tells the true story.
        long startedAt = Environment.TickCount64;
        await Solution.MakeBreakfastAsync();
        long elapsedMs = Environment.TickCount64 - startedAt;

        var result = new HarnessResult();

        result.Add(
            name: "something-cooked",
            passed: Solution.Finished.Count > 0,
            expected: "at least one breakfast item was cooked and recorded",
            actual: $"Finished entries: {Solution.Finished.Count}",
            message: "Nothing was recorded. Either the helpers were never called, or their tasks were never awaited - " +
                     "without await, MakeBreakfastAsync finishes long before the cooking does. " +
                     "Start all three, then await Task.WhenAll(eggs, bacon, toast);");

        result.Add(
            name: "all-items-cooked",
            passed: Solution.Finished.Contains("eggs") && Solution.Finished.Contains("bacon") && Solution.Finished.Contains("toast"),
            expected: "all three items recorded: eggs, bacon, toast",
            actual: $"Finished = [{string.Join(", ", Solution.Finished)}]",
            message: "Some items are missing. Make sure ALL THREE helpers are called and all three receipts go into Task.WhenAll(...) - " +
                     "it only rings when every task you gave it is done.");

        result.Add(
            name: "cooked-in-parallel",
            passed: elapsedMs < 900,
            expected: "breakfast ready in under 900 ms (three 500 ms items cooking AT THE SAME TIME)",
            actual: $"took {elapsedMs} ms",
            message: "The items cooked one-by-one (~1.5 s). Most likely cause: you awaited each task before starting the next. " +
                     "Start ALL THREE first - Task eggs = BoilEggsAsync(); and so on - and only then: await Task.WhenAll(eggs, bacon, toast);");

        return result;
    }
}