using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> WaitForWorkersAsync()
    {
        var cde = new CountdownEvent(3);
        for (var i = 0; i < 3; i++)
            Task.Run(async () => { await Task.Delay(100); cde.Signal(); });
        cde.Wait();
        return "done";
    }
}
