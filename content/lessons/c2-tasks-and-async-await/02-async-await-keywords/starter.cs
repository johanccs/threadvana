using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    // The provided helpers record each step here, in order.
    // The checker reads this to verify the ORDER things happened.
    public static List<string> Log = new List<string>();

    // PROVIDED - boils the water (takes a moment). Do not change.
    // Logs "water boiled", then returns true.
    public static async Task<bool> BoilWaterAsync()
    {
        await Task.Delay(300); // the kettle needs time - no thread is held while waiting
        Log.Add("water boiled");
        return true;
    }

    // PROVIDED - pours the coffee. Do not change.
    // Logs "coffee poured".
    public static async Task PourCoffee()
    {
        await Task.Delay(50); // pouring takes a moment too
        Log.Add("coffee poured");
    }

    // ================================================================
    // THIS IS THE WRONG WAY - it works, but it PARKS A THREAD while
    // waiting. Your job: rewrite this method with async + await.
    // New signature: public static async Task<string> MakeCoffeeAsync()
    // ================================================================
    public static Task<string> MakeCoffeeAsync()
    {
        _ = BoilWaterAsync().Result; // WRONG: .Result blocks a thread until the water is boiled
        PourCoffee().Wait();         // WRONG: .Wait() blocks it again for the pour
        return Task.FromResult("coffee ready");
    }
}