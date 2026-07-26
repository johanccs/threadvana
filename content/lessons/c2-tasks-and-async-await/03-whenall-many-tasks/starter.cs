using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    // The helpers record each finished item here. The checker reads it.
    public static List<string> Finished = new List<string>();

    // PROVIDED - boils the eggs (~500 ms), then records "eggs". Do not change.
    public static async Task BoilEggsAsync()
    {
        await Task.Delay(500); // the pot needs time - no thread is held
        Finished.Add("eggs");
    }

    // PROVIDED - fries the bacon (~500 ms), then records "bacon". Do not change.
    public static async Task FryBaconAsync()
    {
        await Task.Delay(500);
        Finished.Add("bacon");
    }

    // PROVIDED - toasts the bread (~500 ms), then records "toast". Do not change.
    public static async Task ToastBreadAsync()
    {
        await Task.Delay(500);
        Finished.Add("toast");
    }

    public static async Task MakeBreakfastAsync()
    {
        // THIS WORKS, but it is the slow way: each item waits for the
        // previous one, so breakfast takes ~1.5 s. Your job: start all
        // three FIRST (keep each Task receipt), then await Task.WhenAll(...).
        await BoilEggsAsync();   // 500 ms...
        await FryBaconAsync();   // ...then another 500 ms...
        await ToastBreadAsync(); // ...then another 500 ms. Too slow!
    }
}