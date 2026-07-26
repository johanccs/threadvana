using System;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> RetryWithBackoffAsync(Func<Task<string>> operation, int maxRetries)
    {
        for (var i = 0; i <= maxRetries; i++)
        {
            try { return await operation(); }
            catch
            {
                if (i == maxRetries) return "failed";
                await Task.Delay(100 * (int)Math.Pow(2, i));
            }
        }
        return "failed";
    }
}
