using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static int _a = 100, _b = 100;

    public static async Task RunAsync()
    {
        Trace.Log("message", "Two transfers — both see 200, one may overwrite the other (race)");
        var t1 = Task.Run(() => { var x = _a; Thread.Sleep(10); _a = x + 50; });
        var t2 = Task.Run(() => { var y = _a; Thread.Sleep(5); _a = y + 30; });
        await Task.WhenAll(t1, t2);
        Trace.Log("message", $"_a = {_a} (could be 180 if race, or 150 if one lost)");
    }
}
