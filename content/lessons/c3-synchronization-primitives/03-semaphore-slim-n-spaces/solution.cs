using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    // ----------------------------------------------------------------
    // PROVIDED - the checker's counters (do not change). They measure
    // how many calls are inside right now and the highest overlap seen.
    // ----------------------------------------------------------------
    private static readonly object CountGate = new object();
    public static int InsideNow = 0;  // calls inside RIGHT NOW
    public static int MaxInside = 0;  // highest overlap ever seen
    public static int Completed = 0;  // calls that finished

    public static void ObserveEnter()
    {
        lock (CountGate)
        {
            InsideNow++;
            if (InsideNow > MaxInside) MaxInside = InsideNow;
        }
    }

    public static void ObserveExit()
    {
        lock (CountGate)
        {
            InsideNow--;
            Completed++;
        }
    }

    // PROVIDED - pretends to be a slow API (~200 ms). Do not change.
    public static async Task CallApiAsync()
    {
        ObserveEnter();        // checker: one more call is inside
        await Task.Delay(200); // the API is slow - no thread is held
        ObserveExit();         // checker: this call left
    }

    // STEP 1 - the parking lot: AT MOST 2 calls inside at once.
    public static SemaphoreSlim Lot = new SemaphoreSlim(2);

    // STEP 2 - the bouncer pattern: WaitAsync in, Release out (in finally!).
    public static async Task CallApiLimitedAsync()
    {
        await Lot.WaitAsync();        // drive in - or queue until a space frees up
        try
        {
            await CallApiAsync();     // the limited work: at most 2 are EVER here
        }
        finally
        {
            Lot.Release();            // ALWAYS drive out - even on a crash
        }
    }
}