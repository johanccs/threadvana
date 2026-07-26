using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Return a two-word string describing changes in SynchronizationContext across an await.</summary>
    public static async Task<string> CheckContextAsync()
    {
        // TODO: check SynchronizationContext.Current before and after await Task.Delay(1)
        return "not implemented";
    }
}
