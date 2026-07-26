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
        lock (CountGate) // a tiny lock - you just learned this in lock - One Key to the Bathroom!
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

    // TODO step 1: create the parking lot here -
    // public static SemaphoreSlim Lot = new SemaphoreSlim(2);

    // YOUR JOB (step 2): limit this to AT MOST 2 calls at the same time.
    public static async Task CallApiLimitedAsync()
    {
        // NO LIMIT right now - every caller barges straight in, so all
        // 6 calls overlap. Fix it:
        //   await Lot.WaitAsync();
        //   try { await CallApiAsync(); }
        //   finally { Lot.Release(); }
        await CallApiAsync();
    }
}