using System;
using System.Linq;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var nums = Enumerable.Range(1, 10);
        Trace.Log("message", "PLINQ with a faulted item — watch AggregateException");
        try
        {
            nums.AsParallel().Select(n =>
            {
                if (n == 5) throw new InvalidOperationException("bad item");
                return n;
            }).ToArray();
        }
        catch (AggregateException ae)
        {
            Trace.Log("message", $"AggregateException caught: {ae.InnerExceptions.Count} inner exception(s)");
        }
    }
}
