using System.Linq;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var nums = Enumerable.Range(1, 10);
        Trace.Log("message", "Unordered (default):");
        nums.AsParallel().Select(n => { Trace.Log("work-start", $"Processing {n}"); return n; }).ForAll(_ => { });
        Trace.Log("message", "ForAll sent each result directly — no merge overhead");
    }
}
