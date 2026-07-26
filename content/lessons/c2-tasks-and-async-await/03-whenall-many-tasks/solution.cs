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
        // 1. Start ALL THREE first. Calling an async method already starts the
        //    work - what you get back is a receipt, not the finished dish.
        Task eggs  = BoilEggsAsync();
        Task bacon = FryBaconAsync();
        Task toast = ToastBreadAsync();

        // 2. One combined buzzer: rings when EVERY item is ready.
        //    The three 500 ms jobs overlap, so breakfast takes ~500 ms total.
        await Task.WhenAll(eggs, bacon, toast);
    }
}