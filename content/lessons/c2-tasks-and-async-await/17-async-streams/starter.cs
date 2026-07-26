using System.Collections.Generic;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Yield numbers 1..n, pausing 50ms between each. Use async IAsyncEnumerable.</summary>
    public static async IAsyncEnumerable<int> CountToAsync(int n)
    {
        for (var i = 1; i <= n; i++)
        {
            // TODO: await Task.Delay(50); yield return i;
            yield return i;
        }
    }

    /// <summary>Consume the stream and return the sum.</summary>
    public static async Task<int> SumStreamAsync(int n)
    {
        var sum = 0;
        // TODO: await foreach (var x in CountToAsync(n)) sum += x;
        return sum;
    }
}
