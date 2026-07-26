using System.Collections.Concurrent;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var dict = new ConcurrentDictionary<string, int>();
        dict.TryAdd("cpu", 0);
        dict.TryAdd("mem", 0);

        var tasks = new Task[6];
        for (var i = 0; i < 6; i++)
        {
            var r = i % 2 == 0 ? "cpu" : "mem";
            tasks[i] = Task.Run(() => dict.AddOrUpdate(r, _ => 1, (_, old) => old + 1));
        }
        await Task.WhenAll(tasks);
        Trace.Log("message", $"cpu: {dict["cpu"]}, mem: {dict["mem"]} — expected cpu=3, mem=3");
    }
}
