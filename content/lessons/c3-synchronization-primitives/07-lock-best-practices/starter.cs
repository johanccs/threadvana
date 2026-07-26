using System.Threading;

public static class Solution
{
    /// <summary>Shared counter — protect it with a lock.</summary>
    public static int Counter;

    /// <summary>Add one to the counter safely.</summary>
    public static void Increment()
    {
        // TODO: use lock to protect Counter++
        Counter++;
    }

    /// <summary>Reset the counter to zero safely.</summary>
    public static void Reset()
    {
        // TODO: lock here too
        Counter = 0;
    }
}
