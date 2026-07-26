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

    // ================================================================
    // UNSAFE ON PURPOSE - nothing protects the check and the moves, so
    // under load the balance goes NEGATIVE and the total drifts.
    // Your job: wrap EVERYTHING below in  lock (Solution.Gate) { ... }
    // ================================================================
    public static void Transfer(int amount)
    {
        if (AccountA >= amount)     // CHECK: is there enough money?
        {
            Thread.Yield();         // lets another thread slip in (happens by chance in real code!)
            AccountA -= amount;     // SUBTRACT from A
            AccountB += amount;     // ADD to B
        }
    }
}