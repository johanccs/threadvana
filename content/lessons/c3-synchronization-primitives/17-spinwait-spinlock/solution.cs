using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int Counter;
    private static SpinLock _spin = new();

    public static Task<string> IncrementWithSpinLock()
    {
        var taken = false;
        try
        {
            _spin.Enter(ref taken);
            Counter++;
        }
        finally { if (taken) _spin.Exit(); }
        return Task.FromResult("incremented");
    }
}
