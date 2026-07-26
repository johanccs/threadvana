using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> StarveThePoolAsync()
    {
        var tasks = new Task[50];
        for (var i = 0; i < 50; i++) tasks[i] = Task.Run(() => Thread.Sleep(500));
        await Task.WhenAll(tasks);
        return "starved";
    }
}
