using System;
using System.Linq;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly int[] Data = Enumerable.Range(1, 10).ToArray();

    public static Task<string> HandleFaultedPLINQAsync()
    {
        try
        {
            Data.AsParallel().Select(n =>
            {
                if (n > 5) throw new InvalidOperationException();
                return n;
            }).ToArray();
            return Task.FromResult("0");
        }
        catch (AggregateException ae)
        {
            return Task.FromResult(ae.InnerExceptions.Count.ToString());
        }
    }
}
