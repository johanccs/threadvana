using System.Linq;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var nums = Enumerable.Range(1, 20);
        var squares = nums.AsParallel().Select(n => n * n).ToArray();
        Trace.Log("message", $"PLINQ squares: [{string.Join(",", squares.Take(5))}...] — 20 items, parallel");
    }
}
