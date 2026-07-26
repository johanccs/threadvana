using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    // The provided helpers record each step here, in order.
    public static List<string> Log = new List<string>();

    // PROVIDED - boils the water (takes a moment). Do not change.
    public static async Task<bool> BoilWaterAsync()
    {
        await Task.Delay(300); // the kettle needs time - no thread is held while waiting
        Log.Add("water boiled");
        return true;
    }

    // PROVIDED - pours the coffee. Do not change.
    public static async Task PourCoffee()
    {
        await Task.Delay(50); // pouring takes a moment too
        Log.Add("coffee poured");
    }

    // THE RIGHT WAY: async lets the method pause; await marks where.
    public static async Task<string> MakeCoffeeAsync()
    {
        // await = pause the METHOD, not the thread. The thread stays free
        // while the kettle boils, and the method resumes here when done.
        await BoilWaterAsync();

        // This line can only run AFTER the water is boiled - order is kept.
        await PourCoffee();

        // The string rides back to the caller inside Task<string>.
        return "coffee ready";
    }
}