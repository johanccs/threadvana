using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int Counter;
    private static SpinLock _spin = new();

    public static Task<string> IncrementWithSpinLock()
    {
        // TODO: acquire SpinLock, increment, release, return "incremented"
        return Task.FromResult("not implemented");
    }
}
