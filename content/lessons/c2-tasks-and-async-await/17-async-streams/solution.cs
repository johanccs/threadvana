using System.Collections.Generic;
using System.Threading.Tasks;

public static class Solution
{
    public static async IAsyncEnumerable<int> CountToAsync(int n)
    {
        for (var i = 1; i <= n; i++)
        {
            await Task.Delay(50);
            yield return i;
        }
    }

    public static async Task<int> SumStreamAsync(int n)
    {
        var sum = 0;
        await foreach (var x in CountToAsync(n))
            sum += x;
        return sum;
    }
}
