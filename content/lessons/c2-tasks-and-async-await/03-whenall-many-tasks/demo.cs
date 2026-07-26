using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    // One breakfast item: cooks for 600 ms, tracing its span so you can
    // SEE whether the spans line up in a staircase or in a stack.
    private static async Task CookAsync(string item)
    {
        Trace.Log("work-start", $"{item} starts (thread {Environment.CurrentManagedThreadId})");
        await Task.Delay(600); // cooking takes time - no thread is held
        Trace.Log("work-end", $"{item} is done");
    }

    public static async Task RunAsync()
    {
        // ROUND 1 - sequential: each item waits for the previous one.
        // Three 600 ms items, one after another = ~1.8 s.
        Trace.Log("message", "ROUND 1: one by one (slow staircase)");
        await CookAsync("Eggs");
        await CookAsync("Bacon");
        await CookAsync("Toast");
        Trace.Log("message", "Round 1 served - that took a while");

        // ROUND 2 - all at once: start everything FIRST, then await the
        // combined receipt. Three 600 ms items together = ~0.6 s.
        Trace.Log("message", "ROUND 2: Task.WhenAll (fast stack)");
        Task eggs  = CookAsync("Eggs");   // starts immediately - receipt in hand
        Task bacon = CookAsync("Bacon");  // starts too
        Task toast = CookAsync("Toast");  // starts too
        await Task.WhenAll(eggs, bacon, toast); // one buzzer for the whole set

        Trace.Log("message", "Breakfast is served - same work, a third of the time!");
    }
}