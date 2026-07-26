using System;
using System.Threading;

public static class Solution
{
    // Three work slots for your three threads, plus one TRIPWIRE slot.
    // Slot i should receive the value i (for i = 0, 1, 2).
    // Slot 3 must stay -1 forever: if it changes, a thread read the
    // loop variable after the loop had finished. The checker looks!
    public static int[] Results = { -1, -1, -1, -1 };

    public static void Run()
    {
        var threads = new Thread[3];

        for (int i = 0; i < 3; i++)
        {
            int mine = i; // THE FIX: a fresh copy for THIS loop round.
            threads[i] = new Thread(() =>
            {
                // Each thread stores the number it was personally given,
                // into its own slot. The tripwire slot 3 stays untouched.
                Solution.Results[mine] = mine;
            });
            threads[i].Start();
        }

        foreach (var t in threads) t.Join();
    }
}
