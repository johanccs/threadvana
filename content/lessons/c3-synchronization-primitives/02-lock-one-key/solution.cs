using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    // The two accounts. The checker hammers Transfer() and reads these.
    public static int AccountA = 1000;
    public static int AccountB = 1000;

    // PROVIDED - the "bathroom key" for your lock. Use THIS SAME object.
    public static readonly object Gate = new object();

    // PROVIDED - resets the accounts before each hammer round. Do not change.
    public static void Reset()
    {
        AccountA = 1000;
        AccountB = 1000;
    }

    public static void Transfer(int amount)
    {
        // THE FIX: one shared key around the WHOLE critical section.
        // Only one thread at a time can check-and-move, so the check can
        // never go stale and the two moves can never interleave.
        lock (Solution.Gate)
        {
            if (AccountA >= amount)     // CHECK - inside the lock!
            {
                Thread.Yield();         // even if a thread slips in, it WAITS at the lock
                AccountA -= amount;     // SUBTRACT
                AccountB += amount;     // ADD
            }
        }
    }
}