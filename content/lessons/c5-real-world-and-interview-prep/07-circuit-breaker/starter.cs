using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    private static int _failures;

    public static async Task<string> CallWithCircuitBreakerAsync(Func<Task<string>> operation, int threshold)
    {
        if (Volatile.Read(ref _failures) >= threshold)
            return "open";
        try
        {
            var result = await operation();
            Interlocked.Exchange(ref _failures, 0);
            return result;
        }
        catch
        {
            Interlocked.Increment(ref _failures);
            return "failed";
        }
    }
}
