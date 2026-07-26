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
        // TODO: 1. Loop i from 0 to 2. In each round, create a thread whose
        //          body stores ITS OWN number into Solution.Results at its
        //          own slot.
        //       2. Start all three threads (keep them in a Thread[]).
        //       3. Join all three before Run() returns.
        //
        // Careful: capturing the loop variable directly is THE classic bug -
        // all your threads may end up reading the same final value (3!).
    }
}
