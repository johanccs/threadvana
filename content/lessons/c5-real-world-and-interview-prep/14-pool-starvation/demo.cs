using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Firing 30 synchronous sleeps on the pool — watch work piling up");
        var tasks = new Task[30];
        for (var i = 0; i < 30; i++)
            tasks[i] = Task.Run(() => { Thread.Sleep(400); });
        await Task.WhenAll(tasks);
        Trace.Log("message", "All finished — but pool threads were starved while they slept");
    }
}
