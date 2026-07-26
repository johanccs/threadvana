using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int Balance = 100;
    private static readonly object _gate = new();

    public static Task<string> Transfer(int amount)
    {
        lock (_gate) { Balance += amount; }
        return Task.FromResult("fixed");
    }
}
